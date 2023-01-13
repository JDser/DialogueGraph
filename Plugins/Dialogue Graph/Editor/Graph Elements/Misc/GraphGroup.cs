using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class GraphGroup : Group, IGraphElement, IGraphEvents, IColorable, ISerializationCallbackReceiver
    {
        /* Serialized */

        [SerializeField] private string m_GuidSerialized;

        [SerializeField] private Vector2 m_Position;

        [SerializeField] private string m_Title;

        [SerializeField] private string[] m_AddedElementsSerialized;

        [SerializeField] private Color m_userDefinedColor = /* by default */ new Color(0.2f, 0.2f, 0.2f, 1);

        /* Non-Serializable */

        private Color m_currentColor;

        /* Properties */

        public DialogueGraphView GraphView { get; set; }

        public Guid Guid { get; set; }

        public Vector2 Position
        {
            get => m_Position;
            set
            {
                SetPosition(new Rect(value, Vector2.zero));
                m_Position = value;
            }
        }

        public Color UserDefinedColor => m_userDefinedColor;

        private List<Guid> AddedElements { get; set; }

        /* Initialization */

        public GraphGroup()
        {
            AddedElements = new List<Guid>();

            this.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) => BuildContextualMenu(evt)));

            this.Q("titleField").RegisterCallback<FocusOutEvent>(callback => 
            {
                DialogueEditorWindow.RegisterUndo("Change Group Title");
            });
        }

        public void Validate()
        {
            List<GraphElement> toAdd = new List<GraphElement>();

            foreach (Guid guid in AddedElements)
            {
                if (GraphView.graphElements.TryGet(guid, out GraphElement element))
                {
                    toAdd.Add(element);
                }
            }

            AddElements(toAdd);

            GraphView.OnHighlight += ChangeColor;
            ChangeColor(GraphView.CurrentColorMode);
        }

        public void Remove() 
        {
            List<GraphElement> toRemove = new List<GraphElement>(containedElements);
            RemoveElements(toRemove);
        }

        public void RemapConnections(Dictionary<Guid, Guid> remappedElements)
        {
            List<Guid> newGuidList = new List<Guid>();
            for (int i = 0; i < AddedElements.Count; i++)
            {
                if (remappedElements.ContainsKey(AddedElements[i]))
                    newGuidList.Add(remappedElements[AddedElements[i]]);
            }
            AddedElements = newGuidList;
            m_AddedElementsSerialized = null;
        }

        /* Callbacks */

        protected virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt) { }

        #region Color Managment

        public void ChangeColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.None:
                    m_currentColor = new Color(0.2f, 0.2f, 0.2f, 1);
                    break;

                case ColorMode.Category:
                    m_currentColor = m_userDefinedColor;
                    break;

                case ColorMode.UserDefined:
                    m_currentColor = m_userDefinedColor;
                    break;
            }

            style.borderBottomColor = style.borderLeftColor =
            style.borderRightColor = style.borderTopColor = m_currentColor;
        }

        public void SetUserDefinedColor(Color newColor)
        {
            m_currentColor = m_userDefinedColor = newColor;
            style.borderBottomColor = style.borderLeftColor =
            style.borderRightColor = style.borderTopColor = m_currentColor;
        }

        public void ResetUserDefinedColor()
        {
            m_currentColor = m_userDefinedColor = new Color(0.2f, 0.2f, 0.2f, 1);
            style.borderBottomColor = style.borderLeftColor =
            style.borderRightColor = style.borderTopColor = m_currentColor;
        }

        #endregion

        public override void OnSelected()
        {
            base.OnSelected();
            style.borderBottomColor = style.borderLeftColor =
            style.borderRightColor = style.borderTopColor = new Color(0.2666667f, 0.7529412f, 1, 1);
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            style.borderBottomColor = style.borderLeftColor =
            style.borderRightColor = style.borderTopColor = m_currentColor;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);
            foreach (GraphElement item in elements)
            {
                if (item is IGroupable groupable)
                {
                    groupable.Group = this;
                    Guid guid = ((IGraphElement)groupable).Guid;

                    if (!AddedElements.Contains(guid))
                        AddedElements.Add(guid);
                }
            }
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);
            foreach (GraphElement item in elements)
            {
                if(item is IGroupable groupable)
                {
                    groupable.Group = null;
                    AddedElements.Remove(((IGraphElement)groupable).Guid);
                }
            }
        }

        public virtual void OnBeforeSerialize()
        {
            m_GuidSerialized = Guid.ToString();
            m_Position = GetPosition().position;

            m_Title = title;

            m_AddedElementsSerialized = new string[AddedElements.Count];
            for (int i = 0; i < AddedElements.Count; i++)
                m_AddedElementsSerialized[i] = AddedElements[i].ToString();
        }

        public virtual void OnAfterDeserialize()
        {
            Guid = Guid.Parse(m_GuidSerialized);
            Position = m_Position;

            title = m_Title;

            AddedElements.Clear();
            for (int i = 0; i < m_AddedElementsSerialized.Length; i++)
                AddedElements.Add(Guid.Parse(m_AddedElementsSerialized[i]));
        }
    }
}