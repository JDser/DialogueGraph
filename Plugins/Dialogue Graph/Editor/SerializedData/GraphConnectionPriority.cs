using System;
using UnityEngine;

namespace DialogueEditor
{
    using DialogueSystem;

    [Serializable]
    public struct GraphConnectionPriority
    {
        /* Serializable */

        [SerializeField] private string m_Guid;

        [SerializeField] private ConnectionPriority m_Priority;

        /* Properties */

        public Guid Guid => Guid.Parse(m_Guid);

        public ConnectionPriority Priority => m_Priority;

        public GraphConnectionPriority(Guid guid, ConnectionPriority priority)
        {
            m_Guid = guid.ToString();
            m_Priority = priority;
        }
    }
}