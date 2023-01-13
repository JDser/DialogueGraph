using DialogueEditor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    public class NodeHeader : GraphElement
    {
        /* Properties */

        public TextField TitleField { get; private set; }

        public TextField LineField { get; private set; }

        public Toggle ChoiceToggle { get; private set; }

        public virtual string Title => TitleField.text;

        public virtual string Line => LineField.text;

        public virtual bool IsChoice => ChoiceToggle.value;

        public new VisualElement contentContainer { get; private set; }

        public NodeHeader(Nodes.GraphBlockNode blockNode, string title, string line, bool isChoice)
        {
            ClearClassList();
            AddToClassList("blockProperty");

            capabilities = 0;

            this.LoadUXMLTree(DialogueResourceManager.BlockPropertyUXML);
            this.AddStyleSheets(DialogueResourceManager.BlockPropertyStyleSheets);
            this.AddManipulator(new SelectionDropper());

            contentContainer = this.Q("contents");

            this.Q("ports").Delete();
            this.Query("dividerHorizontal").Last().Delete();

            this.Q("node-border").style.backgroundColor = new StyleColor(new UnityEngine.Color(0.17f, 0.17f, 0.17f, 0.8f));
            this.Q("selection-border").Delete();

            VisualElement collapseButton = this.Q("collapse-button");
            VisualElement buttonParent = collapseButton.parent;
            collapseButton.Delete();

            MakeChoiceToggle(blockNode, isChoice, buttonParent);

            SetupTitle(title);
            SetupLine(line);

            RefreshExpandedState();
        }

        protected virtual void MakeChoiceToggle(GraphBlockNode blockNode, bool isChoice, VisualElement buttonParent)
        {
            Label choiceLabel = new Label("Choice");
            choiceLabel.AddToClassList("toggle-label");

            ChoiceToggle = new Toggle();
            ChoiceToggle.Clear();
            ChoiceToggle.AddToClassList("choice-toggle");

            ChoiceToggle.RegisterValueChangedCallback(ToggleChoiceChange);

            ChoiceToggle.value = isChoice;

            ChoiceToggle.Add(choiceLabel);
            buttonParent.Add(ChoiceToggle);

            if (isChoice)
            {
                BlockNodeBorderRenderer blockBorder = blockNode.Q<BlockNodeBorderRenderer>("node-border");

                UnityEngine.Color blueColor = new UnityEngine.Color(0.13f, 0.41f, 0.52f);

                blockBorder.StartColor = blueColor;
                blockBorder.EndColor = blueColor;

                foreach (NodePort port in blockNode.Ports)
                {
                    port.portColor = blueColor;
                }

                choiceLabel.style.color = new UnityEngine.Color(0.82f, 0.82f, 0.82f);
            }

            void ToggleChoiceChange(ChangeEvent<bool> e)
            {
                UnityEngine.Color colorA;
                UnityEngine.Color colorB;

                if (e.newValue)
                {
                    colorA = colorB = new UnityEngine.Color(0.13f, 0.41f, 0.52f);

                    choiceLabel.style.color = new UnityEngine.Color(0.82f, 0.82f, 0.82f);
                }
                else
                {
                    colorA = blockNode.StartColor;
                    colorB = blockNode.EndColor;

                    choiceLabel.style.color = new UnityEngine.Color(0.52f, 0.52f, 0.52f);
                }

                blockNode.BorderRenderer.StartColor = colorA;
                blockNode.BorderRenderer.EndColor = colorB;

                foreach (NodePort port in blockNode.Inputs)
                {
                    port.portColor = colorA;

                    foreach (var edge in port.connections)
                    {
                        edge.UpdateEdgeControl();
                    }
                }

                foreach (NodePort port in blockNode.Outputs)
                {
                    port.portColor = colorB;

                    foreach (var edge in port.connections)
                    {
                        edge.UpdateEdgeControl();
                    }
                }

                DialogueEditorWindow.RegisterUndo("Toggle Choice");
            }
        }

        protected virtual void SetupTitle(string title)
        {
            TitleField = new TextField();
            TitleField.SetValueWithoutNotify(title);
            TitleField.RegisterCallback<FocusOutEvent>(e =>
            {
                DialogueEditorWindow.RegisterUndo("Change Node Title");
            });

            contentContainer.Add(TitleField);

            VisualElement titleToRemove = this.Q("title-label");
            titleToRemove.parent.Insert(0, TitleField);
            titleToRemove.parent.Remove(titleToRemove);

            TitleField.name = "title-label";
            TitleField.Q("unity-text-input").AddToClassList("borderless-text-input");
            TitleField.Q("unity-text-input").style.fontSize = 16;
        }

        protected virtual void SetupLine(string line)
        {
            Label lineLabel = new Label($"Line ({line.Length} chars) :");
            lineLabel.AddToClassList("line-label");

            contentContainer.Add(lineLabel);

            LineField = new TextField();
            LineField.Q("unity-text-input").AddToClassList("width-text-input");
            LineField.Q("unity-text-input").style.fontSize = 14;
            LineField.SetValueWithoutNotify(line);
            LineField.RegisterValueChangedCallback(e => { lineLabel.text = $"Line ({e.newValue.Length} chars) :"; });
            LineField.RegisterCallback<FocusOutEvent>(e =>
            {
                DialogueEditorWindow.RegisterUndo("Change Node Line");
            });

            contentContainer.Add(LineField);
        }

        public void RefreshExpandedState()
        {
            contentContainer.style.display = this.Q("dividerHorizontal").style.display = contentContainer.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}