using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class DragReorderHelper
    {
        private readonly VisualElement m_ContentParent;
        private readonly VisualElement m_ContentContainer;

        private readonly VisualElement m_DragIndicator;
        private VisualElement m_DragHoveredElement;

        private enum DragPosition
        {
            Above,
            Below,
            Error
        }

        private DragPosition m_IndicatorPosition;

        public int IndicatorWidth { set => m_DragIndicator.style.height = value; }

        public Color IndicatorColor { set => m_DragIndicator.style.backgroundColor = value; }

        public GraphView GraphView { get; set; }

        public Func<GraphElement, bool> AcceptedObjects { get; set; }

        public DragReorderHelper(VisualElement contentParent, VisualElement contentViewport, VisualElement contentContainer)
        {
            m_DragIndicator = new VisualElement();
            m_DragIndicator.AddStyleSheets(DialogueResourceManager.DragIndicatorStyleSheets);

            m_DragIndicator.name = "dragIndicator";
            m_DragIndicator.pickingMode = PickingMode.Ignore;
            m_DragIndicator.visible = false;

            m_ContentParent = contentParent;
            m_ContentContainer = contentContainer;

            contentViewport.Add(m_DragIndicator);

            //m_ContentParent.AddManipulator(new Dragger()); // WRONG! Prevents from snapping and dragging to groups
            m_ContentContainer.AddManipulator(new Dragger() { clampToParentEdges = true });

            m_ContentParent.RegisterCallback<DragUpdatedEvent>(DragUpdatedEvent);
            m_ContentParent.RegisterCallback<DragPerformEvent>(DragPerformEvent);
            m_ContentParent.RegisterCallback<DragLeaveEvent>(OnDragStop);
            m_ContentParent.RegisterCallback<DragExitedEvent>(OnDragStop);
        }

        private void DragUpdatedEvent(DragUpdatedEvent e)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (AcceptedObjects == null && GraphView == null)
                return;
            for (int i = 0; i < GraphView.selection.Count; i++)
            {
                if (!AcceptedObjects(GraphView.selection[i] as GraphElement))
                {
                    e.StopPropagation();
                    return;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Vector2 localPos = m_ContentParent.ChangeCoordinatesTo(m_ContentContainer, e.localMousePosition);

            m_DragIndicator.visible = true;

            if (m_ContentContainer.childCount == 0)
            {
                m_DragIndicator.style.top = m_ContentContainer.layout.y;
                m_IndicatorPosition = DragPosition.Above;
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                return;
            }

            for (int i = 0; i < m_ContentContainer.childCount; i++)
            {
                VisualElement child = m_ContentContainer[i];

                float yMin = child.layout.yMin;
                float yMax = child.layout.yMax;
                float meanHeight = child.layout.height / 2;

                if (localPos.y > yMin && localPos.y < yMin + meanHeight)
                {
                    m_DragHoveredElement = child;
                    m_DragIndicator.style.top = yMin;
                    m_IndicatorPosition = DragPosition.Above;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    return;
                }
                else if (localPos.y > yMin + meanHeight && localPos.y < yMax)
                {
                    m_DragHoveredElement = child;
                    m_DragIndicator.style.top = yMax;
                    m_IndicatorPosition = DragPosition.Below;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    return;
                }
            }

            m_IndicatorPosition = DragPosition.Error;
        }

        private void DragPerformEvent(DragPerformEvent e)
        {
            if (AcceptedObjects == null && GraphView == null)
                return;

            int hoveredItemIndex = m_ContentContainer.IndexOf(m_DragHoveredElement);
            int firstItemInSelectionIndex = 0;
            for (int i = 0; i < GraphView.selection.Count; i++)
            {
                if (GraphView.selection[i] is GraphElement)
                {
                    firstItemInSelectionIndex = m_ContentContainer.IndexOf(GraphView.selection[i] as GraphElement);
                    break;
                }
            }

            int adjustingIndex =
                        (m_IndicatorPosition == DragPosition.Above && hoveredItemIndex > firstItemInSelectionIndex) ? -1 :
                        (m_IndicatorPosition == DragPosition.Below && hoveredItemIndex < firstItemInSelectionIndex ? 1 : 0);

            hoveredItemIndex = Mathf.Clamp(hoveredItemIndex + adjustingIndex, 0, m_ContentContainer.childCount);

            for (int i = 0; i < GraphView.selection.Count; i++)
            {
                if (!AcceptedObjects(GraphView.selection[i] as GraphElement))
                    continue;

                m_ContentContainer.Insert(hoveredItemIndex, GraphView.selection[i] as GraphElement);
            }

            m_DragIndicator.visible = false;
            e.StopPropagation();
        }

        private void OnDragStop(EventBase e)
        {
            m_DragIndicator.visible = false;
            m_IndicatorPosition = DragPosition.Error;
        }
    }
}