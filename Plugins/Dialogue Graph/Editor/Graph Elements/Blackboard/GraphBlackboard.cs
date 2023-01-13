//using System.Collections.Generic;
//using UnityEngine.UIElements;

//namespace DialogueEditor
//{
//    using Properties;

//    public class GraphBlackboard : GraphWindowBase
//    {
//        private readonly List<GraphAbstractProperty> m_Fields;

//        private readonly VisualElement m_RowsViewport;
//        private readonly VisualElement m_RowsContainer;

//        private readonly DragReorderHelper m_DragReorderHelper;

//        public GraphBlackboard(DialogueGraphView graphView, string title, string subTitle) :
//                             base(graphView, title, subTitle)
//        {
//            m_Fields = new List<GraphAbstractProperty>();

//            m_RowsViewport = new VisualElement();
//            m_RowsViewport.name = "rowsViewport";

//            m_RowsContainer = new VisualElement();
//            m_RowsContainer.name = "rowsContainer";

//            m_ContentContainer.Add(m_RowsViewport);
//            m_RowsViewport.Add(m_RowsContainer);

//            m_DragReorderHelper = new DragReorderHelper(this, m_RowsViewport, m_RowsContainer);
//            m_DragReorderHelper.GraphView = m_GraphView;
//            m_DragReorderHelper.AcceptedObjects = element =>
//            {
//                return element is GraphAbstractProperty;
//            };

//            style.top = new StyleLength(20);
//        }

//        public override void AddToContent(VisualElement element)
//        {
//            GraphAbstractProperty abstractField = element as GraphAbstractProperty;
//            if (abstractField == null) return;

//            m_RowsContainer.Add(element);
//            m_Fields.Add(abstractField);
//        }

//        public override void ClearContent()
//        {
//            m_RowsContainer.Clear();
//            m_Fields.Clear();
//        }

//        public void Sort()
//        {
//            foreach (var field in m_Fields)
//            {
//                m_RowsContainer.Insert(field.SerializedListIndex, field);
//            }
//        }
//    }
//}