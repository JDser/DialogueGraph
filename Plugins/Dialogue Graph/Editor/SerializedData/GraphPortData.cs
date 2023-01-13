using System;
using UnityEngine;

namespace DialogueEditor
{
    [Serializable]
    public struct GraphPortData
    {
        /* Serializable */

        [SerializeField] private string m_PortName;

        [SerializeField] private string m_NodeGuid;

        /* Properties */

        public string PortName => m_PortName;

        public Guid NodeGuid => Guid.Parse(m_NodeGuid);

        public GraphPortData(string portName, Guid guid)
        {
            m_PortName = portName;
            m_NodeGuid = guid.ToString();
        }
    }
}