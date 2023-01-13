//using System;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Nodes
//{
//    using Properties;

//    public class GraphInlinePropertyNode : GraphAbstractNode, IValuePropagation
//    {
//        /* Serialized */
//        [SerializeField] private string m_PropertyFieldType;

//        [SerializeField] private string m_SerializedValue;

//        /* Non-Serialized */
//        private VisualElement m_ValueField;
//        private GraphAbstractField m_GraphProperty;

//        /* Properties */
//        public override string DefaultTitle => "Inline Property";

//        protected override string Category => "Property";

//        public string SerializedValue { get => m_SerializedValue; set => m_SerializedValue = value; }

//        public Type PropertyType { get; set; }

//        public Type PropertyFieldType { get; private set; }

//        public GraphAbstractField GraphProperty { get => m_GraphProperty; set { } }

//        /* Initialization */
//        protected override void OnValidate()
//        {
//            if (PropertyFieldType == null) return;

//            m_GraphProperty = DialogueEditorUtility.Create<GraphAbstractField>(PropertyFieldType);

//            PropertyType = m_GraphProperty.Type;

//            m_ValueField = m_GraphProperty.GetValueField();
//            m_GraphProperty.SetFieldData(m_ValueField, m_SerializedValue);

//            extensionContainer.Add(m_ValueField);

//            title = m_GraphProperty.TypeTitle;

//            NodePort outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, PropertyType);
//            outputPort.portName = "Output";
//            outputPort.displayName = "Out";

//            outputPort.portColor = ColorSpaceManager.GetPropertyColor(PropertyType);

//            outputPort.IsCompatible = m_GraphProperty.PropertyCompatible;

//            outputContainer.Add(outputPort);
//            m_ports.Add(outputPort.portName, outputPort);
//        }

//        /* Value Compile */
//        public override void Compile()
//        {

//        }

//        /* Callbacks */
//        public override void OnBeforeSerialize()
//        {
//            base.OnBeforeSerialize();

//            if (PropertyFieldType == null) return;

//            m_SerializedValue = m_GraphProperty.GetFieldData(m_ValueField);
//            m_PropertyFieldType = PropertyFieldType.ToString();
//        }

//        public override void OnAfterDeserialize()
//        {
//            base.OnAfterDeserialize();

//            PropertyFieldType = Type.GetType(m_PropertyFieldType);
//        }

//        /* Local Methods */
//        public void Setup(Type propertyFieldType)
//        {
//            PropertyFieldType = propertyFieldType;
//        }

//        public T GetValueField<T>() where T : VisualElement
//        {
//            return m_ValueField as T;
//        }
//    }
//}