using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DialogueEditor.Nodes
{
    using Node = UnityEditor.Experimental.GraphView.Node;

    /* Usefull links */
    // https://stackoverflow.com/questions/353126/c-sharp-multiple-generic-types-in-one-list
    // https://stackoverflow.com/questions/899629/cast-object-to-t

    public abstract class GraphAbstractNode : Node, IGraphElement, IGraphEvents, IGroupable, IConnectable, IColorable, ISerializationCallbackReceiver
    {
        /* Serialized  */

        [SerializeField] protected string m_guidSerialized;

        [SerializeField] protected bool m_expanded = true;

        [SerializeField] protected Vector2 m_position;

        [SerializeField] protected GraphConnection[] m_graphConnections;

        [SerializeField] protected Color m_userDefinedColor;

        /* Non-Serialized */

        protected readonly Dictionary<string, NodePort> m_ports;

        //protected InspectorField m_InspectorField;

        private VisualElement m_CollapseIcon;

        /* Properties */

        public virtual string Title => "Abstract Node";

        public virtual string Category => "Dialogue";

        public Guid Guid { get; set; }

        public Vector2 Position
        {
            get => m_position;
            set
            {
                SetPosition(new Rect(value, Vector2.zero));
                m_position = value;
            }
        }

        public DialogueGraphView GraphView { get; set; }

        public GraphGroup Group { get; set; }

        public GraphConnection[] GraphConnections
        {
            get
            {
                List<GraphConnection> connections = new List<GraphConnection>();
                foreach (NodePort port in Ports)
                {
                    if (port.connected)
                        connections.Add(port.GetGraphConnections());
                }
                return connections.ToArray();
            }
            set => m_graphConnections = value;
        }

        public virtual IEnumerable<NodePort> Ports
        {
            get
            {
                foreach (NodePort port in m_ports.Values)
                {
                    yield return port;
                }
            }
        }

        public IEnumerable<NodePort> Outputs
        {
            get
            {
                foreach (NodePort port in m_ports.Values)
                {
                    if (port.IsOutput)
                        yield return port;
                }
            }
        }

        public IEnumerable<NodePort> Inputs
        {
            get
            {
                foreach (NodePort port in m_ports.Values)
                {
                    if (port.IsInput)
                        yield return port;
                }
            }
        }

        public Color UserDefinedColor => m_userDefinedColor;

        /* Initialization */

        public GraphAbstractNode(Dictionary<string, NodePort> ports)
        {
            m_ports = ports;
        }

        public GraphAbstractNode()
        {
            m_ports = new Dictionary<string, NodePort>();

            m_HighlightColor = ColorSpaceManager.GetCategoryColor(Category);

            this.AddStyleSheets(DialogueResourceManager.AbstractNodeStyleSheets);
            this.AddStyleSheets(DialogueResourceManager.PortStyleSheets);

            titleContainer.name = "nodeTitle";
            Label titleLabel = titleContainer.Q("title-label") as Label;
            titleLabel.name = null;
            titleLabel.ReplaceStyle("unity-text-element", "graphNode__label");

            m_CollapseIcon = this.Q("icon");
            m_CollapseIcon.style.backgroundImage = DialogueResourceManager.GetArrow();

            extensionContainer.name = "graphNodeContent";
            this.Q("node-border").style.overflow = Overflow.Visible;

            title = Title;

            CreatePorts();

            RefreshExpandedState();
        }

        protected virtual void CreatePorts()
        {
            IEnumerable<PortAttribute> ports = GetType().GetCustomAttributes<PortAttribute>();

            foreach (PortAttribute portAttribute in ports)
            {
                if (m_ports.ContainsKey(portAttribute.PortID))
                {
                    Debug.LogError($"Port with {portAttribute.PortID} has already been added.");
                    continue;
                }

                Port.Capacity capacity = portAttribute.PortSingleCapacity ? Port.Capacity.Single : Port.Capacity.Multi;
                Direction direction = portAttribute.IsInput ? Direction.Input : Direction.Output;

                NodePort port = NodePort.Create(this, portAttribute.PortID, Orientation.Horizontal, direction, capacity, portAttribute.Type);
                port.displayName = !string.IsNullOrEmpty(portAttribute.Name) ? portAttribute.Name : portAttribute.PortID;
                port.portColor = ColorSpaceManager.GetPropertyColor(portAttribute.Type);

                VisualElement portContainer = new VisualElement();
                portContainer.AddToClassList("port-container");
                portContainer.Add(port);
                port.Q<Label>().AddToClassList("port-label");
                m_ports.Add(port.portName, port);

                if (portAttribute.IsInput) inputContainer.Add(portContainer);
                else outputContainer.Add(portContainer);
            }
        }

        public virtual void Validate()
        {
            GraphView.OnHighlight += ChangeColor;
            ChangeColor(GraphView.CurrentColorMode);

            if (!m_graphConnections.IsNullOrEmpty())
            {
                for (int i = 0; i < m_graphConnections.Length; i++)
                {
                    GraphConnection connection = m_graphConnections[i];

                    NodePort output = GetPort(connection.PortName);
                    if (output == null || output.IsInput) continue;

                    for (int x = 0; x < connection.Ports.Length; x++)
                    {
                        Guid otherGuid = connection.Ports[x].NodeGuid;
                        string otherPort = connection.Ports[x].PortName;

                        if (GraphView.graphElements.TryGet(otherGuid, out IConnectable other))
                            GraphView.Connect(output, other.GetPort(otherPort));
                    }
                }
            }

            if (!m_expanded)
                ToggleCollapse();
        }

        public virtual void Remove() { }

        public virtual void RemapConnections(Dictionary<Guid, Guid> remappedGuids)
        {
            List<GraphConnection> remappedConnections = new List<GraphConnection>();

            for (int i = 0; i < m_graphConnections.Length; i++)
            {
                GraphConnection connection = m_graphConnections[i];

                NodePort port = GetPort(connection.PortName);

                if (port != null && port.IsInput)
                    continue;

                List<GraphPortData> portData = new List<GraphPortData>();

                for (int x = 0; x < connection.Ports.Length; x++)
                {
                    Guid otherGuid = connection.Ports[x].NodeGuid;
                    string otherPort = connection.Ports[x].PortName;

                    Guid remappedGuid = Guid.Empty;
                    NodePort input = null;

                    if (remappedGuids.TryGetValue(otherGuid, out Guid remapped)) //Other node get copyPasted too
                    {
                        input = GraphView.graphElements.Get<IConnectable>(remapped).GetPort(otherPort);
                        remappedGuid = remapped;
                    }
                    else if (GraphView.graphElements.TryGet(otherGuid, out IConnectable other))
                    {
                        input = other.GetPort(otherPort);
                        remappedGuid = otherGuid;
                    }

                    if (remappedGuid == Guid.Empty || (input.connected && input.capacity == Port.Capacity.Single))
                        continue;

                    portData.Add(new GraphPortData(connection.Ports[x].PortName, remappedGuid));
                }

                remappedConnections.Add(new GraphConnection(connection.PortName, portData.ToArray()));
            }

            m_graphConnections = remappedConnections.ToArray();
        }

        /* Ports */

        public NodePort GetPort(string portName)
        {
            if (string.IsNullOrEmpty(portName))
                return null;

            if (m_ports.TryGetValue(portName, out NodePort port))
                return port;

            return null;
        }

        public NodePort GetCompatiblePort(NodePort other)
        {
            foreach (NodePort port in Ports)
            {
                if (port.direction == other.direction)
                    continue;
                else if (port.portType == other.portType)
                    return port;
                else if (port.IsCompatible(other))
                    return port;
            }

            return null;
        }

        //public virtual void OnPortConnect(NodePort output, NodePort input) { }

        //public virtual void OnPortDisconnect(NodePort output, NodePort input) { }

        /* Callbacks */

        #region Contextual Menu

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", DisconnectInputPortsAction, DisconnectInputsStatus);

            evt.menu.AppendAction("Disconnect Output Ports", DisconnectOutputPortsAction, DisconnectOutputsStatus);

            evt.menu.AppendAction("Disconnect All", DisconnectAllPortsAction, DisconnectAllStatus);

            evt.menu.AppendSeparator();
        }

        protected DropdownMenuAction.Status DisconnectAllStatus(DropdownMenuAction a)
        {
            foreach (NodePort item in Ports)
            {
                if (item.connected)
                {
                    return DropdownMenuAction.Status.Normal;
                }
            }

            return DropdownMenuAction.Status.Disabled;
        }

        protected void DisconnectAllPortsAction(DropdownMenuAction a)
        {
            DisconnectInputPortsAction(a);
            DisconnectOutputPortsAction(a);
        }

        protected DropdownMenuAction.Status DisconnectInputsStatus(DropdownMenuAction a)
        {
            foreach (NodePort item in Inputs)
            {
                if (item.connected)
                {
                    return DropdownMenuAction.Status.Normal;
                }
            }

            return DropdownMenuAction.Status.Disabled;
        }

        protected void DisconnectInputPortsAction(DropdownMenuAction a)
        {
            foreach (NodePort item in Inputs)
            {
                item.ClearConnections();
            }
        }

        protected DropdownMenuAction.Status DisconnectOutputsStatus(DropdownMenuAction a)
        {
            foreach (NodePort item in Outputs)
            {
                if (item.connected)
                {
                    return DropdownMenuAction.Status.Normal;
                }
            }

            return DropdownMenuAction.Status.Disabled;
        }

        protected void DisconnectOutputPortsAction(DropdownMenuAction a)
        {
            foreach (NodePort item in Outputs)
            {
                item.ClearConnections();
            }
        }

        #endregion

        #region Color Managment

        private Color m_HighlightColor;

        public void ChangeColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.None:
                    titleContainer.style.borderBottomColor = Color.clear;
                    return;

                case ColorMode.Category:
                    titleContainer.style.borderBottomColor = m_HighlightColor;
                    return;

                case ColorMode.UserDefined:
                    titleContainer.style.borderBottomColor = m_userDefinedColor;
                    return;
            }
        }

        public void SetUserDefinedColor(Color newColor)
        {
            m_userDefinedColor = newColor;
            titleContainer.style.borderBottomColor = newColor;
        }

        public void ResetUserDefinedColor()
        {
            m_userDefinedColor = Color.clear;
            titleContainer.style.borderBottomColor = Color.clear;
        }

        #endregion

        protected override void ToggleCollapse()
        {
            expanded = !expanded;

            foreach (var port in Inputs)
            {
                if (!port.connected && port.VirtualConnection != null)
                    port.VirtualConnection.visible = expanded;
            }

            m_CollapseIcon.style.backgroundImage = expanded ?
                DialogueResourceManager.GetArrow() :
                DialogueResourceManager.GetArrowLeft();

            inputContainer.style.paddingTop = inputContainer.style.paddingBottom =
            outputContainer.style.paddingTop = outputContainer.style.paddingBottom = expanded ? 4 : 0;
        }

        public override void OnSelected()
        {
            BringToFront();

            //if (m_InspectorField != null)
            //    GraphView.Inspector.AddToContent(m_InspectorField);
        }

        public override void OnUnselected()
        {
            //if (m_InspectorField != null)
            //    GraphView.Inspector.RemoveFromContent(m_InspectorField);
        }

        public virtual void OnBeforeSerialize()
        {
            m_guidSerialized = Guid.ToString();
            m_position = GetPosition().position;
            m_expanded = expanded;
            m_graphConnections = GraphConnections;
        }

        public virtual void OnAfterDeserialize()
        {
            Guid = Guid.Parse(m_guidSerialized);
            Position = m_position;
        }
    }
}