using System;
using System.Collections.Generic;


namespace DialogueEditor
{
    public interface IConnectable
    {
        /* Properties */

        public Guid Guid { get; set; }

        public DialogueGraphView GraphView { get; set; }

        public GraphConnection[] GraphConnections { get; set; }

        public IEnumerable<NodePort> Ports { get; }

        public IEnumerable<NodePort> Outputs { get; }

        public IEnumerable<NodePort> Inputs { get; }

        /* Methods */
        
        public NodePort GetPort(string portName);

        public NodePort GetCompatiblePort(NodePort other);

        public bool RefreshPorts();
    }
}