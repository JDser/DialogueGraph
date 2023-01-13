//using UnityEngine.UIElements;

//namespace DialogueEditor
//{
//    public class InspectorField : VisualElement
//    {
//        private readonly Label m_TitleLabel;
//        private readonly VisualElement m_ContentContainer;

//        public string Title { get => m_TitleLabel.text; set => m_TitleLabel.text = value; }

//        public InspectorField(string title = "Title : InspectorField")
//        {
//            this.LoadUXMLTree(DialogueResourceManager.InspectorFieldUXML);

//            name = "inspectorField";

//            m_TitleLabel = this.Q<Label>("inspectorTitle");
//            m_TitleLabel.text = title;

//            m_ContentContainer = this.Q("fieldContainer");
//        }

//        public void AddField(VisualElement element)
//        {
//            m_ContentContainer.Add(element);
//        }

//        public void RemoveField(VisualElement element)
//        {
//            if (m_ContentContainer.Contains(element))
//                m_ContentContainer.Remove(element);
//        }
//    }
//}