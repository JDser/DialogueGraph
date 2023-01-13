using System;
using UnityEngine;

namespace DialogueEditor
{
    public interface IGraphElement
    {
        public Guid Guid { get; set; }

        public Vector2 Position { get; set; }

        public DialogueGraphView GraphView { get; set; }
    }
}