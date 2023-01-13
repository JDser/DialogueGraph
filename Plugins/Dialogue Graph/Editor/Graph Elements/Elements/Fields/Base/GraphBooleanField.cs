//using System;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Fields
//{
//    [PropertyField("Boolean", typeof(bool), 0.74f, 0.48f, 0.8f)]
//    public class GraphBooleanField : GraphAbstractField
//    {
//        public override string DefaultTitle => "New Boolean";

//        public override string TypeTitle => "Boolean";

//        public override string CurrentValue => m_CurrentValueField.value.ToString();

//        public override Type Type => typeof(bool);

//        public override Func<NodePort, bool> PropertyCompatible => other =>
//        {
//            if (other.portType == typeof(object)) return true;
//            else if (other.portType == typeof(bool)) return true;
//            return false;
//        };

//        private Toggle m_DefaultValueField;
//        private Toggle m_CurrentValueField;

//        public GraphBooleanField()
//        {
//            m_InspectorField = new InspectorField($"Property : {text}");

//            m_DefaultValueField = new Toggle("Default Value : ");
//            m_CurrentValueField = new Toggle("Current Value : ");

//            m_InspectorField.AddField(m_DefaultValueField);
//            m_InspectorField.AddField(m_CurrentValueField);
//        }

//        public override VisualElement GetValueField()
//        {
//            return new Toggle();
//        }

//        public override void SetFieldData(VisualElement e, string data)
//        {
//            bool.TryParse(data, out bool result);
//            (e as Toggle).value = result;
//        }

//        public override string GetFieldData(VisualElement e)
//        {
//            return (e as Toggle).value.ToString();
//        }

//        public override void SetValue(GraphInlinePropertyNode propertyNode)
//        {
//            bool value = propertyNode.GetValueField<Toggle>().value;
//            m_CurrentValueField.value = value;
//        }
//    }
//}