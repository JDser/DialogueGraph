using UnityEngine;

namespace DialogueEditor
{
    public class GraphUndoObject : ScriptableObject
    {
        [SerializeField] private string m_SerializedGraphData;

        [SerializeField] private int m_SerializedVersion;

        public string SerializedData { get => m_SerializedGraphData; set => m_SerializedGraphData = value; }

        public int SerializedVersion { get => m_SerializedVersion; set => m_SerializedVersion = value; }

        public int CurrentVersion { get; set; }

        public bool wasUndoPerformed => CurrentVersion != m_SerializedVersion;

        public SerializedGraphData Wrapper
        {
            get => SerializedGraphData.FromJson(m_SerializedGraphData);
            set => m_SerializedGraphData = JsonUtility.ToJson(value);
        }
    }
}