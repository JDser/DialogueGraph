using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DialogueEditor
{
    public static class DialogueResourceManager
    {
        /* UXML Files */

        public const string BlockNodeUXML = "Assets/Plugins/Dialogue Graph/Editor/Resources/uxml/BlockNode.uxml";
        public const string BlockPropertyUXML = "Assets/Plugins/Dialogue Graph/Editor/Resources/uxml/BlockProperty.uxml";
        public const string VirtualConnectionUXML = "Assets/Plugins/Dialogue Graph/Editor/Resources/uxml/VirtualConnection.uxml";
        public const string GraphWindowBaseUXML = "Assets/Plugins/Dialogue Graph/Editor/Resources/uxml/GraphWindowBase.uxml";
        public const string InspectorFieldUXML = "Assets/Plugins/Dialogue Graph/Editor/Resources/uxml/InspectorField.uxml";

        /* StyleSheets */

        public const string AbstractNodeStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/abstractNodeStyles.uss";
        public const string PortStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/portStyles.uss";
        public const string BlockNodeStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/nodeStyles.uss";
        public const string BlockPropertyStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/blockPropertyStyles.uss";
        public const string EdgeStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/edges.uss";
        public const string DragIndicatorStyleSheets = "Assets/Plugins/Dialogue Graph/Editor/Resources/styles/dragIndcator.uss";

        /* Shaders */

        public const string BorderShaderPath = "Assets/Plugins/Dialogue Graph/Editor/Resources/shaders/nodeBorder.shader";

        /* Icons */

        private const string dotPath = "Assets/Plugins/Dialogue Graph/Editor/Resources/icons/dot.png";
        private const string arrowPath = "Assets/Plugins/Dialogue Graph/Editor/Resources/icons/arrow.png";
        private const string arrowLeftPath = "Assets/Plugins/Dialogue Graph/Editor/Resources/icons/arrowLeft.png";
        private const string dialogueIconPath = "Assets/Plugins/Dialogue Graph/Editor/Resources/icons/small_dialogueIcon.png";


        private static Texture2D DotIcon { get; set; }

        private static Texture2D Arrow { get; set; }

        private static Texture2D ArrowLeft { get; set; }

        private static Texture2D DialogueIcon { get; set; }

        public static Texture2D DrawTextureCircle(Color color)
        {
            if (DotIcon == null)
                DotIcon = AssetDatabase.LoadMainAssetAtPath(dotPath) as Texture2D;

            Texture2D texture = new Texture2D(DotIcon.width, DotIcon.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (DotIcon.GetPixel(x, y).a > 0.5f)
                    {
                        texture.SetPixel(x, y, color);
                        continue;
                    }

                    texture.SetPixel(x, y, Color.clear);
                }
            }

            texture.filterMode = FilterMode.Trilinear;
            texture.Apply();

            return texture;
        }

        public static Texture2D GetArrow()
        {
            if (Arrow == null)
                Arrow = AssetDatabase.LoadMainAssetAtPath(arrowPath) as Texture2D;
            return Arrow;
        }

        public static Texture2D GetArrowLeft()
        {
            if (ArrowLeft == null)
                ArrowLeft = AssetDatabase.LoadMainAssetAtPath(arrowLeftPath) as Texture2D;
            return ArrowLeft;
        }

        public static Texture2D GetDialogueIcon()
        {
            if (DialogueIcon == null)
                DialogueIcon = AssetDatabase.LoadMainAssetAtPath(dialogueIconPath) as Texture2D;
            return DialogueIcon;
        }
    }

    public static class DialogueEditorUtility
    {
        public static string RelatePathToAssetFolder(string path)
        {
            return path.Substring(path.LastIndexOf("Assets/"));
        }

        public static T Create<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        public static SearchWindowProvider.CacheType[] CacheSearchEntries<T>()
        {
            List<SearchWindowProvider.CacheType> tempFieldCache = new List<SearchWindowProvider.CacheType>();

            string typeNamespace = typeof(T).Namespace;
            IEnumerable<Type> types = typeof(T).Assembly.GetTypes().Where(t => string.Equals(t.Namespace, typeNamespace, StringComparison.Ordinal));

            foreach (var item in types)
            {
                SearchEntryAttribute attr = item.GetCustomAttribute<SearchEntryAttribute>();
                if (attr != null)
                {
                    tempFieldCache.Add(new SearchWindowProvider.CacheType(item, attr));
                }
            }

            return tempFieldCache.ToArray();
        }

        public static Vector2 CalculateMeanPosition(IEnumerable<GraphElement> elements, Vector2 LocalMousePosition)
        {
            Vector2 moveDirection = Vector2.zero;
            int elementsCount = 0;

            foreach (var item in elements)
            {
                if (item.IsMovable())
                {
                    moveDirection += (item as IGraphElement).Position;
                    elementsCount++;
                }
            }

            moveDirection /= elementsCount;
            moveDirection = LocalMousePosition - moveDirection;
            return moveDirection;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }

    //public static class GraphTypeManager
    //{
    //    private static readonly Dictionary<Type, Type> GraphPropertyTypes = new Dictionary<Type, Type>();
    //
    //    public static void AddPropertyType(Type propertyType, Type graphType)
    //    {
    //        if (propertyType == null) return;
    //
    //        if (GraphPropertyTypes.ContainsKey(propertyType)) return;
    //
    //        GraphPropertyTypes.Add(propertyType, graphType);
    //    }
    //
    //    public static Type GetPropertyType(Type type)
    //    {
    //        if (type == null) return null;
    //
    //        if (GraphPropertyTypes.TryGetValue(type, out Type graphType))
    //            return graphType;
    //
    //        return null;
    //    }
    //}

    /* ColorSpaceManager */
    public enum ColorMode
    {
        None,
        Category,
        UserDefined
    }

    public static class ColorSpaceManager
    {
        /* Category */
        private static Dictionary<string, Color> CategoryColors = new Dictionary<string, Color>
        {
            { "Dialogue", new Color(0.294f, 0.572f, 0.952f) },
            { "Event", new Color(0.796f, 0.188f, 0.133f) },
            { "Property", new Color(0.592f, 0.819f, 0.239f) },
            { "Logic", new Color(0.611f, 0.309f, 1f) },
        };

        public static void AddCategoryColor(string categoryName, Color color)
        {
            if (CategoryColors.ContainsKey(categoryName)) return;

            CategoryColors.Add(categoryName, color);
        }

        public static Color GetCategoryColor(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new Color(0.7f, 0.7f, 0.7f);

            if (CategoryColors.TryGetValue(category, out Color value))
                return value;

            return new Color(0.7f, 0.7f, 0.7f);
        }

        private static Dictionary<Type, Color> PropertyColors = new Dictionary<Type, Color>()
        {
            { typeof(object),   new Color(0.58f,0.5f,0.9f)  },
        };

        public static void AddPropertyColor(Type propertyType, Color color)
        {
            if (propertyType == null)
                return;

            if (PropertyColors.ContainsKey(propertyType))
                return;

            PropertyColors.Add(propertyType, color);
        }

        public static Color GetPropertyColor(Type propertyType)
        {
            if (propertyType == null)
                return new Color(0.7f, 0.7f, 0.7f);

            if (PropertyColors.TryGetValue(propertyType, out Color value))
                return value;

            return new Color(0.7f, 0.7f, 0.7f);
        }
    }

    /* Unity Object Extensions */
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Is this asset is subAsset of other asset?  
        /// </summary>
        public static bool IsSubAssetOf(this Object subAsset, Object mainAsset)
        {
            string subAssetPath = AssetDatabase.GetAssetPath(subAsset);
            /* Asset was created in GraphView and about to be added to context */
            if (string.IsNullOrEmpty(subAssetPath))
                return false;

            return subAssetPath == AssetDatabase.GetAssetPath(mainAsset);
        }

        public static bool IsSubAsset(this Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            /* Path to MainAsset is NOT equal null => asset is SubAsset */
            return !string.IsNullOrEmpty(assetPath);
        }

        public static void AddObject(this Object asset, Object objectToAdd)
        {
            if (objectToAdd.IsSubAsset())
                return;

            AssetDatabase.AddObjectToAsset(objectToAdd, asset);
        }

        public static void AddObjects(this Object asset, IEnumerable<Object> objects)
        {
            foreach (var item in objects)
            {
                asset.AddObject(item);
            }
        }
    }

    /* VisualElement Extensions */
    public static class VisualElementExtensions
    {
        public static void LoadUXMLTree(this VisualElement origin, string assetPath)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
            asset.CloneTree(origin);
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] names)
        {
            foreach (var item in names)
            {
                StyleSheet style = EditorGUIUtility.Load(item) as StyleSheet;
                element.styleSheets.Add(style);
            }
            return element;
        }

        public static VisualElement ReplaceStyle(this VisualElement element, string oldClass, string newClass)
        {
            element.RemoveFromClassList(oldClass);
            element.AddToClassList(newClass);
            return element;
        }

        public static void Delete(this VisualElement element)
        {
            element.parent.Remove(element);
        }
    }

    /* GraphElement Extensions */
    public static class GraphElementExtensions
    {
        public static bool IsDeletable(this GraphElement element)
        {
            return (element.capabilities & Capabilities.Deletable) == Capabilities.Deletable;
        }
    }

    ///* GraphAbstractNode Extensions */
    //public static class GraphAbstractNodeExtensions
    //{
    //    //public static void PropagateDynamicValue(this GraphAbstractNode inputNode, NodePort output, IEnumerable<NodePort> portsToChange)
    //    public static void PropagateDynamicValue(this GraphAbstractNode node, NodePort connectedPort, IEnumerable<NodePort> portsToChange)
    //    {         
    //       // ////IValuePropagation valuePropagation = output.GraphNode as IValuePropagation;
    //       // //IValuePropagation outputNode = output.GraphNode as IValuePropagation;
    //       // //if (outputNode == null || outputNode.GraphProperty == null)
    //       // //    return;
    //       //
    //       // if (connectedPort.GraphNode is not IValuePropagation outputNode || outputNode.GraphProperty == null)
    //       //     return;
    //       //
    //       // if (node is IValuePropagation inputNodePropagation)
    //       //     inputNodePropagation.GraphProperty = outputNode.GraphProperty;
    //       //
    //       // foreach (NodePort port in node.Ports)
    //       // {
    //       //     //// We only want to change 'typeof(object)' ports
    //       //     //if (port.portType != typeof(object))
    //       //     //    continue; 
    //       //     
    //       //     if (!port.IsDynamic)
    //       //         continue;
    //       // 
    //       //     port.portType = connectedPort.portType;
    //       //     port.portColor = connectedPort.portColor;
    //       // 
    //       //     foreach (NodePort otherPort in port.ConnectedPorts)
    //       //     {
    //       //         if (otherPort == connectedPort)
    //       //             continue;
    //       //
    //       //         //make it so that the order of connected ports doesn't matter
    //       //         //otherPort.GraphNode.OnPortConnect(port, otherPort);
    //       //
    //       //         otherPort.GraphNode.PropagateDynamicValue(port, null);
    //       //
    //       //         //if (port.IsInput)
    //       //         //    otherPort.GraphNode.OnPortConnect(otherPort, port);
    //       //         //    //otherPort.GraphNode.PropagateDynamicValue(port);
    //       //         //else
    //       //         //    otherPort.GraphNode.OnPortConnect(port, otherPort);
    //       //         //    //otherPort.GraphNode.PropagateDynamicValue(port);
    //       //     }
    //       // 
    //       //     if (port.IsInput)
    //       //     {
    //       //         //Debug.Log("Set IsCompatible Input");
    //       //         // We dont want to set IsCompatible to input port
    //       // 
    //       //         //Debug.Log("VirtualConnection Set Data");
    //       //         GraphAbstractProperty prop = outputNode.GraphProperty;
    //       //         port.AddVirtualConnection(prop.GetValueField(), prop.SetFieldData, prop.GetFieldData);
    //       // 
    //       //         //port.AddVirtualConnection(outputNode.GetVirtualConnection());
    //       //     }
    //       //     else
    //       //     {
    //       //         //Debug.Log("Set IsCompatible Output");
    //       //         port.IsCompatible = connectedPort.IsCompatible;
    //       //     }
    //       // }

    //        #region
    //        //foreach (var port in portsToChange)
    //        //{
    //        //    if (port.portType != typeof(object))
    //        //        continue;
    //        //
    //        //    if (port.IsDynamicChanged)
    //        //        continue;
    //        //
    //        //    /* Compares if port types are compatible */
    //        //    /* Ignores if ports have the same direction */
    //        //    if (!connectedPort.IsCompatible(port))
    //        //        continue;
    //        //
    //        //    port.IsDynamicChanged = true;
    //        //
    //        //    port.portType = connectedPort.portType;
    //        //    port.portColor = connectedPort.portColor;
    //        //
    //        //    if (port.direction == Direction.Output)
    //        //    {
    //        //        port.IsCompatible = connectedPort.IsCompatible;
    //        //
    //        //        /* Propagate further */
    //        //        foreach (var edge in port.connections)
    //        //        {
    //        //            GraphAbstractNode nextNode = (edge.input as NodePort).GraphNode;
    //        //
    //        //            /* Check for loop */
    //        //            /* Next connected node is the same as previous connected node */
    //        //            if (nextNode == connectedPort.GraphNode)
    //        //                continue;
    //        //
    //        //            /* if this port's node is not IValuePropagationNode it will be ignored  */
    //        //            nextNode.OnPortConnect(port, edge.input as NodePort);
    //        //        }
    //        //
    //        //        continue;
    //        //    }
    //        //
    //        //    /* If node has saved connection => set custom data */
    //        //    //port.AddVirtualConnection(valuePropagation.CreateFieldFunc(), valuePropagation.SetDataFunc, valuePropagation.GetDataFunc);
    //        //
    //        //    //IGraphProperty prop = valuePropagation.GraphProperty;
    //        //    GraphAbstractProperty prop = outputNode.GraphProperty;
    //        //    port.AddVirtualConnection(prop.GetValueField(), prop.SetFieldData, prop.GetFieldData);
    //        //
    //        //    Debug.Log("VirtualConnection Set Data");
    //        //    //if (thisInputNode.Node != null)
    //        //    //    port.VirtualConnection.SetData(thisInputNode.Node.GetCustomData(port.portName));
    //        //}
    //        #endregion
    //    }

    //    //public static void SuppressDynamicValue(this GraphAbstractNode thisOutputNode, NodePort output)
    //    public static void SuppressDynamicValue(this GraphAbstractNode node, NodePort connectedPort)
    //    {
    //        ///* If node has any active inputs then return */
    //        ////if (thisOutputNode.Inputs.Any(port => { return (output.IsCompatible(port) || output.portType == port.portType) && port.connected; }))
    //        //if (node.Inputs.Any(port => { return port.IsDynamic && port.connected; }))
    //        //    return;
    //        //
    //        //if (node is IValuePropagation propagationNode)
    //        //    propagationNode.GraphProperty = null;
    //        //
    //        //foreach (NodePort port in node.Ports)
    //        //{
    //        //    if (!port.IsDynamic)
    //        //        continue;
    //        //
    //        //    port.portType = typeof(object);
    //        //    port.portColor = ColorSpaceManager.GetPropertyColor(typeof(object));
    //        //
    //        //    foreach (NodePort otherPort in port.ConnectedPorts)
    //        //    {
    //        //        if (otherPort == connectedPort)
    //        //            continue;
    //        //
    //        //        //make it so that the order of connected ports doesn't matter
    //        //        //otherPort.GraphNode.OnPortConnect(port, otherPort);
    //        //
    //        //        otherPort.GraphNode.SuppressDynamicValue(port);
    //        //
    //        //        //if (port.IsInput)
    //        //        //    otherPort.GraphNode.OnPortDisconnect(otherPort, port);
    //        //        ////otherPort.GraphNode.PropagateDynamicValue(port);
    //        //        //else
    //        //        //    otherPort.GraphNode.OnPortDisconnect(port, otherPort);
    //        //        ////otherPort.GraphNode.PropagateDynamicValue(port);
    //        //    }
    //        //
    //        //    if (port.IsInput)
    //        //    {
    //        //        port.RemoveVirtualConnection();
    //        //    }
    //        //    else
    //        //    {
    //        //        port.IsCompatible = other => false;
    //        //    }
    //        //}

    //        #region
    //        ////IEnumerable<PortAttribute> portAttributes = thisOutputNode.GetType().GetCustomAttributes<PortAttribute>();
    //        //IEnumerable<PortAttribute> portAttributes = node.GetType().GetCustomAttributes<PortAttribute>();
    //        //
    //        //foreach (var attribute in portAttributes)
    //        //{
    //        //    ////Prevents from looping of dialogue nodes
    //        //    //if (attribute.Type == typeof(BaseNode))
    //        //    //    continue;
    //        //
    //        //    //NodePort port = thisOutputNode.GetPort(attribute.PortID);
    //        //    NodePort port = node.GetPort(attribute.PortID);
    //        //
    //        //    //port.IsDynamicChanged = false;
    //        //
    //        //    port.portType = attribute.Type;
    //        //    port.portColor = ColorSpaceManager.GetPropertyColor(attribute.Type);
    //        //
    //        //    if (port.IsOutput)
    //        //    {
    //        //        port.IsCompatible = other => false;
    //        //
    //        //        /* Propagate further */
    //        //        foreach (var edge in port.connections)
    //        //        {
    //        //            GraphAbstractNode nextInputNode = (edge.input as NodePort).GraphNode;
    //        //
    //        //            /* Check for loop */
    //        //            //if (nextInputNode.HasDescendant(thisOutputNode))
    //        //            if (nextInputNode.HasDescendant(node))
    //        //                continue;
    //        //
    //        //            nextInputNode.OnPortDisconnect(port, edge.input as NodePort);
    //        //        }
    //        //
    //        //        continue;
    //        //    }
    //        //
    //        //    port.RemoveVirtualConnection();
    //        //}
    //        #endregion
    //    }

    //    //public static bool HasDescendant(this GraphAbstractNode thisInputNode, GraphAbstractNode searchedNode)
    //    //{
    //    //    foreach (var outputPort in thisInputNode.Outputs)
    //    //    {
    //    //        //if (!outputPort.connected || outputPort.portType == typeof(BaseNode))
    //    //        if (!outputPort.connected)
    //    //            continue;
    //    //
    //    //        foreach (var edge in outputPort.connections)
    //    //        {
    //    //            GraphAbstractNode connectedInputNode = (edge.input as NodePort).GraphNode;
    //    //
    //    //            if (connectedInputNode == searchedNode)
    //    //                return true;
    //    //            else if (connectedInputNode.HasDescendant(searchedNode))
    //    //                return true;
    //    //        }
    //    //    }
    //    //
    //    //    return false;
    //    //}

    //    //public static GraphConnection[] GetConnections(this GraphAbstractNode graphNode)
    //    //{
    //    //    List<GraphConnection> connections = new List<GraphConnection>();
    //    //    foreach (NodePort port in graphNode.Ports)
    //    //    {
    //    //        if (port.connected)
    //    //            connections.Add(port.GetGraphConnections());
    //    //    }
    //    //    return connections.ToArray();
    //    //}
    //}

    /* Dictionary<Guid, GraphElement> Extensions */
    public static class DictionaryExtensions
    {
        public static TValue Get<TValue>(this Dictionary<Guid, GraphElement> dict, Guid key)
        {
            if (dict.TryGetValue(key, out GraphElement value))
                return value is TValue casted ? casted : default;

            return default;
        }

        public static bool TryGet<TValue>(this Dictionary<Guid, GraphElement> dict, Guid key, out TValue value)
        {
            if (dict.TryGetValue(key, out GraphElement v))
            {
                if (v is TValue casted)
                {
                    value = casted;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }

    public static class EdgeExtensions
    {
        private static StyleSheet edgeSheets { get; set; }

        public static void SetPriority(this Edge edge, DialogueSystem.ConnectionPriority priority)
        {
            if (edgeSheets == null)
            {
                edgeSheets = AssetDatabase.LoadAssetAtPath<StyleSheet>(DialogueResourceManager.EdgeStyleSheets);
            }

            if (!edge.styleSheets.Contains(edgeSheets))
            {
                edge.styleSheets.Add(edgeSheets);
            }

            edge.ClearClassList();
            edge.AddToClassList("edge");

            edge.userData = priority;

            switch (priority)
            {
                case DialogueSystem.ConnectionPriority.VeryLow:
                    edge.AddToClassList("edge-very-low-width");
                    break;
                case DialogueSystem.ConnectionPriority.Low:
                    edge.AddToClassList("edge-low-width");
                    break;
                case DialogueSystem.ConnectionPriority.Medium:
                    edge.AddToClassList("edge-medium-width");
                    break;
                case DialogueSystem.ConnectionPriority.High:
                    edge.AddToClassList("edge-high-width");
                    break;
                case DialogueSystem.ConnectionPriority.VeryHigh:
                    edge.AddToClassList("edge-very-high-width");
                    break;
            }

            edge.UpdateEdgeControl();
        }
    }
}