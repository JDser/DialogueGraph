using System;
using UnityEngine;

namespace DialogueSystem
{
    public enum ConnectionPriority
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    }

    [Serializable]
    public struct Connection : IComparable
    {
        [SerializeField] private Node m_Node;
        [SerializeField] private ConnectionPriority m_Priority;

        public Node Node => m_Node;

        public ConnectionPriority Priority => m_Priority;

        public Connection(Node node, ConnectionPriority priority)
        {
            m_Node = node;
            m_Priority = priority;
        }

        public int CompareTo(object obj)
        {
            return Mathf.Clamp(((Connection)obj).m_Priority - this.m_Priority, -1, 1);
        }
    }
}