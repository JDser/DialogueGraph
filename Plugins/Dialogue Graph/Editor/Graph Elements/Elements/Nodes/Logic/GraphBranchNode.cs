//namespace DialogueEditor.Nodes
//{
//    using Properties;

//    [SearchEntry("Logic", "Branch")]
//    [Input("Predicate", typeof(bool), PortSingleCapacity = true)]
//    [Input("True", typeof(object), PortSingleCapacity = true)]
//    [Input("False", typeof(object), PortSingleCapacity = true)]
//    [Output("Out", typeof(object), PortSingleCapacity = false)]
//    public class GraphBranchNode : GraphAbstractNode, IValuePropagation
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "Branch";

//        public GraphAbstractProperty GraphProperty { get; set; }

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input)
//        {
//            if (input.name != "Predicate")
//                this.PropagateDynamicValue(output, Ports);
//        }

//        public override void OnPortDisconnect(NodePort output, NodePort input)
//        {
//            if (input.name != "Predicate")
//                this.SuppressDynamicValue(output);
//        }
//    }
//}