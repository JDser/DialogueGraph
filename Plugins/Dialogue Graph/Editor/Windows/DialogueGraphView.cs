using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor
{
    using DialogueEditor.Nodes;
    using DialogueEditor.Properties;

    public delegate void HighlightNodesEvent(ColorMode colorMode);

    public class DialogueGraphView : GraphView
    {
        /* Non-Serializable */

        public event HighlightNodesEvent OnHighlight;

        private readonly Dictionary<Guid, GraphElement> m_GraphElements;

        private Vector2 m_mousePosition;

        /* Properties */

        public new Dictionary<Guid, GraphElement> graphElements => m_GraphElements;

        public DialogueEditorWindow EditorWindow { get; private set; }

        public SearchWindowProvider SearchWindowProvider { get; set; }

        public ColorMode CurrentColorMode { get; private set; }

        //public GraphBlackboard Blackboard { get; private set; }

        //public GraphInspector Inspector { get; private set; }

        public Vector2 LocalMousePosition
        {
            get => viewTransform.matrix.inverse.MultiplyPoint(m_mousePosition);
            set => m_mousePosition = value;
        }

        protected override bool canCopySelection => selection.Any(s => s is GraphElement element && element.IsCopiable());

        protected override bool canCutSelection => selection.Any(s => s is GraphElement element && element.IsCopiable() && element.IsDeletable());

        protected override bool canDuplicateSelection => canCopySelection;

        protected override bool canDeleteSelection => selection.Any(s => s is GraphElement element && element.IsDeletable());

        public bool HasSelection => selection.Count > 0;

        /* Initialization */
        public DialogueGraphView(DialogueEditorWindow editor, GraphUndoObject graphData)
        {
            EditorWindow = editor;
            m_GraphElements = new Dictionary<Guid, GraphElement>();

            AddManipulators();
            AddCallbacks();
            SetupWindowElements();

            AddBlackboardContext();
            AddSearchWindow();

            SerializedGraphData wrapper = graphData.Wrapper;
            if (wrapper != null)
            {
                Deserialize(wrapper);
            }
            else
            {
                CurrentColorMode = ColorMode.None;

                Vector2 position = new Vector2(EditorWindow.position.width / 2, EditorWindow.position.height / 2);
                AddGraphElement(new GraphStartNode(), position);
            }

            AddToolbar();

            EditorWindow.rootVisualElement.Add(this);

            this.StretchToParentSize();
        }

        private void AddManipulators()
        {
            SetupZoom(0.125f, 8);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddCallbacks()
        {
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<DragUpdatedEvent>(DragUpdatedEvent);
            RegisterCallback<DragPerformEvent>(DragPerformEvent);

            graphViewChanged = GraphViewChange;
            serializeGraphElements = SerializeElements;
            canPasteSerializedData = CanPasteSerialized;
            unserializeAndPaste = UnserializeAndPaste;
            deleteSelection = DeleteSelectionCallback;
        }

        private void SetupWindowElements()
        {
            /* AddGrid */
            GridBackground grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);

            ///* AddBlackboard */
            //Blackboard = new GraphBlackboard(this, EditorWindow.ContextName, EditorWindow.WindowName);
            //Blackboard.addItemRequested = (callback) => (callback as GraphBlackboard).ShowContextMenu();
            //Add(Blackboard);

            ///* AddInspector */
            //Inspector = new GraphInspector(this, "Graph Inspector");
            //Add(Inspector);
        }

        private void AddToolbar()
        {
            Toolbar m_Toolbar = new Toolbar();

            ToolbarButton m_SaveAsset = new ToolbarButton(EditorWindow.SaveChanges);
            m_SaveAsset.style.unityTextAlign = TextAnchor.MiddleLeft;
            m_SaveAsset.text = "Save Asset";

            m_Toolbar.Add(m_SaveAsset);

            ToolbarSpacer m_FirstSpacer = new ToolbarSpacer();
            m_FirstSpacer.style.width = 5;
            m_Toolbar.Add(m_FirstSpacer);

            ToolbarButton m_SaveAs = new ToolbarButton(EditorWindow.SaveAsNewAsset);
            m_SaveAs.style.unityTextAlign = TextAnchor.MiddleLeft;
            m_SaveAs.text = "Save As...";

            m_Toolbar.Add(m_SaveAs);

            ToolbarSpacer m_SecondSpacer = new ToolbarSpacer();
            m_SecondSpacer.style.width = 5;
            m_Toolbar.Add(m_SecondSpacer);

            ToolbarButton m_ShowInProject = new ToolbarButton(EditorWindow.ShowAsset);
            m_ShowInProject.style.unityTextAlign = TextAnchor.MiddleLeft;
            m_ShowInProject.text = "Show In Project";

            m_Toolbar.Add(m_ShowInProject);

            ToolbarSpacer m_FlexSpacer = new ToolbarSpacer();
            m_FlexSpacer.style.flexGrow = 1f;
            m_Toolbar.Add(m_FlexSpacer);

            Label m_PopupLabel = new Label("Color Mode");
            m_PopupLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            m_Toolbar.Add(m_PopupLabel);

            PopupField<ColorMode> m_ColorPopupField = new PopupField<ColorMode>(new List<ColorMode>()
                { ColorMode.None, ColorMode.Category, ColorMode.UserDefined },
                CurrentColorMode,
                FormatSelectedValue,
                FormatListItem);

            m_ColorPopupField.RegisterValueChangedCallback(ColorPopupValueChange);

            m_ColorPopupField.style.width = 100;
            m_Toolbar.Add(m_ColorPopupField);

            //ToolbarToggle m_BlackboardToggle = new ToolbarToggle();
            //m_BlackboardToggle.text = "Blackboard";
            //m_BlackboardToggle.SetValueWithoutNotify(Blackboard.Visible);
            //m_BlackboardToggle.RegisterValueChangedCallback(e => { Blackboard.Visible = e.newValue; });
            //m_Toolbar.Add(m_BlackboardToggle);

            //ToolbarSpacer m_ThirdSpacer = new ToolbarSpacer();
            //m_ThirdSpacer.style.width = 5;
            //m_Toolbar.Add(m_ThirdSpacer);

            //ToolbarToggle m_InspectorToggle = new ToolbarToggle();
            //m_InspectorToggle.text = "Graph Inspector";
            //m_InspectorToggle.SetValueWithoutNotify(Inspector.Visible);
            //m_InspectorToggle.RegisterValueChangedCallback(e => { Inspector.Visible = e.newValue; });
            //m_Toolbar.Add(m_InspectorToggle);

            Add(m_Toolbar);
            m_Toolbar.SetEnabled(true);


            string FormatSelectedValue(ColorMode mode)
            {
                if (mode == ColorMode.None) return "<None>";
                else if (mode == ColorMode.UserDefined) return "User Defined";
                else return "Category";
            }

            string FormatListItem(ColorMode mode)
            {
                if (mode == ColorMode.None) return "<None>";
                else if (mode == ColorMode.UserDefined) return "User Defined";
                else return "Category";
            }

            void ColorPopupValueChange(ChangeEvent<ColorMode> e)
            {
                CurrentColorMode = e.newValue;
                OnHighlight(CurrentColorMode);
            }
        }

        private void AddBlackboardContext()
        {
            //Type abstractProperty = typeof(GraphAbstractProperty);
            //Type[] types = DialogueEditorUtility.GetTypesInNamespace(abstractProperty, abstractProperty.Namespace);

            //for (int i = 0; i < types.Length; i++)
            //{
            //    Type fieldType = types[i];

            //    PropertyFieldAttribute prop = fieldType.GetCustomAttribute<PropertyFieldAttribute>();
            //    if (prop == null)
            //        continue;

            //    GraphTypeManager.AddPropertyType(prop.PropertyType, prop.GraphNodeType);
            //    ColorSpaceManager.AddPropertyColor(prop.PropertyType, prop.Color);

            //    Blackboard.AddContext(prop.Name, () =>
            //    {
            //        /* Undo shoots after FocusOut event (closing field's textEditor) */
            //        GraphAbstractProperty field = DialogueEditorUtility.Create<GraphAbstractProperty>(fieldType);
            //        AddPropertyField(field);
            //        field.OpenTextEditor();
            //    });
            //}
        }

        private void AddSearchWindow()
        {
            m_CachedNodeTypes = DialogueEditorUtility.CacheSearchEntries<GraphAbstractNode>();
            m_CachedPropertyTypes = DialogueEditorUtility.CacheSearchEntries<AbstractBlockProperty>();

            SearchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            SearchWindowProvider.Initialize(this);

            nodeCreationRequest = OnNodeCreationRequest;
        }

        /* Callbacks */

        #region Search Window Provider
        public NodePort connectedPort { get; set; }

        private SearchWindowProvider.CacheType[] m_CachedNodeTypes;

        private SearchWindowProvider.CacheType[] m_CachedPropertyTypes;

        private void OnNodeCreationRequest(NodeCreationContext context)
        {
            if (selection.Any(e => e is GraphBlockNode || e is AbstractBlockProperty))
            {
                OpenPropertySearchWindow(null);
            }
            else
            {
                OpenNodeSearchWindow(null);
            }
        }

        public void OpenNodeSearchWindow(NodePort port)
        {
            connectedPort = port;
            SearchWindowProvider.SearchTree = CreateNodeTree();
            SearchWindow.Open(new SearchWindowContext(m_mousePosition), SearchWindowProvider);
        }

        private void OpenPropertySearchWindow(DropdownMenuAction action)
        {
            connectedPort = null; /* Don't forget to clean port from which could be dragged previously */
            SearchWindowProvider.SearchTree = CreatePropertyTree();
            SearchWindow.Open(new SearchWindowContext(m_mousePosition), SearchWindowProvider);
        }

        private List<SearchTreeEntry> CreateNodeTree()
        {
            List<SearchWindowProvider.NodeEntry> entries = new List<SearchWindowProvider.NodeEntry>();

            // Scan assembly namespace for available nodes
            for (int i = 0; i < m_CachedNodeTypes.Length; i++)
            {
                entries.Add(new SearchWindowProvider.NodeEntry()
                {
                    title = m_CachedNodeTypes[i].attribute.titles,
                    order = m_CachedNodeTypes[i].attribute.order,
                    type = m_CachedNodeTypes[i].type,
                });
            }

            //// Fields
            //foreach (GraphAbstractProperty item in GraphElements.Values.OfType<GraphAbstractProperty>())
            //{
            //    PropertyFieldAttribute propertyAttribute = item.GetType().GetCustomAttribute<PropertyFieldAttribute>();
            //    if (propertyAttribute == null)
            //        continue;

            //    entries.Add(new SearchWindowProvider.NodeEntry()
            //    {
            //        title = new string[] { "Properties", $"Property : {item.PropertyTitle}" },
            //        type = propertyAttribute.GraphNodeType,
            //        data = item, //GraphAbstractPropertyField
            //    });
            //}

            //// Basic Fields
            //entries.Add(new SearchWindowProvider.NodeEntry()
            //{
            //    title = new string[] { "Properties", "Basic", "Boolean" },
            //    type = typeof(GraphInlinePropertyNode),
            //    data = typeof(GraphBooleanField) //Type
            //});

            //entries.Add(new SearchWindowProvider.NodeEntry()
            //{
            //    title = new string[] { "Properties", "Basic", "Float" },
            //    type = typeof(GraphInlinePropertyNode),
            //    data = typeof(GraphFloatField) //Type
            //});

            //entries.Add(new SearchWindowProvider.NodeEntry()
            //{
            //    title = new string[] { "Properties", "Basic", "Integer" },
            //    type = typeof(GraphInlinePropertyNode),
            //    data = typeof(GraphIntegerField) //Type
            //});

            //entries.Add(new SearchWindowProvider.NodeEntry()
            //{
            //    title = new string[] { "Properties", "Basic", "String" },
            //    type = typeof(GraphInlinePropertyNode),
            //    data = typeof(GraphStringField) //Type
            //});

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group 
            entries.Sort((entry1, entry2) =>
            {
                //Sort entries with same paths by order
                if (entry1.title.Length > 1 && entry1.title.Length == entry2.title.Length)
                {
                    int preLastIndex = entry1.title.Length - 2;
                    if (entry1.title[preLastIndex] == entry2.title[preLastIndex])
                    {
                        if (entry1.order < entry2.order)
                            return -1;
                        else
                            return 1;
                    }
                }

                for (int i = 0; i < entry1.title.Length; i++)
                {
                    if (i >= entry2.title.Length)
                        return 1;
                    int value = entry1.title[i].CompareTo(entry2.title[i]);
                    if (value != 0)
                    {
                        if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                            return entry1.title.Length < entry2.title.Length ? -1 : 1;
                        return value;
                    }
                }

                return 0;
            });

            // Making actual tree
            List<SearchTreeEntry> tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Node"), 0), };

            List<string> groups = new List<string>();

            for (int x = 0; x < entries.Count; x++)
            {
                SearchWindowProvider.NodeEntry nodeEntry = entries[x];
                int createIndex = int.MaxValue;

                for (int i = 0; i < nodeEntry.title.Length; i++)
                {
                    string group = nodeEntry.title[i];
                    if (i >= groups.Count)
                    {
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                for (int i = createIndex; i < nodeEntry.title.Length - 1; i++)
                {
                    string group = nodeEntry.title[i];
                    groups.Add(group);

                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(nodeEntry.title.Last(), SearchWindowProvider.Icon))
                {
                    level = nodeEntry.title.Length,
                    userData = nodeEntry
                };

                tree.Add(searchTreeEntry);
            }

            return tree;
        }

        private List<SearchTreeEntry> CreatePropertyTree()
        {
            List<SearchWindowProvider.NodeEntry> entries = new List<SearchWindowProvider.NodeEntry>();

            // Scan assembly namespace for available nodes
            for (int i = 0; i < m_CachedPropertyTypes.Length; i++)
            {
                entries.Add(new SearchWindowProvider.NodeEntry()
                {
                    title = m_CachedPropertyTypes[i].attribute.titles,
                    order = m_CachedPropertyTypes[i].attribute.order,
                    type = m_CachedPropertyTypes[i].type,
                });
            }

            // Sort the entries lexicographically by group then title with the requirement that items always comes before sub-groups in the same group 
            entries.Sort((entry1, entry2) =>
            {
                //Sort entries with same paths by order
                if (entry1.title.Length > 1 && entry1.title.Length == entry2.title.Length)
                {
                    int preLastIndex = entry1.title.Length - 2;
                    if (entry1.title[preLastIndex] == entry2.title[preLastIndex])
                    {
                        if (entry1.order < entry2.order)
                            return -1;
                        else
                            return 1;
                    }
                }

                for (int i = 0; i < entry1.title.Length; i++)
                {
                    if (i >= entry2.title.Length)
                        return 1;
                    int value = entry1.title[i].CompareTo(entry2.title[i]);
                    if (value != 0)
                    {
                        if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                            return entry1.title.Length < entry2.title.Length ? -1 : 1;
                        return value;
                    }
                }

                return 0;
            });

            // Making actual tree
            List<SearchTreeEntry> tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Node"), 0), };

            List<string> groups = new List<string>();

            for (int x = 0; x < entries.Count; x++)
            {
                SearchWindowProvider.NodeEntry nodeEntry = entries[x];
                int createIndex = int.MaxValue;

                for (int i = 0; i < nodeEntry.title.Length; i++)
                {
                    string group = nodeEntry.title[i];
                    if (i >= groups.Count)
                    {
                        createIndex = i;
                        break;
                    }
                    if (groups[i] != group)
                    {
                        groups.RemoveRange(i, groups.Count - i);
                        createIndex = i;
                        break;
                    }
                }

                for (int i = createIndex; i < nodeEntry.title.Length - 1; i++)
                {
                    string group = nodeEntry.title[i];
                    groups.Add(group);

                    tree.Add(new SearchTreeGroupEntry(new GUIContent(group)) { level = i + 1 });
                }

                SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(nodeEntry.title.Last(), SearchWindowProvider.Icon))
                {
                    level = nodeEntry.title.Length,
                    userData = nodeEntry
                };

                tree.Add(searchTreeEntry);
            }

            return tree;
        }

        public void OnSearchSelectedEntry(SearchWindowProvider.NodeEntry nodeEntry)
        {
            GraphElement createdElement = DialogueEditorUtility.Create<GraphElement>(nodeEntry.type);

            if (createdElement is GraphAbstractNode abstractNode)
            {
                //if (createdElement is GraphPropertyNode propertyNode)
                //{
                //    (nodeEntry.data as GraphAbstractProperty).AddNode(propertyNode);
                //}
                //else if (createdElement is GraphInlinePropertyNode inlinePropertyNode)
                //{
                //    inlinePropertyNode.Setup(nodeEntry.data as Type);
                //}

                AddGraphElement(abstractNode, LocalMousePosition);

                if (connectedPort != null)
                {
                    NodePort otherPort = abstractNode.GetCompatiblePort(connectedPort);

                    if (otherPort != null)
                    {
                        Connect(connectedPort, otherPort);
                    }

                    connectedPort = null;
                }

                DialogueEditorWindow.RegisterUndo($"Add {abstractNode.Title}");
                return;
            }
            else if (createdElement is AbstractBlockProperty property)
            {
                GraphBlockNode blockNode = null;

                ISelectable selectable = selection.LastOrDefault(e => e is GraphBlockNode || e is AbstractBlockProperty);
                if (selectable == null)
                {
                    Debug.LogError("Selectable is null!");
                    return;
                }
                else if (selectable is GraphBlockNode block)
                {
                    blockNode = block;
                }
                else if (selectable is AbstractBlockProperty blockProperty)
                {
                    blockNode = blockProperty.GetFirstAncestorOfType<GraphBlockNode>();
                }
                else
                {
                    Debug.LogError("Could not find parent blockNode!");
                    return;
                }

                property.BlockNodeGuid = blockNode.Guid;

                AddGraphElement(property, Vector2.zero);

                DialogueEditorWindow.RegisterUndo($"Add {property.Title}");
                return;
            }
        }
        #endregion

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (evt.target is DialogueGraphView)
            {
                evt.menu.InsertAction(0, "Create Block Node", CreateBlockNodeAction);
                //evt.menu.action[1]  is Create Node option now
                evt.menu.InsertAction(2, "Create Sticky Note", CreateStickyNoteAction);
            }

            /* Sticky notes */
            if (evt.target is GraphStickyNote)
            {
                evt.menu.AppendAction("Cut", callback => CutSelectionCallback(), canCutSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Copy", callback => CopySelectionCallback(), canCopySelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Delete", callback => DeleteSelectionCallback(AskUser.DontAskUser), canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Duplicate", callback => DuplicateSelectionCallback(), canDuplicateSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();
            }

            if (evt.target is GraphAbstractNode)
            {
                if (evt.target is GraphBlockNode)
                {
                    evt.menu.InsertAction(0, "Create Property", OpenPropertySearchWindow);
                    evt.menu.InsertSeparator(null, 1);

                    evt.menu.InsertAction(2, "Toggle Choice", ToggleChoiceAction);
                    evt.menu.InsertSeparator(null, 3);
                }

                //if (evt.target is GraphPropertyNode || evt.target is GraphInlinePropertyNode)
                //{
                //    evt.menu.AppendAction("Convert To/Inline Node", ConvertToInlineNodeAction, InlineNodeStatus);
                //    evt.menu.AppendAction("Convert To/Property", ConvertToPropertyAction, PropertyStatus);
                //    evt.menu.AppendSeparator();
                //}
            }

            if (CurrentColorMode == ColorMode.UserDefined && evt.target is IColorable && !(evt.target is GraphBlockNode))
            {
                evt.menu.AppendAction("Color/Change", ChangeColorAction);

                evt.menu.AppendAction("Color/Reset", ResetColorAction);
                evt.menu.AppendSeparator();
            }

            if (evt.target is IGroupable)
            {
                /* Group Elements */
                evt.menu.AppendAction("Group Selection %g", GroupAction);

                /* Ungroup Elements */
                evt.menu.AppendAction("Ungroup Selection %u", UngroupAction, UngroupStatus);
            }

            if (evt.target is GraphGroup)
            {
                evt.menu.AppendAction("Delete Group and Contents", DeleteGroupAndContentsAction);
                evt.menu.AppendSeparator();
            }

            if (evt.target is Edge flowEdge)
            {
                if (flowEdge.input.node is GraphBlockNode inputNode && flowEdge.output.node is GraphBlockNode outputNode)
                {
                    AppendPriorityActions(evt, flowEdge);

                    evt.menu.AppendAction("Focus/Input node",
                        e =>
                        {
                            ClearSelection();
                            AddToSelection(inputNode);
                            FrameSelection();
                        });

                    evt.menu.AppendAction("Focus/Output node",
                        e =>
                        {
                            ClearSelection();
                            AddToSelection(outputNode);
                            FrameSelection();
                        });

                }
            }
        }

        #region ContextualMenu Actions & Statuses
        private void CreateBlockNodeAction(DropdownMenuAction callback)
        {
            AddGraphElement(new GraphBlockNode(), LocalMousePosition - new Vector2(160, 30)); // Half the width of the element and some height offset
            DialogueEditorWindow.RegisterUndo("Add GraphBlockNode");
        }

        private void CreateStickyNoteAction(DropdownMenuAction callback)
        {
            GraphStickyNote note = new GraphStickyNote();

            note.title = "New Note";
            note.contents = "Write something here";

            AddGraphElement(note, LocalMousePosition);

            DialogueEditorWindow.RegisterUndo("Add Sticky Note");
        }

        private void ToggleChoiceAction(DropdownMenuAction callback)
        {
            foreach (GraphBlockNode node in selection.OfType<GraphBlockNode>())
            {
                node.ToggleChoice();
            }
        }

        //private void ConvertToInlineNodeAction(DropdownMenuAction callback)
        //{
        //    List<GraphPropertyNode> propertyNodes = selection.OfType<GraphPropertyNode>().ToList();

        //    for (int i = 0; i < propertyNodes.Count; i++)
        //    {
        //        GraphPropertyNode node = propertyNodes[i];

        //        if (!node.AssociatedField.IsConvertable())
        //        {
        //            i--;
        //            propertyNodes.Remove(node);
        //            continue;
        //        }

        //        GraphInlinePropertyNode inlineNode = new GraphInlinePropertyNode();
        //        inlineNode.GraphConnections = node.GraphConnections;

        //        inlineNode.SerializedValue = node.AssociatedField.CurrentValue;
        //        inlineNode.Setup(node.AssociatedField.GetType());

        //        AddGraphElement(inlineNode, node.Position);
        //    }

        //    DeleteElements(propertyNodes);
        //    EditorWindow.RegisterUndo("Convert To Inline Node");
        //}

        //private DropdownMenuAction.Status InlineNodeStatus(DropdownMenuAction callback)
        //{
        //    if (selection.OfType<GraphPropertyNode>().Any())
        //        return DropdownMenuAction.Status.Normal;
        //    return DropdownMenuAction.Status.Disabled;
        //}

        //private void ConvertToPropertyAction(DropdownMenuAction callback)
        //{
        //    IEnumerable<GraphInlinePropertyNode> inlineNodes = selection.OfType<GraphInlinePropertyNode>();
        //    foreach (var node in inlineNodes)
        //    {
        //        GraphAbstractProperty field = DialogueEditorUtility.Create<GraphAbstractProperty>(node.PropertyFieldType);

        //        AddPropertyField(field);
        //        field.SetValue(node);

        //        GraphPropertyNode propertyNode = new GraphPropertyNode();
        //        propertyNode.GraphConnections = node.GraphConnections;

        //        AddGraphElement(propertyNode, node.Position);

        //        //GraphPropertyNode validates from graphProperty
        //        field.AddNode(propertyNode);
        //    }

        //    DeleteElements(inlineNodes);
        //    EditorWindow.RegisterUndo("Convert To Property");
        //}

        //private DropdownMenuAction.Status PropertyStatus(DropdownMenuAction callback)
        //{
        //    if (selection.OfType<GraphInlinePropertyNode>().Any())
        //        return DropdownMenuAction.Status.Normal;
        //    return DropdownMenuAction.Status.Disabled;
        //}

        private void ChangeColorAction(DropdownMenuAction callback)
        {
            /* Get ColorPicker */
            var t = typeof(EditorWindow).Assembly.GetTypes().FirstOrDefault(ty => ty.Name == "ColorPicker");
            var m = t?.GetMethod("Show", new[] { typeof(Action<Color>), typeof(Color), typeof(bool), typeof(bool) });
            if (m == null)
                return;

            if (!(selection.FirstOrDefault(e => e is IColorable) is IColorable colorable)) return;

            Color defaultColor = colorable.UserDefinedColor;
            defaultColor.a = 1f;

            m.Invoke(null, new object[] { (Action<Color>)ApplyColor, defaultColor, true, false });


            void ApplyColor(Color color)
            {
                foreach (var item in selection)
                {
                    if (item is IColorable colorable)
                    {
                        colorable.SetUserDefinedColor(color);
                        DialogueEditorWindow.RegisterUndo("Change Node Color"); // Bottleneck
                    }
                }
            }
        }

        private void ResetColorAction(DropdownMenuAction callback)
        {
            foreach (var item in selection)
            {
                if (item is IColorable colorable)
                {
                    colorable.ResetUserDefinedColor();
                }
            }

            DialogueEditorWindow.RegisterUndo("Reset Node Color");
        }

        private void GroupAction(DropdownMenuAction callback)
        {
            GraphGroup group = new GraphGroup();

            group.title = "New Group";

            AddGraphElement(group, LocalMousePosition);

            for (int i = 0; i < selection.Count; i++)
            {
                GraphElement element = selection[i] as GraphElement;
                if (element != null && element.IsGroupable())
                {
                    group.AddElement(element);
                }
            }

            group.FocusTitleTextField();

            DialogueEditorWindow.RegisterUndo("Add Group");
        }

        private void UngroupAction(DropdownMenuAction callback)
        {
            for (int i = 0; i < selection.Count; i++)
            {
                if (selection[i] is IGroupable element)
                {
                    element.Group.RemoveElement(selection[i] as GraphElement);
                    element.Group = null;
                }
            }
            DialogueEditorWindow.RegisterUndo("Ungroup elements");
        }

        private DropdownMenuAction.Status UngroupStatus(DropdownMenuAction callback)
        {
            bool hasUngroupable = selection.OfType<IGroupable>().Any(x => x.Group != null);
            return hasUngroupable ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }

        private void DeleteGroupAndContentsAction(DropdownMenuAction callback)
        {
            GraphGroup group = selection.OfType<GraphGroup>().First();
            if (group == null)
                return;

            List<GraphElement> elementsToDelete = new List<GraphElement>(group.containedElements);
            elementsToDelete.Add(group);

            DeleteElements(elementsToDelete);
        }

        private void AppendPriorityActions(ContextualMenuPopulateEvent evt, Edge flowEdge)
        {
            DialogueSystem.ConnectionPriority priority = flowEdge.userData != null ?
           (DialogueSystem.ConnectionPriority)flowEdge.userData :
            DialogueSystem.ConnectionPriority.Medium;

            void SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority priority)
            {
                foreach (var edge in selection.OfType<Edge>())
                {
                    if (edge.input.node is GraphBlockNode && edge.output.node is GraphBlockNode)
                    {
                        edge.SetPriority(priority);
                    }
                }
            }

            evt.menu.AppendAction("Priority/Very Low",
                e =>
                {
                    SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority.VeryLow);
                    DialogueEditorWindow.RegisterUndo("Set Edge Priority Very Low");
                },
                priority == DialogueSystem.ConnectionPriority.VeryLow ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            evt.menu.AppendAction("Priority/Low",
                e =>
                {
                    SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority.Low);
                    DialogueEditorWindow.RegisterUndo("Set Edge Priority Low");
                },
                priority == DialogueSystem.ConnectionPriority.Low ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            evt.menu.AppendAction("Priority/Medium",
                e =>
                {
                    SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority.Medium);
                    DialogueEditorWindow.RegisterUndo("Set Edge Priority Medium");
                },
                priority == DialogueSystem.ConnectionPriority.Medium ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            evt.menu.AppendAction("Priority/High",
                e =>
                {
                    SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority.High);
                    DialogueEditorWindow.RegisterUndo("Set Edge Priority High");
                },
                priority == DialogueSystem.ConnectionPriority.High ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);

            evt.menu.AppendAction("Priority/Very High",
                e =>
                {
                    SetSelectedEdgesPriority(DialogueSystem.ConnectionPriority.VeryHigh);
                    DialogueEditorWindow.RegisterUndo("Set Edge Priority Very High");
                },
                priority == DialogueSystem.ConnectionPriority.VeryHigh ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
        }
        #endregion

        private void DeleteSelectionCallback(string operationName, AskUser askuser)
        {
            DeleteElements(selection.OfType<GraphElement>());
        }

        public new void DeleteElements(IEnumerable<GraphElement> elements)
        {
            List<GraphElement> elementsToDelete = new List<GraphElement>(elements);

            GraphElement tempElement;

            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                tempElement = elementsToDelete[i];

                if (!tempElement.IsDeletable())
                {
                    elementsToDelete.RemoveAt(i);
                    continue;
                }

                if (tempElement is IConnectable connectable)
                {
                    foreach (var port in connectable.Ports)
                    {
                        elementsToDelete.AddRange(port.connections);
                    }
                }
                //else if (tempElement is GraphAbstractProperty field)
                //{
                //    elementsToDelete.AddRange(field.AssociatedNodes);
                //}
            }

            base.DeleteElements(elementsToDelete);
        }

        private GraphViewChange GraphViewChange(GraphViewChange changes)
        {
            if (changes.elementsToRemove != null)
            {
                changes.elementsToRemove.ForEach(OnRemoveElement);
                DialogueEditorWindow.RegisterUndo("Elements deleted");
            }

            if (changes.movedElements != null)
            {
                DialogueEditorWindow.RegisterUndo("Elements moved");
            }

            return changes;
        }

        private void OnRemoveElement(GraphElement element)
        {
            if (element is Edge edge)
            {
                ClearEdge(edge);
                return;
            }

            if (element is IGraphEvents graphEvent)
            {
                graphEvent.Remove();
            }

            if (element is IGraphElement graphElement)
            {
                graphElements.Remove(graphElement.Guid);
                return;
            }
        }

        /* Mouse & Keyboard Inputs & Dragging */
        private void OnKeyDown(KeyDownEvent evt)
        {
            /* Hotkeys */
            switch (evt.keyCode)
            {
                /* Group Selection Ctrl + G */
                case KeyCode.G:
                    if (evt.ctrlKey && HasSelection)
                    {
                        GroupAction(null);
                        break;
                    }
                    break;


                /* Ungroup Selection Ctrl + U */
                case KeyCode.U:
                    if (evt.ctrlKey && HasSelection)
                    {
                        if (UngroupStatus(null) != DropdownMenuAction.Status.Normal)
                            break;

                        UngroupAction(null);
                        break;
                    }
                    break;
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            m_mousePosition = evt.localMousePosition;
        }

        private void DragUpdatedEvent(DragUpdatedEvent evt)
        {
            //if (selection.Any(e => { return e is GraphAbstractProperty; }))
            //{
            //    if (evt.target is GraphAbstractProperty)
            //    {
            //        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            //    }
            //    else if (evt.target is GraphBlackboard)
            //    {
            //        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            //    }
            //    else
            //    {
            //        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            //    }
            //}     

            if (selection.Any(e => { return e is AbstractBlockProperty; }))
            {
                if (evt.target is AbstractBlockProperty || evt.target is GraphBlockNode)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
        }

        private void DragPerformEvent(DragPerformEvent evt)
        {
            if (!HasSelection)
                return;

            //if (evt.target is GraphAbstractProperty || evt.target is GraphBlackboard)
            //    return;

            //LocalMousePosition = evt.localMousePosition;

            //for (int i = 0; i < selection.Count; i++)
            //{
            //    /* Create Property Node from current field */
            //    if (selection[i] is GraphAbstractProperty field)
            //    {
            //        PropertyFieldAttribute propertyAttribute = field.GetType().GetCustomAttribute<PropertyFieldAttribute>();
            //        if (propertyAttribute == null)
            //            continue;

            //        GraphPropertyNode propertyNode = DialogueEditorUtility.Create<GraphPropertyNode>(propertyAttribute.GraphNodeType);
            //        field.AddNode(propertyNode);

            //        AddGraphElement(propertyNode, LocalMousePosition);
            //    }
            //}

            //EditorWindow.RegisterUndo("Property Drop");
        }

        /* Copy/Cut & Paste */
        private string SerializeElements(IEnumerable<GraphElement> elements)
        {
            IEnumerable<GraphElement> serializedElements = elements.Where(x => x.IsCopiable());
            SerializedGraphData wrapper = new SerializedGraphData(serializedElements);
            return JsonUtility.ToJson(wrapper, true);
        }

        private bool CanPasteSerialized(string data)
        {
            return SerializedGraphData.FromJson(data) != null;
        }

        private void UnserializeAndPaste(string operationName, string data)
        {
            var wrapper = SerializedGraphData.FromJson(data);
            if (wrapper == null) return;

            GraphElement[] elements = wrapper.GraphElements.ToArray();

            Vector2 moveDirection = DialogueEditorUtility.CalculateMeanPosition(elements, LocalMousePosition);

            Dictionary<Guid, Guid> remappedGuids = new Dictionary<Guid, Guid>();
            List<IGraphEvents> elementsToValidate = new List<IGraphEvents>();

            GraphElement tempElement;

            /* Adding to Graph */
            for (int i = 0; i < elements.Length; i++)
            {
                tempElement = elements[i];

                if (tempElement is IGraphElement graphElement)
                {
                    Guid newGuid = Guid.NewGuid();
                    remappedGuids.Add(graphElement.Guid, newGuid);

                    graphElement.Guid = newGuid;
                    graphElement.GraphView = this;
                    graphElement.Position += moveDirection;

                    graphElements.Add(graphElement.Guid, tempElement);
                }

                if (tempElement is IGraphEvents graphEvent)
                    elementsToValidate.Add(graphEvent);

                AddElement(tempElement);
            }

            /* Validating */
            for (int i = 0; i < elementsToValidate.Count; i++)
            {
                elementsToValidate[i].RemapConnections(remappedGuids);
                elementsToValidate[i].Validate();
            }

            /* Adding to selection */
            ClearSelection();
            for (int i = 0; i < elementsToValidate.Count; i++)
            {
                AddToSelection(elementsToValidate[i] as ISelectable);
            }

            DialogueEditorWindow.RegisterUndo("Elements pasted");
        }

        /* Add/Remove GraphElements */
        public void AddGraphElement(IGraphElement element, Vector2 position)
        {
            element.Guid = Guid.NewGuid();
            element.GraphView = this;
            element.Position = position;

            AddElement(element as GraphElement);
            graphElements.Add(element.Guid, element as GraphElement);

            if (element is IGraphEvents graphEvent)
                graphEvent.Validate();
        }

        //public void AddPropertyField(GraphAbstractField field)
        //{
        //    field.Guid = Guid.NewGuid();
        //    field.GraphView = this;
        //    field.Validate();
        //    GraphElements.Add(field.Guid, field);
        //}

        /* Ports & Edges */
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            NodePort start = startPort as NodePort;

            ports.ForEach(other =>
            {
                if (start.IsCompatibleWith(other as NodePort))
                    compatiblePorts.Add(other);
            });

            return compatiblePorts;
        }

        public Edge Connect(NodePort outputPort, NodePort inputPort)
        {
            Edge edge = outputPort.ConnectTo(inputPort);
            AddElement(edge);

            outputPort.AddConnection(inputPort);
            inputPort.AddConnection(outputPort);

            //inputPort.GraphNode?.OnPortConnect(outputPort, inputPort);

            inputPort.Owner.RefreshPorts();

            return edge;
        }

        private void ClearEdge(Edge edge)
        {
            NodePort outputPort = (NodePort)edge.output;
            NodePort inputPort = (NodePort)edge.input;

            outputPort.RemoveConnection(inputPort);
            inputPort.RemoveConnection(outputPort);

            //inputPort.GraphNode?.OnPortDisconnect(outputPort, inputPort);
        }

        /* Serialization */
        public SerializedGraphData Serialize()
        {
            //return new SerializedGraphData(CurrentColorMode, Blackboard.Serialize(), Inspector.Serialize(), GraphElements.Values);
            return new SerializedGraphData(CurrentColorMode, new FloatWindowData(), new FloatWindowData(), graphElements.Values);
        }

        public void Deserialize(SerializedGraphData wrapper)
        {
            CurrentColorMode = wrapper.ColorMode;

            foreach (var element in wrapper.GraphElements)
            {
                AddElement(element);

                if (element is IGraphElement graphElement)
                {
                    graphElement.GraphView = this;
                    graphElements.Add(graphElement.Guid, element);
                }
            }

            List<IGraphEvents> toValidate = graphElements.Values.OfType<IGraphEvents>().ToList(); //Allows to modify GraphElements collection
            foreach (var item in toValidate)
            {
                item.Validate();
            }

            //Blackboard.Deserialize(wrapper.BlackboardData);
            //Inspector.Deserialize(wrapper.InspectorData);

            //Blackboard.Sort();
        }

        public void ClearGraph()
        {
            /* RemoveGraphElements */
            List<GraphElement> selectedElements = new List<GraphElement>(graphElements.Values);
            for (int i = 0; i < selectedElements.Count; i++)
            {
                RemoveElement(selectedElements[i]);
            }
            edges.ForEach(edge => RemoveElement(edge));

            //Blackboard.ClearContent();
            //Inspector.ClearContent();

            graphElements.Clear();

            ClearSelection();
        }

        /* Asset compiling */
        public void Save()
        {
            DialogueSystem.DialogueAsset context = EditorWindow.Context;

            /* Gathering objects to add */
            List<UnityEngine.Object> objectsToAdd = new List<UnityEngine.Object>();
            foreach (var item in graphElements.Values)
            {
                if (item is IHasObjectsToAdd objToWrite)
                {
                    objectsToAdd.AddRange(objToWrite.GetObjects());
                }
            }

            /* Deleting unwanted objects */
            UnityEngine.Object[] objectsToDestroy = AssetDatabase.LoadAllAssetsAtPath(EditorWindow.ContextPath).Where(e => e != EditorWindow.Context).ToArray();
            if (!objectsToDestroy.IsNullOrEmpty())
            {
                for (int i = 0; i < objectsToDestroy.Length; i++)
                {
                    if (objectsToAdd.Contains(objectsToDestroy[i]))
                    {
                        continue;
                    }

                    if (objectsToDestroy[i].IsSubAssetOf(context))
                    {
                        UnityEngine.Object.DestroyImmediate(objectsToDestroy[i], true);
                    }
                }
            }

            /* Adding new objects to asset */
            context.AddObjects(objectsToAdd);
            context.SortObjects(objectsToAdd);

            /* Serialize Graph */
            /* The serialising of the graph must be done after adding nodes to the main asset */
            context.SerializedGraph = JsonUtility.ToJson(Serialize());

            /* Compiling */
            foreach (var item in graphElements.Values.OfType<ICompilable>())
            {
                item.Compile();
            }
        }
    }
}