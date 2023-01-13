using UnityEditor;
using UnityEngine;

namespace DialogueEditor
{
    using DialogueSystem;

    /* using SerializedObject stackoverflow */
    // https://stackoverflow.com/questions/56122827/is-there-a-way-to-use-serializedobject-in-editorwindow-script

    public class DialogueEditorWindow : EditorWindow, ISerializationCallbackReceiver
    {
        #region Static
        private static DialogueEditorWindow CurrentWindow { get; set; }

        public static void RegisterUndo(string undoActionName)
        {
            if (CurrentWindow == null)
            {
                throw new System.NullReferenceException("CurrentWindow is null!");
            }

            CurrentWindow.RegisterUndoAsync(undoActionName);
        }
        #endregion

        #region Variables
        [SerializeField] private DialogueAsset m_context;

        [SerializeField] private string m_contextName;

        [SerializeField] private string m_contextPath;

        [SerializeField] private bool m_IgnoreMissingContext;

        [SerializeField] private string m_tempSerializedGraph;

        public DialogueAsset Context => m_context; 

        public string ContextName => m_contextName;

        public string ContextPath => m_contextPath;

        public string WindowName => "Dialogue Editor";

        private DialogueGraphView GraphView { get; set; }

        private GraphUndoObject GraphUndoObject { get; set; }
        #endregion

        public static void Open(DialogueAsset asset)
        {
            if (EditorWindowsManager.TryGetWindow(asset, out DialogueEditorWindow window))
            {
                window.Focus();

                /* Force Show window if error occurs  */
                if (!window.hasFocus && !window.docked)
                    window.Show();

                return;
            }

            window.m_context = asset;
            window.m_contextName = asset.name;
            window.m_contextPath = AssetDatabase.GetAssetPath(asset);

            window.Setup();
        }

        private void Setup()
        {
            titleContent.text = m_contextName;
            titleContent.image = DialogueResourceManager.GetDialogueIcon();

            hasUnsavedChanges = false;

            Undo.undoRedoPerformed += UndoPerformed;
            saveChangesMessage = $"Do you want to save the changes you made in the Dialogue Graph? \n\n{m_contextPath} \n\nYour changes will be lost if you don't save them.";

            if (GraphUndoObject == null)
                GraphUndoObject = CreateInstance<GraphUndoObject>();
            GraphUndoObject.SerializedData = Context != null ? Context.SerializedGraph : m_tempSerializedGraph;

            GraphView = new DialogueGraphView(this, GraphUndoObject);

            DelayedFrameAll(100);
        }

        private void OnEnable()
        {
            if (Context != null || !string.IsNullOrEmpty(m_tempSerializedGraph))
            {
                Setup();
            }
        }

        #region Original
        //private void Setup(DialogueAsset asset)
        //{
        //    m_context = asset;

        //    m_contextName = m_context.name;
        //    m_contextPath = AssetDatabase.GetAssetPath(m_context);

        //    titleContent.image = DialogueResourceManager.GetDialogueIcon();
        //    titleContent.text = m_contextName;

        //    GraphUndoObject.SerializedData = Context.SerializedGraph;

        //    if (GraphUndoObject.Wrapper != null)
        //    {
        //        GraphView.ClearGraph();
        //        GraphView.Deserialize(GraphUndoObject.Wrapper);
        //    }

        //    DelayedFrameAll(100);
        //}

        //private void OnEnable()
        //{
        //    if (GraphView != null)
        //    {
        //        Debug.LogError("Somehow there was graphView!");
        //        rootVisualElement.Remove(GraphView);
        //    }

        //    Undo.undoRedoPerformed += UndoPerformed;
        //    saveChangesMessage = $"Do you want to save the changes you made in the Dialogue Graph? \n\n{m_contextPath} \n\nYour changes will be lost if you don't save them.";

        //    if (GraphUndoObject == null)
        //        GraphUndoObject = CreateInstance<GraphUndoObject>();
        //    GraphUndoObject.SerializedData = Context != null ? Context.SerializedGraph : m_tempSerializedGraph;

        //    GraphView = new DialogueGraphView(this, GraphUndoObject);

        //    Debug.Log("FrameAll");
        //}
        #endregion

        private void OnFocus()
        {
            if (Context != null) // check if asset was renamed
                m_contextName = Context.name;

            titleContent.text = m_contextName;

            CurrentWindow = this;
        }

        private void OnDestroy()
        {
            if (CurrentWindow == this)
            {
                Undo.undoRedoPerformed = null;
            }

            Undo.ClearUndo(GraphUndoObject);
            EditorWindowsManager.Remove(Context);
        }

