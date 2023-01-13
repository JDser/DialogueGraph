using System;
using UnityEngine;

namespace DialogueEditor
{
    [Serializable]
    public struct GraphConnection
    {
        /* Serializable */

        [SerializeField] private string m_PortName;

        [SerializeField] private GraphPortData[] m_ConnectedPorts;

        /* Properties */

        public string PortName => m_PortName;

        public GraphPortData[] Ports => m_ConnectedPorts;

        public GraphConnection(string portName, GraphPortData[] ports)
        {
            m_PortName = portName;
            m_ConnectedPorts = ports;
        }
    }
}