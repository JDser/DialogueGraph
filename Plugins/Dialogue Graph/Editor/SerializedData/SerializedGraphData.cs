using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueEditor
{
    [Serializable]
    public class SerializedGraphData : ISerializationCallbackReceiver
    {
        /* Serializable */

        [SerializeField] private ColorMode m_ColorMode;

        [SerializeField] private FloatWindowData m_BlackboardData;

        [SerializeField] private FloatWindowData m_InspectorData;

        [SerializeField] private List<GraphElementData> m_SerializedElements = new List<GraphElementData>();

        [NonSerialized] private HashSet<GraphElement> m_ElementsHashset = new HashSet<GraphElement>();

        /* Properties */

        public ColorMode ColorMode => m_ColorMode;

        public FloatWindowData BlackboardData => m_BlackboardData;

        public FloatWindowData InspectorData => m_InspectorData;

        public IEnumerable<GraphElement> GraphElements => m_ElementsHashset;

        public SerializedGraphData() { }

        public SerializedGraphData(IEnumerable<GraphElement> elements)
        {
            foreach (var item in elements)
            {
                m_ElementsHashset.Add(item);
            }
        }

        public SerializedGraphData(ColorMode colorMode, FloatWindowData blackboardData, FloatWindowData inspectorData, IEnumerable<GraphElement> elements)
        {
            m_ColorMode = colorMode;

            m_BlackboardData = blackboardData;

            m_InspectorData = inspectorData;

            foreach (var item in elements)
            {
                m_ElementsHashset.Add(item);
            }
        }

        public void OnBeforeSerialize()
        {
            m_SerializedElements = SerializeList(m_ElementsHashset);
        }

        public void OnAfterDeserialize()
        {
            m_ElementsHashset.Clear();
            var deserializedNode = DeserializeList<GraphElement>(m_SerializedElements);
            foreach (var item in deserializedNode)
            {
                m_ElementsHashset.Add(item);
            }
        }

        public static SerializedGraphData FromJson(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            try
            {
                SerializedGraphData deseralized = JsonUtility.FromJson<SerializedGraphData>(data);
                return deseralized.m_ElementsHashset.Count != 0 ? deseralized : null;
            }
            catch
            {
                return null;
            }
        }

        public static List<GraphElementData> SerializeList<T>(IEnumerable<T> list)
        {
            List<GraphElementData> result = new List<GraphElementData>();
            foreach (T item in list)
            {
                result.Add(new GraphElementData(item.GetType(), JsonUtility.ToJson(item)));
            }

            return result;
        }

        public static List<T> DeserializeList<T>(IEnumerable<GraphElementData> list)
        {
            List<T> result = new List<T>();
            foreach (var item in list)
            {
                if (!item.IsValid())
                {
                    Debug.LogError("Create Node Error");
                    continue;
                }

                T instance = DialogueEditorUtility.Create<T>(item.Type);

                JsonUtility.FromJsonOverwrite(item.Data, instance);

                result.Add(instance);
            }

            return result;
        }
    }
}