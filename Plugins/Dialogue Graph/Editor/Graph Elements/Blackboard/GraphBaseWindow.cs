using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class GraphBaseWindow : GraphElement
    {
        private readonly Label m_TitleLabel;

        private readonly Label m_SubTitleLabel;

        private readonly GenericMenu m_Menu;

        protected DialogueGraphView m_GraphView;

        protected VisualElement m_ContentContainer;

        public override string title
        {
            get => m_TitleLabel.text;
            set => m_TitleLabel.text = value;
        }

        public string subTitle
        {
            get => m_SubTitleLabel.text;
            set => m_SubTitleLabel.text = value;
        }

        public bool Visible
        {
            get => style.display == DisplayStyle.Flex;
            set
            {
                style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public Action<GraphBaseWindow> addItemRequested { get; set; }

        public GraphBaseWindow(DialogueGraphView graphView, string title, string subTitle)
        {
            m_GraphView = graphView;

            this.LoadUXMLTree(DialogueResourceManager.GraphWindowBaseUXML);
            this.AddToClassList("graphWindow");

            Add(new ResizableElement());

            m_TitleLabel = this.Q<Label>("titleLabel");
            m_SubTitleLabel = this.Q<Label>("subTitleLabel");
            m_ContentContainer = this.Q<ScrollView>();

            this.Q<Button>("addButton").clicked += () => addItemRequested(this);

            capabilities = Capabilities.Movable;
            this.AddManipulator(new Dragger() { clampToParentEdges = true });

            //Prevent zooming
            this.RegisterCallback<WheelEvent>(e => e.StopPropagation());

            m_Menu = new GenericMenu();

            this.title = title;
            this.subTitle = subTitle;
        }

        #region Content Methods
        public virtual void AddToContent(VisualElement element)
        {
            m_ContentContainer.Add(element);
        }

        public virtual void RemoveFromContent(VisualElement element)
        {
            if (m_ContentContainer.Contains(element))
                m_ContentContainer.Remove(element);
        }

        public virtual void ClearContent()
        {
            m_ContentContainer.Clear();
        }
        #endregion

        #region AddItemRequested Context Menu
        public void ShowContextMenu()
        {
            Debug.LogWarning("If Menu is not showing its becuse it's readonly");
            m_Menu.ShowAsContext();
        }

        public void AddContext(string actionName, GenericMenu.MenuFunction action)
        {
            m_Menu.AddItem(new GUIContent(actionName), false, action);
        }

        public void AddSeparator()
        {
            m_Menu.AddSeparator(null);
        }
        #endregion

        #region Serialization
        public FloatWindowData Serialize()
        {
            return new FloatWindowData(this.Visible, this.GetPosition());
        }

        public void Deserialize(FloatWindowData data)
        {
            this.Visible = data.IsActive;
            this.SetPosition(data.Rect);
        }
        #endregion
    }
}