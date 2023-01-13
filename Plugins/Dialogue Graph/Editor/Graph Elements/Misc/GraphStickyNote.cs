using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace DialogueEditor
{
    public class GraphStickyNote : StickyNote, IGraphElement, IGroupable, ISerializationCallbackReceiver
    {
        /* Serialized */
        [SerializeField] private string m_GuidSerialized;

        [SerializeField] private Vector2 m_Position;

        [SerializeField] private Vector2 m_Size = new Vector2(200, 180);

        [SerializeField] private string m_Title;

        [SerializeField] private string m_Contents;

        [SerializeField] private StickyNoteTheme m_Theme;

        [SerializeField] private StickyNoteFontSize m_FontSize;

        /* Properties */
        public DialogueGraphView GraphView { get; set; }

        public Guid Guid { get; set; }

        public Vector2 Position
        {
            get => m_Position;
            set
            {
                SetPosition(new Rect(value, m_Size));
                m_Position = value;
            }
        }

        public GraphGroup Group { get; set; }
         
        /* Initialization */
        public GraphStickyNote()
        {
            this.Q("title-field").RegisterCallback<FocusOutEvent>(callback =>
            {
                DialogueEditorWindow.RegisterUndo("Change Sticky Note Title");
            });

            this.Q("contents").Q("contents-field").RegisterCallback<FocusOutEvent>(callback =>
            {
                DialogueEditorWindow.RegisterUndo("Change Sticky Note Contents");
            });
            
            capabilities =
                Capabilities.Movable
                | Capabilities.Deletable
                | Capabilities.Ascendable
                | Capabilities.Selectable
                | Capabilities.Copiable
                | Capabilities.Groupable;          
        }

        /* Callbacks */
        public virtual void OnBeforeSerialize()
        {
            m_GuidSerialized = Guid.ToString();

            Rect rect = GetPosition();
            m_Position = rect.position;
            m_Size = new Vector2(rect.width, rect.height);

            m_Title = title;
            m_Contents = contents;

            m_Theme = theme;
            m_FontSize = fontSize;
        }

        public virtual void OnAfterDeserialize()
        {
            Guid = Guid.Parse(m_GuidSerialized);
            SetPosition(new Rect(m_Position, m_Size));

            title = m_Title;
            contents = m_Contents;

            theme = m_Theme;
            fontSize = m_FontSize;
        }
    }
}