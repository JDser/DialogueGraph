using UnityEngine;

namespace DialogueSystem
{
    public class DialogueAsset : ScriptableObject
    {
        [SerializeField] private Node[] m_Nodes;

        [SerializeField] private BaseVariable[] m_Variables;

        public Node[] Nodes => m_Nodes;

        public BaseVariable[] Variables => m_Variables;



#if UNITY_EDITOR
        [SerializeField] private string m_SerializedGraph;

        /// <summary>
        /// Editor only.
        /// </summary>
        public string SerializedGraph { get => m_SerializedGraph; set => m_SerializedGraph = value; }

        /// <summary>
        /// Editor only.
        /// </summary>
        public void SortObjects(System.Collections.Generic.IEnumerable<Object> objects)
        {
            System.Collections.Generic.List<Node> nodes = new System.Collections.Generic.List<Node>();
            System.Collections.Generic.List<BaseVariable> variables = new System.Collections.Generic.List<BaseVariable>();

            foreach (var item in objects)
            {
                if (item is Node node)
                {
                    nodes.Add(node);
                    continue;
                }
                else if (item is BaseVariable variable)
                {
                    variables.Add(variable);
                    continue;
                }
            }

            m_Nodes = nodes.ToArray();
            m_Variables = variables.ToArray();
        }
#endif
    }
}