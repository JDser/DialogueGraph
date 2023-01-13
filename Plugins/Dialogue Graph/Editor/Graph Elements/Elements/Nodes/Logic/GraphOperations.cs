//using System;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Nodes
//{
//    using Properties;

//    [SearchEntry("Logic", "Math", "Add", order = 1)]
//    [Output("Out", typeof(object), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationAdd : GraphAbstractNode, IValuePropagation
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A + B";

//        public GraphAbstractProperty GraphProperty { get; set; }

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Math", "Subtract", order = 2)]
//    [Output("Out", typeof(object), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationSubtract : GraphAbstractNode, IValuePropagation
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A - B";

//        //public IGraphProperty GraphProperty { get; set; }
//        public GraphAbstractProperty GraphProperty { get; set; }

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Math", "Multiply", order = 3)]
//    [Output("Out", typeof(object), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationMultiply : GraphAbstractNode, IValuePropagation
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A * B";

//        //public IGraphProperty GraphProperty { get; set; }
//        public GraphAbstractProperty GraphProperty { get; set; }

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Math", "Divide", order = 4)]
//    [Output("Out", typeof(object), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationDivide : GraphAbstractNode, IValuePropagation
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A / B";

//        //public IGraphProperty GraphProperty { get; set; }
//        public GraphAbstractProperty GraphProperty { get; set; }

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    /* Comparison Operations */
//    [SearchEntry("Logic", "Compare", "Is Equal", order = 4)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsEqual : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A == B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Compare", "Is Not Equal", order = 5)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsNotEqual : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A != B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Compare", "Is Less", order = 8)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsLess : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A < B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Compare", "Is Less Equal", order = 9)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsLessEqual : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A =< B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Compare", "Is Greater", order = 6)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsGreater : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A > B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }

//    [SearchEntry("Logic", "Compare", "Is Greater Equal", order = 7)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(object), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(object), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationIsGreaterEqual : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "A >= B";

//        public override void Compile()
//        {

//        }

//        public override void OnPortConnect(NodePort output, NodePort input) => this.PropagateDynamicValue(output, Ports);

//        public override void OnPortDisconnect(NodePort output, NodePort input) => this.SuppressDynamicValue(output);
//    }


//    [SearchEntry("Logic", "Compare", "AND", order = 0)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(bool), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(bool), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationAnd : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "AND";

//        protected override void OnValidate()
//        {
//            Action<VisualElement, string> setData = (e, s) =>
//             {
//                 bool.TryParse(s, out bool result);
//                 (e as Toggle).value = result;
//             };

//            Func<VisualElement, string> getData = (e) =>
//            {
//                return (e as Toggle).value.ToString();
//            };


//            Toggle toggleA = new Toggle();

//            GetPort("ValueA").AddVirtualConnection(toggleA, setData, getData);

//            Toggle toggleB = new Toggle();

//            GetPort("ValueB").AddVirtualConnection(toggleB, setData, getData);
//        }

//        public override void Compile()
//        {

//        }
//    }

//    [SearchEntry("Logic", "Compare", "OR", order = 1)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(bool), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(bool), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationOr : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "OR";

//        protected override void OnValidate()
//        {
//            Action<VisualElement, string> setData = (e, s) =>
//            {
//                bool.TryParse(s, out bool result);
//                (e as Toggle).value = result;
//            };

//            Func<VisualElement, string> getData = (e) =>
//            {
//                return (e as Toggle).value.ToString();
//            };


//            Toggle toggleA = new Toggle();

//            GetPort("ValueA").AddVirtualConnection(toggleA, setData, getData);

//            Toggle toggleB = new Toggle();

//            GetPort("ValueB").AddVirtualConnection(toggleB, setData, getData);
//        }

//        public override void Compile()
//        {

//        }
//    }

//    [SearchEntry("Logic", "Compare", "NOT", order = 2)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(bool), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(bool), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationNOT : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "NOT";

//        protected override void OnValidate()
//        {
//            Action<VisualElement, string> setData = (e, s) =>
//            {
//                bool.TryParse(s, out bool result);
//                (e as Toggle).value = result;
//            };

//            Func<VisualElement, string> getData = (e) =>
//            {
//                return (e as Toggle).value.ToString();
//            };


//            Toggle toggleA = new Toggle();

//            GetPort("ValueA").AddVirtualConnection(toggleA, setData, getData);

//            Toggle toggleB = new Toggle();

//            GetPort("ValueB").AddVirtualConnection(toggleB, setData, getData);
//        }

//        public override void Compile()
//        {

//        }
//    }

//    [SearchEntry("Logic", "Compare", "NOR", order = 3)]
//    [Output("Out", typeof(bool), PortSingleCapacity = false)]
//    [Input("ValueA", typeof(bool), Name = "A", PortSingleCapacity = true)]
//    [Input("ValueB", typeof(bool), Name = "B", PortSingleCapacity = true)]
//    public class GraphOperationNOR : GraphAbstractNode
//    {
//        protected override string Category => "Logic";

//        public override string DefaultTitle => "NOR";

//        protected override void OnValidate()
//        {
//            Action<VisualElement, string> setData = (e, s) =>
//            {
//                bool.TryParse(s, out bool result);
//                (e as Toggle).value = result;
//            };

//            Func<VisualElement, string> getData = (e) =>
//            {
//                return (e as Toggle).value.ToString();
//            };


//            Toggle toggleA = new Toggle();

//            GetPort("ValueA").AddVirtualConnection(toggleA, setData, getData);

//            Toggle toggleB = new Toggle();

//            GetPort("ValueB").AddVirtualConnection(toggleB, setData, getData);
//        }

//        public override void Compile()
//        {

//        }
//    }
//}