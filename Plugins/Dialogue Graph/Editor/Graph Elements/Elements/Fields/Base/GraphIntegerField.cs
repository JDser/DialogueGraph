//using System;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Fields
//{
//    [PropertyField("Integer", typeof(int), 0.6f, 0.93f, 0.57f)]
//    public class GraphIntegerField : GraphAbstractField
//    {
//        public override string DefaultTitle => "New Integer";

//        public override string TypeTitle => "Integer";

//        public override string CurrentValue => m_CurrentValueField.value.ToString();

//        public override Type Type => typeof(int);

//        public override Func<NodePort, bool> PropertyCompatible => other =>
//        {
//            if (other.portType == typeof(object)) return true;
//            else if (other.portType == typeof(float)) return true;
//            return false;
//        };

//        private IntegerField m_DefaultValueField;
//        private IntegerField m_CurrentValueField;

//        public GraphIntegerField()
//        {
//            m_InspectorField = new InspectorField($"Property : {text}");

//            m_DefaultValueField = new IntegerField("Default Value : ");
//            m_CurrentValueField = new IntegerField("Current Value : ");

//            m_InspectorField.AddField(m_DefaultValueField);
//            m_InspectorField.AddField(m_CurrentValueField);
//        }

//        public override VisualElement GetValueField()
//        {
//            IntegerField integerField = new IntegerField();
//            integerField.Q("unity-text-input").AddToClassList("virtualConnection__input-field");
//            return integerField;
//        }

//        public override void SetFieldData(VisualElement e, string data)
//        {
//            int.TryParse(data, out int result);
//            (e as IntegerField).value = result;
//        }

//        public override string GetFieldData(VisualElement e)
//        {
//            return (e as IntegerField).value.ToString();
//        }

//        public override void SetValue(GraphInlinePropertyNode propertyNode)
//        {
//            int value = propertyNode.GetValueField<IntegerField>().value;
//            m_CurrentValueField.value = value;
//        }
//    }
//}