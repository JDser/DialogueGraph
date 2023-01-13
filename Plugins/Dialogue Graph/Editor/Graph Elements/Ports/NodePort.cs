using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public partial class NodePort : Port
    {
        protected readonly List<NodePort> m_ConnectedPorts;

        protected string m_PortName;

        public new string portName { get => m_PortName; set { m_PortName = value; displayName = value; } }

        public string displayName { get => m_ConnectorText.text; set => m_ConnectorText.text = value; }

        public bool IsInput => direction == Direction.Input;

        public bool IsOutput => direction == Direction.Output;

        public override bool connected => m_ConnectedPorts.Count > 0;

        public List<NodePort> ConnectedPorts => m_ConnectedPorts;

        public bool IsDynamic { get; private set; }

        //public VirtualConnection VirtualConnection { get; private set; }
        public VirtualConnectionBase VirtualConnection { get; private set; }

        public IConnectable Owner { get; private set; }

        public Func<NodePort, bool> IsCompatible { get; set; }

        protected NodePort(string name, Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
                      base(portOrientation, portDirection, portCapacity, type)
        {
            portName = name;

            m_ConnectedPorts = new List<NodePort>();

            IsDynamic = type == typeof(object);

            if (portDirection == Direction.Input)
                IsCompatible = other => other.IsCompatible(this);
            else
                IsCompatible = other => false;
        }

        //public void AddVirtualConnection(VisualElement inputField, Action<VisualElement, string> setData, Func<VisualElement, string> getData)
        //{
        //    RemoveVirtualConnection();
        //
        //    VirtualConnection = new VirtualConnection(portColor, inputField, setData, getData);
        //    parent.Insert(0, VirtualConnection);
        //
        //    VirtualConnection.visible = !connected;
        //
        //    VirtualConnection.style.top = parent.style.paddingTop;
        //}

        public void AddVirtualConnection(VirtualConnectionBase virtualConnection)
        {
            RemoveVirtualConnection();

            VirtualConnection = virtualConnection;
            parent.Insert(0, VirtualConnection);

            VirtualConnection.visible = !connected;

            VirtualConnection.style.top = parent.style.paddingTop;
        }

        public void RemoveVirtualConnection()
        {
            if (VirtualConnection != null)
            {
                parent.Remove(VirtualConnection);
                VirtualConnection = null;
            }
        }

        public void AddConnection(NodePort other)
        {
            m_ConnectedPorts.Add(other);

            if (VirtualConnection != null)
                VirtualConnection.visible = false;
        }

        public void RemoveConnection(NodePort other)
        {
            m_ConnectedPorts.Remove(other);

            if (VirtualConnection != null && !connected)
                VirtualConnection.visible = true;
        }

        public void ClearConnections()
        {
            if (connected)
                m_GraphView.DeleteElements(connections);
        }

        public GraphConnection GetGraphConnections()
        {
            GraphPortData[] portData = new GraphPortData[m_ConnectedPorts.Count];
            for (int i = 0; i < m_ConnectedPorts.Count; i++)
            {
                portData[i] = new GraphPortData(m_ConnectedPorts[i].portName, m_ConnectedPorts[i].Owner.Guid);
            }
            return new GraphConnection(portName, portData);
        }

        public static NodePort Create(IConnectable owner, string id, Orientation orientation, Direction direction, Capacity capacity, Type type)
        {
            NodePort port = new NodePort(id, orientation, direction, capacity, type);
            port.m_EdgeConnector = new EdgeConnector<Edge>(new NodeEdgeListener());
            port.AddManipulator(port.m_EdgeConnector);
            port.Owner = owner;
            return port;
        }

        public static NodePort Create(IConnectable owner, string id, Orientation orientation, Direction direction, Capacity capacity, Type type, IEdgeConnectorListener customEdgeListener)
        {
            NodePort port = new NodePort(id, orientation, direction, capacity, type);
            port.m_EdgeConnector = new EdgeConnector<Edge>(customEdgeListener);
            port.AddManipulator(port.m_EdgeConnector);
            port.Owner = owner;
            return port;
        }

        public bool IsCompatibleWith(NodePort other)
        {
            if (this.direction == other.direction) return false;
            else if (this.node == other.node) return false;
            else if (this == other) return false;
            else if (this.portType == other.portType) return true;
            else return this.IsCompatible(other);
        }
    }
}