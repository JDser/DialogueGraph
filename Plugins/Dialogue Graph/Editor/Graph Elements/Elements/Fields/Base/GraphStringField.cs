//using System;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Fields
//{

//    [PropertyField("String", typeof(string), 0.96f, 1f, 0.6f)]
//    public class GraphStringField : GraphAbstractField
//    {
//        public override string DefaultTitle => "New String";

//        public override string TypeTitle => "String";

//        public override string CurrentValue => m_CurrentValueField.value.ToString();

//        public override Type Type => typeof(string);

//        public override Func<NodePort, bool> PropertyCompatible => other =>
//        {
//            if (other.portType == typeof(object)) return true;
//            else return false;
//        };

//        private TextField m_DefaultValueField;
//        private TextField m_CurrentValueField;

//        public GraphStringField()
//        {
//            m_InspectorField = new InspectorField($"Property : {text}");

//            m_DefaultValueField = new TextField("Default Value : ");
//            m_CurrentValueField = new TextField("Current Value : ");

//            m_InspectorField.AddField(m_DefaultValueField);
//            m_InspectorField.AddField(m_CurrentValueField);
//        }

//        public override void Compile()
//        {

//        }

//        public override VisualElement GetValueField()
//        {
//            TextField textField = new TextField();
//            textField.Q("unity-text-input").AddToClassList("virtualConnection__input-field");
//            return textField;
//        }

//        public override void SetFieldData(VisualElement e, string data)
//        {
//            (e as TextField).value = data;
//        }

//        public override string GetFieldData(VisualElement e)
//        {
//            return (e as TextField).value;
//        }

//        public override void SetValue(GraphInlinePropertyNode propertyNode)
//        {
//            string value = propertyNode.GetValueField<TextField>().value;
//            m_CurrentValueField.value = value;
//        }
//    }
//}