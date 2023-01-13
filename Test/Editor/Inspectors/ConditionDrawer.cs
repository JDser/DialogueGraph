using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (GUI.Button(position, "Open Conditions Editor"))
        {
            Debug.Log("Edit");
            EditorWindow.CreateWindow<ConditionEditorWindow>("Conditions Editor", typeof(DialogueEditor.DialogueEditorWindow), typeof(SceneView));
        }
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return base.CreatePropertyGUI(property);
    }

}