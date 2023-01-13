using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueEditor.Nodes
{
    [SearchEntry("Test", "Test Node")]
    [Input("Input string", typeof(string))]
    [Input("Input bool", typeof(bool))]
    [Output("Output string", typeof(string))]
    [Output("Output bool", typeof(bool))]
    [Output("Output float", typeof(float))]
    public class TestNode : GraphAbstractNode
    {
        public override string Title => "Test Node";
    }
}