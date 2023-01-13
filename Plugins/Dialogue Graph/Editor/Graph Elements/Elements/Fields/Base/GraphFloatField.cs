//using System;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Fields
//{
//    [PropertyField("Float", typeof(float), 0.51f, 0.89f, 0.9f)]
//    public class GraphFloatField : GraphAbstractField
//    {
//        public override string DefaultTitle => "New Float";

//        public override string TypeTitle => "Float";

//        public override string CurrentValue => m_CurrentValueField.value.ToString();

//        public override Type Type => typeof(float);

//        public override Func<NodePort, bool> PropertyCompatible => other =>
//        {
//            if (other.portType == typeof(object)) return true;
//            else if (other.portType == typeof(int)) return true;
//            return false;
//        };

//        private FloatField m_DefaultValueField;
//        private FloatField m_CurrentValueField;

//        public GraphFloatField()
//        {
//            m_InspectorField = new InspectorField($"Property : {text}");

//            m_DefaultValueField = new FloatField("Default Value : ");
//            m_CurrentValueField = new FloatField("Current Value : ");

//            m_InspectorField.AddField(m_DefaultValueField);
//            m_InspectorField.AddField(m_CurrentValueField);
//        }

//        public override VisualElement GetValueField()
//        {
//            FloatField floatField = new FloatField();
//            floatField.Q("unity-text-input").AddToClassList("virtualConnection__input-field");
//            return floatField;
//        }

//        public override void SetFieldData(VisualElement e, string data)
//        {
//            float.TryParse(data, out float result);
//            (e as FloatField).value = result;
//        }

//        public override string GetFieldData(VisualElement e)
//        {
//            return (e as FloatField).value.ToString();
//        }

//        public override void SetValue(GraphInlinePropertyNode propertyNode)
//        {
//            float value = propertyNode.GetValueField<FloatField>().value;
//            m_CurrentValueField.value = value;
//        }
//    }
//}