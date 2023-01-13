using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogueEditor
{
    internal static class EditorWindowsManager
    {
        private static readonly Dictionary<Object, EditorWindow> windows = new Dictionary<Object, EditorWindow>();

        public static bool TryGetWindow<T>(Object context, out T newWindow) where T : EditorWindow
        {
            if (windows.TryGetValue(context, out EditorWindow window))
            {
                newWindow = (T)window;
                return true;
            }

            newWindow = EditorWindow.CreateWindow<T>(context.name, typeof(T), typeof(SceneView));
            windows.Add(context, newWindow);
            return false;
        }

        public static void Remove(Object context)
        {
            if (windows.ContainsKey(context))
                windows.Remove(context);
        }

        public static void Reassign(Object context, EditorWindow editorWindow)
        {
            if (windows.ContainsValue(editorWindow))
            {
                var key = windows.First(kvp => kvp.Value == editorWindow).Key;
                windows.Remove(key);
            }

            if (windows.ContainsKey(context))
            {
                windows[context] = editorWindow;
                return;
            }

            windows.Add(context, editorWindow);
        }
    }
}