        private void OnInspectorUpdate()
        {
            if (Context == null && !m_IgnoreMissingContext)
            {
                int result = EditorUtility.DisplayDialogComplex(
                    "Asset removed from project", // Title
                    $"The file has been deleted or removed from the project folder. \n\n{m_contextPath} \n\nWould you like to save your Dialogue Asset?", // Message
                    "Save As...", // OK
                    "Discard Graph and Close Window", // Cancel
                    "Cancel"); // Alt

                /* OK */
                if (result == 0)
                {
                    SaveNewAsset();
                }
                /* Cancel */
                else if (result == 1)
                {
                    Close();
                }
                /* Alternative */
                else
                {
                    m_contextName += " (deleted)";
                    titleContent.text = m_contextName;
                    m_tempSerializedGraph = JsonUtility.ToJson(GraphView.Serialize());

                    m_IgnoreMissingContext = true;
                }

                return;
            }
        }

        private async void DelayedFrameAll(int milisecondsTime)
        {
            await System.Threading.Tasks.Task.Delay(milisecondsTime); // Bad practise(?)
            GraphView.FrameAll();
        }

        public void OnBeforeSerialize()
        { }

        public void OnAfterDeserialize()
        {
            EditorWindowsManager.Reassign(Context, this);
        }

        /* Save Methods */
        public override void SaveChanges()
        {
            if (Context != null)
                SaveAsset();
            else
                SaveNewAsset();
        }

        public void SaveAsNewAsset()
        {
            if (Context != null)
                CopyAsset();
            else
                SaveNewAsset();
        }

        public void ShowAsset()
        {
            if (Context != null)
                EditorGUIUtility.PingObject(Context);
        }

        private void SaveAsset()
        {
            GraphView.Save();
            EditorUtility.SetDirty(Context);
            hasUnsavedChanges = false;
        }

        private void CopyAsset()
        {
            string folderPath = m_contextPath.Remove(m_contextPath.LastIndexOf('/'));
            string newPath = EditorUtility.SaveFilePanel("Save Dialogue As...", folderPath, m_contextName + " (copy)", "asset");
            if (string.IsNullOrEmpty(newPath))
                return;

            newPath = DialogueEditorUtility.RelatePathToAssetFolder(newPath);

            SaveAsset();

            bool success = AssetDatabase.CopyAsset(m_contextPath, newPath);
            if (success)
            {
                Open(AssetDatabase.LoadMainAssetAtPath(newPath) as DialogueAsset);
            }
        }

        private void SaveNewAsset()
        {
            string folderPath = m_contextPath.Remove(m_contextPath.LastIndexOf('/'));
            string newPath = EditorUtility.SaveFilePanel("Save Dialogue As...", folderPath, m_contextName, "asset");
            if (string.IsNullOrEmpty(newPath))
                return;

            newPath = DialogueEditorUtility.RelatePathToAssetFolder(newPath);

            m_context = CreateInstance<DialogueAsset>();
            AssetDatabase.CreateAsset(m_context, newPath);

            SaveAsset();

            EditorWindowsManager.Reassign(m_context, this);

            m_contextName = m_context.name;
            m_contextPath = newPath;
            m_IgnoreMissingContext = false;

            titleContent.text = m_contextName;
        }

        /* Undo Methods */
        private async void RegisterUndoAsync(string operationName)
        {
            hasUnsavedChanges = true;

            //Skip 1 frame
            //Not serializing newly added to te graphView nodes
            await System.Threading.Tasks.Task.Delay(100); // Bad practise(?)

            /* GraphUndoObject gets removed if Editor goes to PlayMode */
            if (GraphUndoObject == null)
            {
                GraphUndoObject = CreateInstance<GraphUndoObject>();
            }

            Undo.RegisterCompleteObjectUndo(GraphUndoObject, operationName);

            GraphUndoObject.CurrentVersion++;
            GraphUndoObject.SerializedVersion = GraphUndoObject.CurrentVersion;
            GraphUndoObject.Wrapper = GraphView.Serialize();
        }

        private void UndoPerformed()
        {
            if (GraphUndoObject.Wrapper == null || !GraphUndoObject.wasUndoPerformed)
                return;

            GraphView.ClearGraph();
            GraphView.Deserialize(GraphUndoObject.Wrapper);

            GraphUndoObject.CurrentVersion = GraphUndoObject.SerializedVersion;
        }
    }
}