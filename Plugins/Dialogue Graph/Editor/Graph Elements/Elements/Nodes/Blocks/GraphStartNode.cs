using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Nodes
{
    public class GraphStartNode : GraphBlockNode
    {
        public override string Title => "Start Node";

        public override Color StartColor => new Color(0.63f, 0.62f, 0.16f);

        public override Color EndColor => new Color(0.63f, 0.62f, 0.16f);

        public GraphStartNode() : base()
        {
            capabilities =
                Capabilities.Movable
                | Capabilities.Ascendable
                | Capabilities.Selectable
                | Capabilities.Snappable
                | Capabilities.Groupable;
        }

        protected override void CreatePorts()
        {
            NodePort OutputFlow = NodePort.Create(this, "Output", Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(GraphBlockNode), new FlowEdgeListener());
            OutputFlow.portColor = EndColor;
            OutputFlow.displayName = null;
            outputContainer.Add(OutputFlow);

            /* "Disabling" port's text field */
            VisualElement textField = OutputFlow.Q("type");
            textField.style.height = 0;
            textField.style.marginLeft = 0;
            textField.style.marginRight = 0;

            m_ports.Add(OutputFlow.portName, OutputFlow);
        }

        public override void Validate()
        {
            DragReorderHelper.GraphView = GraphView;

            m_Header = new StartNodeHeader(this);
            extensionContainer.Insert(0, m_Header);

            ValidateConnections();
        }
    }
}