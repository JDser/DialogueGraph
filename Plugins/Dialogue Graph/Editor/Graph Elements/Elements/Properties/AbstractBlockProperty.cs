using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DialogueEditor.Properties
{
    public abstract class AbstractBlockProperty : GraphElement, IGraphElement, IGraphEvents, ICompilable,IConnectable, ISerializationCallbackReceiver
    {
        /* Serialized */

        [SerializeField] private string m_guidSerialized;

        [SerializeField] private bool m_expanded = true;

        [SerializeField] private string m_blockNodeGuid;

        [SerializeField] protected GraphConnection[] m_graphConnections;

        /* Non-Serialized */

        protected readonly Dictionary<string, NodePort> m_ports;

        private VisualElement m_CollapseButtonIcon;

        /* Properties */

        public virtual string Title => "Abstract";

        public Guid Guid { get; set; }

        public Vector2 Position { get; set; }

        public DialogueGraphView GraphView { get; set; }

        public Guid BlockNodeGuid { get; set; }

        public virtual bool expanded
        {
            get => m_expanded;
            set
            {
                if (m_expanded == value)
                    return;

                m_expanded = value;
                RefreshExpandedState();
            }
        }

        public new VisualElement contentContainer { get; private set; }

        public VisualElement portContainer { get; private set; }

        public VisualElement inputContainer { get; private set; }

        public VisualElement outputContainer { get; private set; }

        protected UnityEngine.Object ObjectToSelect { get; set; }

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

        /* Initialization */

        public AbstractBlockProperty()
        {
            m_ports = new Dictionary<string, NodePort>();

            ClearClassList();
            AddToClassList("blockProperty");

            capabilities = Capabilities.Copiable | Capabilities.Deletable | Capabilities.Droppable | Capabilities.Selectable;

            this.LoadUXMLTree(DialogueResourceManager.BlockPropertyUXML);
            this.AddStyleSheets(DialogueResourceManager.BlockPropertyStyleSheets);
            this.AddManipulator(new SelectionDropper());

            contentContainer = this.Q("contents");
            portContainer = this.Q("ports");
            inputContainer = portContainer.Q("inputContainer");
            outputContainer = portContainer.Q("outputContainer");

            this.Q<Label>("title-label").text = Title;
            this.Q<Button>("collapse-button").clicked += ToggleCollapse;

            m_CollapseButtonIcon = this.Q("collapse-icon");
            m_CollapseButtonIcon.style.backgroundImage = DialogueResourceManager.GetArrow();

            CreatePorts();
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
            if (GraphView.graphElements.TryGet(BlockNodeGuid, out Nodes.GraphBlockNode blockNode))
            {
                blockNode.contentContainer.Add(this);
            }
            else
            {
                Debug.LogError("Could not find parent blockNode!");
                GraphView.graphElements.Remove(Guid);
                this.Delete();
                return;
            }

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

            RefreshExpandedState();
        }

        public virtual void Remove() { }

        public virtual void RemapConnections(Dictionary<Guid, Guid> remappedGuids)
        {
            if (remappedGuids.TryGetValue(BlockNodeGuid, out Guid newBlockNode))
            {
                BlockNodeGuid = newBlockNode;
            }
            else if (GraphView.selection.Find(e => e is Nodes.GraphBlockNode) is Nodes.GraphBlockNode blockNode)
            {
                BlockNodeGuid = blockNode.Guid;
            }

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

        public bool RefreshPorts()
        {
            if (inputContainer.childCount == 0 && outputContainer.childCount == 0)
            {
                VisualElement lastDivider = this.Query("dividerHorizontal").Last();
                portContainer.style.display = lastDivider.style.display = DisplayStyle.None;
                return false;
            }

            int inputCount = 0;
            int outputCount = 0;

            foreach (var port in Ports)
            {
                if (m_expanded || port.connected)
                {
                    port.style.display = DisplayStyle.Flex;

                    if (port.IsInput) inputCount++;
                    else outputCount++;
                }
                else
                {
                    port.style.display = DisplayStyle.None;
                }
            }


            if (inputCount == 0)
            {
                inputContainer.style.display = DisplayStyle.None;
                outputContainer.AddToClassList("rounded");
                outputContainer.style.borderBottomLeftRadius = 7;
            }
            else
            {
                inputContainer.style.display = DisplayStyle.Flex;
                outputContainer.RemoveFromClassList("rounded");
                outputContainer.style.borderBottomLeftRadius = 0;
            }

            if (outputCount == 0)
            {
                outputContainer.style.display = DisplayStyle.None;
            }
            else
            {
                outputContainer.style.display = DisplayStyle.Flex;
            }

            portContainer.Q("dividerVertical").style.display = (inputCount == 0 || outputCount == 0) ? DisplayStyle.None : DisplayStyle.Flex;

            return inputCount > 0 || outputCount > 0;
        }


        /* Misc */

        public void ToggleCollapse()
        {
            expanded = !expanded;
        }

        public void RefreshExpandedState()
        {
            DisplayStyle status;

            if (m_expanded)
            {
                m_CollapseButtonIcon.style.backgroundImage = DialogueResourceManager.GetArrow();
                status = DisplayStyle.Flex;
            }
            else
            {
                m_CollapseButtonIcon.style.backgroundImage = DialogueResourceManager.GetArrowLeft();
                status = DisplayStyle.None;
            }

            VisualElement firstDivider = this.Query("dividerHorizontal").First();
            contentContainer.style.display = firstDivider.style.display = contentContainer.childCount > 0 ? status : DisplayStyle.None;

            RefreshPorts();
        }

        public override void OnSelected()
        {
            if (ObjectToSelect != null)
                UnityEditor.Selection.activeObject = ObjectToSelect;
            else
                UnityEditor.Selection.activeObject = null;
        }

        public virtual void OnBeforeSerialize()
        {
            m_guidSerialized = Guid.ToString();
            m_blockNodeGuid = GetFirstAncestorOfType<Nodes.GraphBlockNode>().Guid.ToString();
            m_graphConnections = GraphConnections;
        }

        public virtual void OnAfterDeserialize()
        {
            Guid = Guid.Parse(m_guidSerialized);
            BlockNodeGuid = Guid.Parse(m_blockNodeGuid);
        }

        public abstract UnityEngine.Object GetObject();

        public abstract void Compile();
    }
}