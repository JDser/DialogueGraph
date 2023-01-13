//using System;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace DialogueEditor.Fields
//{
//    using DialogueSystem;
//    using Node = UnityEditor.Experimental.GraphView.Node;

//    public abstract class GraphAbstractField : BlackboardField, IGraphElement, IGraphEvents, ICompilable , ISerializationCallbackReceiver
//    {
//        /* Serialized */

//        [SerializeField] private string m_guidSerialized;

//        [SerializeField] private string m_titleSerialized;

//        [SerializeField] private int m_serializedListIndex;

//        [SerializeField] private string[] m_SerializedGuids;

//        /* Non-Serialized */

//        protected GraphBlackboard m_Blackboard;

//        protected InspectorField m_InspectorField;

//        /* Properties */

//        /* IGraphElement Properties */

//        public Guid Guid { get; set; }

//        public Vector2 Position { get; set; }

//        public DialogueGraphView GraphView { get; set; }

//        public virtual string DefaultTitle => "Abstract Property";

//        /* IGraphProperty Properties */

//        public virtual string TypeTitle => "Abstract";

//        public string PropertyTitle { get => m_titleSerialized; set => m_titleSerialized = value; }

//        public int SerializedListIndex => m_serializedListIndex;

//        public List<GraphPropertyNode> AssociatedNodes { get; private set; }

//        public virtual string CurrentValue { get; }

//        public virtual Func<NodePort, bool> PropertyCompatible => other => false;

//        public abstract Type Type { get; }

//        public virtual bool IsConvertable() => true;

//        /* Initialization */
//        public GraphAbstractField()
//        {
//            RegisterCallback<FocusOutEvent>(FocusOut);

//            AssociatedNodes = new List<GraphPropertyNode>();

//            RegisterCallback<PointerEnterEvent>(callback =>
//            {
//                for (int i = 0; i < AssociatedNodes.Count; i++)
//                {
//                    AssociatedNodes[i].OnHighlight(true);
//                }
//            });

//            RegisterCallback<PointerLeaveEvent>(callback =>
//            {
//                for (int i = 0; i < AssociatedNodes.Count; i++)
//                {
//                    AssociatedNodes[i].OnHighlight(false);
//                }
//            });

//            PropertyTitle = DefaultTitle;

//            Color color = ColorSpaceManager.GetPropertyColor(Type);
//            icon = DialogueResourceManager.DrawTextureCircle(color);

//            capabilities =
//                Capabilities.Selectable
//                | Capabilities.Droppable
//                | Capabilities.Deletable
//                | Capabilities.Renamable
//                | Capabilities.Copiable;
//        }

//        public virtual void Validate()
//        {
//            text = PropertyTitle;
//            typeText = TypeTitle;

//            m_Blackboard = GraphView.Blackboard;
//            m_Blackboard.AddToContent(this);

//            if (m_SerializedGuids.IsNullOrEmpty())
//                return;

//            for (int i = 0; i < m_SerializedGuids.Length; i++)
//            {
//                if (Guid.TryParse(m_SerializedGuids[i], out Guid guid))
//                {
//                    AddNode(GraphView.GraphElements.Get<GraphPropertyNode>(guid));
//                }
//            }
//        }

//        public virtual void Remove()
//        {

//        }

//        public virtual void OnPaste(Dictionary<Guid, Guid> remappedGuids)
//        {
//            PropertyTitle += " (copy)";

//            for (int i = 0; i < m_SerializedGuids.Length; i++)
//            {
//                if (!Guid.TryParse(m_SerializedGuids[i], out Guid guid))
//                    continue;

//                if (remappedGuids.ContainsKey(guid))
//                {
//                    m_SerializedGuids[i] = remappedGuids[guid].ToString();
//                }
//                else
//                {
//                    GraphPropertyNode node = GraphView.GraphElements.Get<GraphPropertyNode>(guid);
//                    if (node.AssociatedField != null)
//                        m_SerializedGuids[i] = null;
//                }
//            }
//        }

//        /* Node Managment */
//        public void AddNode(GraphPropertyNode node)
//        {
//            AssociatedNodes.Add(node);
//            node.Setup(this);
//        }

//        public void RemoveNode(GraphPropertyNode variableNode)
//        {
//            if (AssociatedNodes.Contains(variableNode))
//            {
//                AssociatedNodes.Remove(variableNode);
//                highlighted = false;
//            }
//        }

//        /* Inline Node */
//        public virtual VisualElement GetValueField()
//        { throw new NotImplementedException(); }

//        public virtual void SetFieldData(VisualElement e, string data)
//        { throw new NotImplementedException(); }

//        public virtual string GetFieldData(VisualElement e)
//        { throw new NotImplementedException(); }

//        public virtual void SetValue(GraphInlinePropertyNode propertyNode)
//        { throw new NotImplementedException(); }

//        /* Callbacks */
//        private void FocusOut(FocusOutEvent evt)
//        {
//            if (m_InspectorField != null)
//                m_InspectorField.Title = $"Property : {text}";

//            if (AssociatedNodes != null)
//            {
//                for (int i = 0; i < AssociatedNodes.Count; i++)
//                {
//                    (AssociatedNodes[i] as Node).title = text;
//                }
//            }

//            GraphView.EditorWindow.RegisterUndo("Property Changed");
//        }

//        public override void OnSelected()
//        {
//            if (m_InspectorField != null)
//                GraphView.Inspector.AddToContent(m_InspectorField);
//        }

//        public override void OnUnselected()
//        {
//            if (m_InspectorField != null)
//                GraphView.Inspector.RemoveFromContent(m_InspectorField);
//        }

//        public void OnBeforeSerialize()
//        {
//            m_guidSerialized = Guid.ToString();
//            PropertyTitle = text;
//            m_serializedListIndex = parent.IndexOf(this);

//            m_SerializedGuids = new string[AssociatedNodes.Count];
//            for (int i = 0; i < m_SerializedGuids.Length; i++)
//            {
//                m_SerializedGuids[i] = AssociatedNodes[i].Guid.ToString();
//            }
//        }

//        public void OnAfterDeserialize()
//        {
//            Guid = Guid.Parse(m_guidSerialized);
//        }

//        public virtual void Compile()
//        {

//        }
//    }
//}