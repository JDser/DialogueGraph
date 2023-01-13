//using System;

//namespace DialogueEditor
//{
//    using Nodes;

//    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
//    public class PropertyFieldAttribute : Attribute
//    {
//        private readonly string m_Name;

//        private readonly Type m_PropertyType;

//        private readonly float r, g, b;

//        public string Name => m_Name;

//        public Type PropertyType => m_PropertyType;

//        public UnityEngine.Color Color => new UnityEngine.Color(r, g, b);

//        public Type GraphNodeType { get; set; }

//        public PropertyFieldAttribute(string name)
//        {
//            m_Name = name;
//            GraphNodeType = typeof(GraphPropertyNode);
//            m_PropertyType = null;
//        }

//        public PropertyFieldAttribute(string name, Type type, float r, float g, float b)
//        {
//            m_Name = name;
//            GraphNodeType = typeof(GraphPropertyNode);
//            m_PropertyType = type;

//            this.r = r;
//            this.g = g;
//            this.b = b;
//        }
//    }
//}