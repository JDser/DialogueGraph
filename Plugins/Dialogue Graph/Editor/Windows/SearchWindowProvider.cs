using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueEditor
{
    public sealed class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public struct CacheType
        {
            public Type type;
            public SearchEntryAttribute attribute;

            public CacheType(Type type, SearchEntryAttribute attribute)
            {
                this.type = type;
                this.attribute = attribute;
            }
        }

        public struct NodeEntry
        {
            public string[] title;
            public int order;
            public Type type;
            public object data;
        }

        private DialogueGraphView m_GraphView;

        public Texture2D Icon { get; private set; }

        public List<SearchTreeEntry> SearchTree { get; set; }

        public void Initialize(DialogueGraphView graphView)
        {
            m_GraphView = graphView;

            Icon = new Texture2D(1, 1);
            Icon.SetPixel(0, 0, Color.clear);
            Icon.Apply();
        }

        private void OnDestroy()
        {
            if (Icon != null)
            {
                DestroyImmediate(Icon);
                Icon = null;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return SearchTree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            m_GraphView.OnSearchSelectedEntry((NodeEntry)SearchTreeEntry.userData);
            return true;
        }
    }
}