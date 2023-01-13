using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Nodes
{
    using DialogueEditor.Properties;
    using DialogueSystem;

    [SearchEntry("Block Node", order = 0)]
    public partial class GraphBlockNode : GraphAbstractNode, IHasObjectsToAdd, ICompilable
    {
        /* Non-Serialized */

        protected NodeHeader m_Header;

        /* Properties */

        public override string Title => "Block Node";

        public BlockNodeBorderRenderer BorderRenderer { get; private set; }

        public new VisualElement inputContainer { get; private set; }

        public new VisualElement outputContainer { get; private set; }

        public new VisualElement extensionContainer { get; private set; }

        public new VisualElement contentContainer { get; private set; }

        public virtual Color StartColor => new Color(0.13f, 0.52f, 0.32f, 1f);

        public virtual Color EndColor => new Color(0.13f, 0.52f, 0.32f, 1f);

        protected DragReorderHelper DragReorderHelper { get; set; }

        /* Initialization */

        public GraphBlockNode() : base(new Dictionary<string, NodePort>())
        {
            Clear();

            this.LoadUXMLTree(DialogueResourceManager.BlockNodeUXML);
            this.AddStyleSheets(DialogueResourceManager.PortStyleSheets);

            BorderRenderer = this.Q<BlockNodeBorderRenderer>("node-border");
            BorderRenderer.StartColor = StartColor;
            BorderRenderer.EndColor = EndColor;

            inputContainer = this.Q("input");
            outputContainer = this.Q("output");
            extensionContainer = this.Q("extensions");
            contentContainer = this.Q("contentContainer");

            DragReorderHelper = new DragReorderHelper(this, contentContainer.parent, contentContainer);
            DragReorderHelper.IndicatorWidth = 2;
            DragReorderHelper.IndicatorColor = new Color(1f, 0.62f, 0.17f);
            DragReorderHelper.AcceptedObjects = e =>
            {
                return e is AbstractBlockProperty;
            };

            CreatePorts();
        }

        protected override void CreatePorts()
        {
            VisualElement textField;
        
            NodePort InputFlow = NodePort.Create(this,"Input", Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(GraphBlockNode), new FlowEdgeListener());
            InputFlow.portColor = StartColor;
            InputFlow.displayName = null;
            inputContainer.Add(InputFlow);
        
            /* "Disabling" port's text field */
            textField = InputFlow.Q("type");
            textField.style.height = 0;
            textField.style.marginLeft = 0;
            textField.style.marginRight = 0;
        
            m_ports.Add(InputFlow.portName, InputFlow);
        
        
            NodePort OutputFlow = NodePort.Create(this, "Output", Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GraphBlockNode), new FlowEdgeListener());
            OutputFlow.portColor = EndColor;
            OutputFlow.displayName = null;
            outputContainer.Add(OutputFlow);
        
            /* "Disabling" port's text field */
            textField = OutputFlow.Q("type");
            textField.style.height = 0;
            textField.style.marginLeft = 0;
            textField.style.marginRight = 0;
        
            m_ports.Add(OutputFlow.portName, OutputFlow);
        }

        public override void Validate()
        {
            DragReorderHelper.GraphView = GraphView;

            m_Header = new NodeHeader(this, m_Title, m_Line, m_IsChoice);
            extensionContainer.Insert(0, m_Header);

            ValidateConnections();
        }

        public override void Remove()
        {
            foreach (var item in contentContainer.Query<AbstractBlockProperty>().ToList())
            {
                GraphView.graphElements.Remove(item.Guid);

                foreach (var port in item.Ports)
                {
                    foreach (var edge in port.connections)
                    {
                        if (port.IsInput)
                        {
                            (edge.output as NodePort).RemoveConnection(port);
                            (edge.output as NodePort).Disconnect(edge);
                        }
                        else
                        {
                            (edge.input as NodePort).RemoveConnection(port);
                            (edge.input as NodePort).Disconnect(edge);
                        }

                        GraphView.RemoveElement(edge);
                    }
                }

                item.Delete();
            }
        }

        public override void RemapConnections(Dictionary<Guid, Guid> remappedGuids)
        {
            m_SerializedReference = null;

            if (!m_ConnectionProrities.IsNullOrEmpty())
            {
                for (int i = 0; i < m_ConnectionProrities.Length; i++)
                {
                    if (remappedGuids.TryGetValue(m_ConnectionProrities[i].Guid, out Guid remappedGuid))
                    {
                        m_ConnectionProrities[i] = new GraphConnectionPriority(remappedGuid, m_ConnectionProrities[i].Priority);
                    }
                }
            }

            base.RemapConnections(remappedGuids);
        }

        protected void ValidateConnections()
        {
            if (m_graphConnections.IsNullOrEmpty())
                return;

            for (int i = 0; i < m_graphConnections.Length; i++)
            {
                GraphConnection connection = m_graphConnections[i];

                NodePort output = GetPort(connection.PortName);
                if (output == null || output.IsInput) continue;

                for (int x = 0; x < connection.Ports.Length; x++)
                {
                    Guid otherGuid = connection.Ports[x].NodeGuid;
                    string otherPort = connection.Ports[x].PortName;

                    if (GraphView.graphElements.TryGet(otherGuid, out GraphBlockNode other))
                    {
                        Edge edge = GraphView.Connect(output, other.GetPort(otherPort));

                        if (m_ConnectionProrities.IsNullOrEmpty())
                            continue;

                        for (int y = 0; y < m_ConnectionProrities.Length; y++)
                        {
                            if (other.Guid == m_ConnectionProrities[y].Guid)
                            {
                                edge.SetPriority(m_ConnectionProrities[y].Priority);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /* Callbacks */

        public void ToggleChoice()
        {
            if (m_Header.ChoiceToggle != null)
            {
                m_Header.ChoiceToggle.value = !m_Header.ChoiceToggle.value;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();

            if (m_SerializedReference != null)
                UnityEditor.Selection.activeObject = m_SerializedReference.Reference;
            else
                UnityEditor.Selection.activeObject = null;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();

            m_Title = m_Header.Title;
            m_Line = m_Header.Line;
            m_IsChoice = m_Header.IsChoice;

            #region Collect Connections
            NodePort port = Outputs.First();
            if (port != null && port.connected)
            {
                m_ConnectionProrities = new GraphConnectionPriority[port.ConnectedPorts.Count];

                int currentIndex = 0;

                foreach (Edge edge in port.connections)
                {
                    ConnectionPriority priority = edge.userData != null ? (ConnectionPriority)edge.userData : ConnectionPriority.Medium;
                    m_ConnectionProrities[currentIndex] = new GraphConnectionPriority((edge.input.node as GraphBlockNode).Guid, priority);

                    currentIndex++;
                }
            }
            else
            {
                m_ConnectionProrities = null;
            }
            #endregion
        }

        #region Node Compilation
        [SerializeField] private ObjectReference<Node> m_SerializedReference;

        [SerializeField] private string m_Title = "New Node";

        [SerializeField] private string m_Line = "New Line";

        [SerializeField] private bool m_IsChoice = false;

        [SerializeField] private GraphConnectionPriority[] m_ConnectionProrities = null;

        public IEnumerable<UnityEngine.Object> GetObjects()
        {
            if (m_SerializedReference == null)
            {
                m_SerializedReference = new ObjectReference<Node>();
            }

            yield return m_SerializedReference.Reference;

            List<AbstractBlockProperty> properties = contentContainer.Query<AbstractBlockProperty>().ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                UnityEngine.Object toAdd = properties[i].GetObject();
                if (toAdd != null)
                {
                    yield return toAdd;
                }
            }
        }

        public virtual void Compile()
        {
            List<Property> properties = new List<Property>();
            List<AbstractBlockProperty> abstractProperties = contentContainer.Query<AbstractBlockProperty>().ToList();
            for (int i = 0; i < abstractProperties.Count; i++)
            {
                Property toAdd = abstractProperties[i].GetObject() as Property;
                if (toAdd != null)
                {
                    properties.Add(toAdd);
                }
            }

            Connection[] connections = null;

            NodePort port = Outputs.First();
            if (port != null && port.connected)
            {
                connections = new Connection[port.ConnectedPorts.Count];

                int currentIndex = 0;

                foreach (Edge edge in port.connections)
                {
                    ConnectionPriority priority = edge.userData != null ? (ConnectionPriority)edge.userData : ConnectionPriority.Medium;
                    connections[currentIndex] = new Connection((edge.input.node as GraphBlockNode).m_SerializedReference.Reference, priority);

                    currentIndex++;
                }

                Array.Sort(connections);
            }

            m_SerializedReference.Reference.Set(m_Title, m_Line, m_IsChoice, connections, properties.ToArray());
        }
        #endregion
    }
}