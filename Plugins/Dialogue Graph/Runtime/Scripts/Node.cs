using UnityEngine;

namespace DialogueSystem
{
    public sealed class Node : ScriptableObject
    {
        [SerializeField] private string m_Line;

        [SerializeField] private bool m_IsChoice;

        [SerializeField] private Connection[] m_Connections;

        [SerializeField] private Property[] m_Properties;

        public string Title => name;

        public string Line => m_Line;

        public bool IsChoice => m_IsChoice;

        public Connection[] Connections => m_Connections;

        public Property[] Properties => m_Properties;

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor only.
        /// </summary>
        public void Set(string title, string line, bool isChoice, Connection[] connections, Property[] properties)
        {
            this.name = title;
            this.m_Line = line;
            this.m_IsChoice = isChoice;
            this.m_Connections = connections;
            this.m_Properties = properties;
        }
#endif
    }
}