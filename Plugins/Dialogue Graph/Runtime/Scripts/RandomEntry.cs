using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class RandomEntry : Property
    {
        public override void OnBegin()
        {
            int random = Random.Range(0, thread.CurrentNode.Connections.Length);
            thread.NextNode = thread.CurrentNode.Connections[random].Node;
        }
    }
}