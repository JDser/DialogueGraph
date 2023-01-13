using UnityEditor;
using UnityEngine;

namespace DialogueEditor
{
    [System.Serializable]
    public class ObjectReference<T> : ISerializationCallbackReceiver where T : ScriptableObject
    {
        /* Serializable */

        [SerializeField] private string m_ObjectID;

        /* Properties */

        public T Reference { get; private set; }

        public ObjectReference()
        {
            Reference = ScriptableObject.CreateInstance<T>();
        }

        public void OnBeforeSerialize()
        {
            if (Reference == null)
            {
                m_ObjectID = null;
                return;
            }

            m_ObjectID = GlobalObjectId.GetGlobalObjectIdSlow(Reference).ToString();
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(m_ObjectID) && GlobalObjectId.TryParse(m_ObjectID, out GlobalObjectId id))
            {
                Reference = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as T;
            }
        }
    }
}