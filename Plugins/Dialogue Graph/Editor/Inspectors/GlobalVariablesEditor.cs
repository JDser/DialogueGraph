//using System;
//using System.Linq;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditorInternal;
//using UnityEngine;

//namespace DialogueEditor
//{
//    /* Generic Menu source */
//    // https://docs.unity3d.com/ScriptReference/GenericMenu.html

//    /* Reordable List tutorial */
//    // https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/

//    using DialogueSystem;

//    //[CustomEditor(typeof(MetaProperties))]
//    public class GlobalVariablesEditor : Editor
//    {
//        [MenuItem("Assets/Create/Dialogue System/Variables Table", false, 1)]
//        public static void CreateDialogueAsset()
//        {
//            ProjectWindowUtil.CreateAsset(CreateInstance<VariableTable>(), "New Variables Table.asset");
//        }

//        private SerializedProperty m_PropertiesArray;

//        private ReorderableList m_PropertyList;

//        private void OnEnable()
//        {
//            //m_PropertiesArray = serializedObject.FindProperty("m_Properties");
//            //
//            //m_PropertyList = new ReorderableList(serializedObject, m_PropertiesArray, true, true, true, true);
//            //
//            //m_PropertyList.drawHeaderCallback = DrawListHeader;
//            //m_PropertyList.drawElementCallback = DrawListItems;
//            //
//            //m_PropertyList.onAddCallback = AddNewListItem;
//            //m_PropertyList.onRemoveCallback = RemoveListItem;
//        }

//        public override void OnInspectorGUI()
//        {
//            GUILayout.Space(10);

//            m_PropertyList.DoLayoutList();
//            serializedObject.ApplyModifiedProperties();
//        }

//        private void DrawListHeader(Rect rect)
//        {
//            EditorGUI.LabelField(rect, "Properties");
//        }

//        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
//        {
//            if (index >= m_PropertyList.serializedProperty.arraySize)
//                return;

//            SerializedProperty element = m_PropertyList.serializedProperty.GetArrayElementAtIndex(index);

//            if (element.objectReferenceValue == null)
//            {
//                m_PropertyList.serializedProperty.DeleteArrayElementAtIndex(index);
//                return;
//            }


//            Rect fieldRect = new Rect(rect);
//            fieldRect.height = EditorGUIUtility.singleLineHeight;
//            fieldRect.y += 2.5f; /* Make it looks like centered */

//            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
//        }

//        private void AddNewListItem(ReorderableList list)
//        {
//            ///* Display Generic Menu with available property types */
//            //GenericMenu menu = new GenericMenu();
//            //
//            //IEnumerable<Type> exporters = typeof(BaseProperty).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseProperty)) && !t.IsAbstract);
//            //
//            //foreach (var type in exporters)
//            //{
//            //    string propertyName = type.Name.Replace("Property", "");
//            //    menu.AddItem(new GUIContent(propertyName), false, () =>
//            //    {
//            //        BaseProperty newProperty = CreateInstance(type) as BaseProperty;
//            //
//            //        newProperty.name = $"New Meta {propertyName}";
//            //
//            //        target.AddObject(newProperty);
//            //
//            //        m_PropertiesArray.InsertArrayElementAtIndex(m_PropertiesArray.arraySize);
//            //        m_PropertiesArray.GetArrayElementAtIndex(m_PropertiesArray.arraySize - 1).objectReferenceValue = newProperty;
//            //
//            //        serializedObject.ApplyModifiedProperties();
//            //    });
//            //}
//            //
//            //menu.ShowAsContext();
//        }

//        private void RemoveListItem(ReorderableList list)
//        {
//            SerializedProperty element = m_PropertyList.serializedProperty.GetArrayElementAtIndex(list.index);

//            UnityEngine.Object toDelete = element.objectReferenceValue;
//            DestroyImmediate(toDelete, true);

//            m_PropertyList.serializedProperty.DeleteArrayElementAtIndex(list.index);
//        }

//    }
//}