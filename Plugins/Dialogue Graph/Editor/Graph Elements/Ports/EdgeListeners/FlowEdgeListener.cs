using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueEditor.Nodes
{
    public partial class GraphBlockNode
    {
        protected class FlowEdgeListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                NodePort draggedPort = ((edge.output ?? null) ??
                       edge.input ?? null) as NodePort;

                DialogueGraphView graph = draggedPort.Owner.GraphView;
                graph.LocalMousePosition = position;
                Vector2 newPosition = graph.LocalMousePosition;
                newPosition -= draggedPort.IsInput ? new Vector2(160, 200) : new Vector2(160, 30); // Half the width of the element and some height offset

                GraphBlockNode newNode = new GraphBlockNode();
                graph.AddGraphElement(newNode, newPosition);

                NodePort otherPort = newNode.GetCompatiblePort(draggedPort);
                if (otherPort != null)
                {
                    graph.Connect(draggedPort, otherPort).SetPriority(DialogueSystem.ConnectionPriority.Medium);
                }

                DialogueEditorWindow.RegisterUndo("Add GraphBlockNode");
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                DialogueGraphView graph = graphView as DialogueGraphView;

                NodePort input = edge.input as NodePort;
                NodePort output = edge.output as NodePort;

                List<GraphElement> m_EdgesToDelete = new List<GraphElement>();

                if (input.capacity == Port.Capacity.Single)
                {
                    foreach (Edge edgeToDelete in input.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                }

                if (output.capacity == Port.Capacity.Single)
                {
                    foreach (Edge edgeToDelete in output.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                }

                if (m_EdgesToDelete.Count > 0)
                    graph.DeleteElements(m_EdgesToDelete);

                if (input != null && output != null)
                {
                    Edge newEdge = graph.Connect(output, input);
                    newEdge.SetPriority(DialogueSystem.ConnectionPriority.Medium);
                    DialogueEditorWindow.RegisterUndo("Connect");
                }
            }
        }
    }
}