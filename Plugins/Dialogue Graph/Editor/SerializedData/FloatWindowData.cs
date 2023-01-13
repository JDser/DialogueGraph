using System;
using UnityEngine;

namespace DialogueEditor
{
    [Serializable]
    public struct FloatWindowData
    {
        /* Serializable */

        [SerializeField] private bool m_IsActive;

        [SerializeField] private Vector2 m_Positon;

        [SerializeField] private Vector2 m_Size;

        /* Properties */

        public bool IsActive => m_IsActive;

        public Rect Rect => new Rect(m_Positon, m_Size);

        public FloatWindowData(bool isActive, Rect rect)
        {
            this.m_IsActive = isActive;
            this.m_Positon = rect.position;
            this.m_Size = rect.size;
        }
    }
}