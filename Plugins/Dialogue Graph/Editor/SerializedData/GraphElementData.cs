using System;
using UnityEngine;

namespace DialogueEditor
{
    [Serializable]
    public struct GraphElementData
    {
        /* Serializable */

        [SerializeField] private string m_Type;

        [SerializeField] private string m_Data;

        /* Properties */

        public Type Type => Type.GetType(m_Type);

        public string Data => m_Data;

        public GraphElementData(Type type, string data)
        {
            this.m_Type = type.ToString();
            this.m_Data = data;
        }

        public bool IsValid()
        {
            if (Type == null) return false;
            return !string.IsNullOrEmpty(m_Data);
        }
    }
}