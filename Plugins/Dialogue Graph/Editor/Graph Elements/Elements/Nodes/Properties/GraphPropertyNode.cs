//using System;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Nodes
//{
//    using Properties;

//    public class GraphPropertyNode : GraphAbstractNode, IValuePropagation
//    {
//        /* Serialized */
//        [SerializeField] private string m_fieldGuidSerialized;

//        /* Non-Serialized */
//        private NodePort m_OutputPort;
//        private Pill m_Pill;

//        /* Properties */
//        public override string DefaultTitle => "Property Node";

//        protected override string Category => "Property";

//        public override string title { set { if (m_OutputPort == null) return; m_OutputPort.displayName = value; } }

//        public override IEnumerable<NodePort> Ports { get { yield return m_OutputPort; } }

//        public GraphAbstractField AssociatedField { get; protected set; }

//        public GraphAbstractField GraphProperty { get => AssociatedField; set { } }

//        /* Initialization */
//        public GraphPropertyNode() : base(null)
//        {
//            Clear();
//            ClearClassList();

//            m_Pill = new Pill();
//            m_Pill.Q("output").name = "pillOutput";

//            Add(m_Pill);
//        }

//        public override void Validate() { }

//        public override void Remove()
//        {
//            if (AssociatedField != null)
//                AssociatedField.RemoveNode(this);
//        }

//        public override void OnPaste(Dictionary<Guid, Guid> remappedGuids)
//        {
//            base.OnPaste(remappedGuids);

//            /* If copyPasted only propertyNode */
//            if (Guid.TryParse(m_fieldGuidSerialized, out Guid guid) && !remappedGuids.ContainsKey(guid))
//            {
//                if (GraphView.GraphElements.TryGet(guid, out GraphAbstractField property))
//                    property.AddNode(this);
//            }
//        }

//        /* Value Compile */
//        public override void Compile()
//        {

//        }

//        /* Callbacks */
//        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
//        {
//            evt.menu.AppendAction("Disconnect Output Ports", DisconnectOutputPorts, DisconnectOutputsStatus);

//            evt.menu.AppendSeparator();
//        }

//        public override void OnSelected()
//        {
//            BringToFront();

//            if (AssociatedField != null)
//                AssociatedField.ShowInspectorField(true);

//        }

//        public override void OnUnselected()
//        {
//            if (AssociatedField != null)
//                AssociatedField.ShowInspectorField(false);
//        }

//        public override void OnBeforeSerialize()
//        {
//            base.OnBeforeSerialize();

//            if (AssociatedField != null)
//                m_fieldGuidSerialized = AssociatedField.Guid.ToString();
//        }

//        public void Setup(GraphAbstractField property)
//        {
//            AssociatedField = property;

//            title = AssociatedField.PropertyTitle;

//            m_OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, AssociatedField.Type);
//            m_OutputPort.IsCompatible = AssociatedField.PropertyCompatible;

//            m_OutputPort.portName = "Output";
//            m_OutputPort.displayName = AssociatedField.text;
//            m_OutputPort.portColor = ColorSpaceManager.GetPropertyColor(AssociatedField.Type);

//            m_ports.Add(m_OutputPort.portName, m_OutputPort);

//            m_Pill.right = m_OutputPort;
//            m_Pill.icon = AssociatedField.icon;

//            RegisterCallback<PointerEnterEvent>(callback => AssociatedField.highlighted = true);
//            RegisterCallback<PointerLeaveEvent>(callback => AssociatedField.highlighted = false);

//            ValidateConnections();
//        }

//        public virtual void OnHighlight(bool isHighlighted)
//        {
//            m_Pill.highlighted = isHighlighted;
//        }
//    }
//}