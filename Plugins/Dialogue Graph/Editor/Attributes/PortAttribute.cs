using System;

namespace DialogueEditor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PortAttribute : Attribute
    {
        protected readonly string m_PortID;
        protected readonly Type m_Type;
        protected readonly bool m_IsInput;

        public string PortID => m_PortID;

        public Type Type => m_Type;

        public bool IsInput => m_IsInput;

        public string Name { get; set; }

        public bool PortSingleCapacity { get; set; }

        public PortAttribute(string portId, Type type, bool isInput)
        {
            m_PortID = portId;
            m_Type = type;
            m_IsInput = isInput;

            PortSingleCapacity = true;
        }
    }

    public sealed class InputAttribute : PortAttribute
    {
        public InputAttribute(string portId, Type type) : base(portId, type, true)
        {
            PortSingleCapacity = false;
        }
    }

    public sealed class OutputAttribute : PortAttribute
    {
        public OutputAttribute(string portId, Type type) : base(portId, type, false)
        {
            PortSingleCapacity = true;
        }
    }
}
