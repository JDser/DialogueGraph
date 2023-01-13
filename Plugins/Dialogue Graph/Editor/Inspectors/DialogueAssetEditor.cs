using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace DialogueEditor
{
    using DialogueSystem;

    // Customising Unitys Menu Bar using MenuItem
    // https://hugecalf-studios.github.io/unity-lessons/lessons/editor/menuitem/

    [CustomEditor(typeof(DialogueAsset))]
    public class DialogueAssetEditor : Editor
    {    
		[MenuItem("Assets/Create/Dialogue System/Dialogue Asset", false, 0)]
        public static void CreateDialogueAsset()
        {
            ProjectWindowUtil.CreateAsset(CreateInstance<DialogueAsset>(), "New Dialogue.asset");
        }
		
        [OnOpenAsset]
        private static bool OpenEditor(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is DialogueAsset asset)
            {
                DialogueEditorWindow.Open(asset);
                return true;
            }
            return false;
        }

        //private SerializedProperty m_PropertiesArray;
        //private ReorderableList m_PropertyList;
        //private bool m_PropListFoldout = true;

        private SerializedProperty m_NodesArray;
        private ReorderableList m_NodeList;
        private bool m_NodeListFoldout = true;

        private void OnEnable()
        {
            //m_PropertiesArray = serializedObject.FindProperty("m_Properties");
            //MakePropertyList();

            m_NodesArray = serializedObject.FindProperty("m_Nodes");
            MakeNodeList();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            if (GUILayout.Button("Open Dialogue Editor"))
            {
                DialogueEditorWindow.Open(target as DialogueAsset);
            }
            GUILayout.Space(10);

            serializedObject.Update();

            //m_PropListFoldout = EditorGUILayout.Foldout(m_PropListFoldout, "Properties");
            //if (m_PropListFoldout)
            //{
            //    GUILayout.Space(5);
            //    m_PropertyList.DoLayoutList();
            //}  
            
            m_NodeListFoldout = EditorGUILayout.Foldout(m_NodeListFoldout, "Nodes");
            if (m_NodeListFoldout)
            {
                GUILayout.Space(5);
                m_NodeList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Lists
        //private void MakePropertyList()
        //{
        //    m_PropertyList = new ReorderableList(serializedObject, m_PropertiesArray, true, false, false, false);
        //    m_PropertyList.drawElementCallback = DrawPropertyListItems;
        //}

        //private void DrawPropertyListItems(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    if (index >= m_PropertyList.serializedProperty.arraySize)
        //        return;

        //    SerializedProperty element = m_PropertyList.serializedProperty.GetArrayElementAtIndex(index);

        //    if (element.objectReferenceValue == null)
        //    {
        //        m_PropertyList.serializedProperty.DeleteArrayElementAtIndex(index);
        //        return;
        //    }

        //    Rect fieldRect = new Rect(rect);
        //    fieldRect.height = EditorGUIUtility.singleLineHeight;
        //    fieldRect.y += 2.5f; /* Makes it looks like centered */

        //    EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
        //}    
        
        private void MakeNodeList()
        {
            m_NodeList = new ReorderableList(serializedObject, m_NodesArray, true, false, false, false);
            m_NodeList.drawElementCallback = DrawNodeListItems;
        }

        private void DrawNodeListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= m_NodeList.serializedProperty.arraySize)
                return;

            SerializedProperty element = m_NodeList.serializedProperty.GetArrayElementAtIndex(index);

            if (element.objectReferenceValue == null)
            {
                m_NodeList.serializedProperty.DeleteArrayElementAtIndex(index);
                return;
            }

            Rect fieldRect = new Rect(rect);
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.y += 2.5f; /* Makes it looks like centered */

            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
        }
        #endregion
    }
}