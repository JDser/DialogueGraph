using DialogueEditor.Nodes;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class StartNodeHeader : NodeHeader
    {
        /* Properties */
        public override string Title => "Start";

        public override string Line => null;

        public override bool IsChoice => false;

        public StartNodeHeader(GraphBlockNode blockNode) : base(blockNode, "Start", string.Empty, false) { }

        protected override void MakeChoiceToggle(GraphBlockNode blockNode, bool isChoice, VisualElement buttonParent) { }

        protected override void SetupTitle(string title)
        {
            Label TitleField = new Label(title);
            contentContainer.Add(TitleField);

            VisualElement titleToRemove = this.Q("title-label");
            titleToRemove.parent.Insert(0, TitleField);
            titleToRemove.parent.Remove(titleToRemove); 
            
            TitleField.name = "title-label";
        }

        protected override void SetupLine(string line) { }
    }
}