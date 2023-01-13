using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueEditor
{
    public partial class NodePort
    {
        private class NodeEdgeListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                NodePort draggedPort = ((edge.output ?? null) ??
                                   (edge.input ?? null)) as NodePort;

                DialogueGraphView graph = draggedPort.Owner.GraphView;

                graph.LocalMousePosition = position;
                graph.OpenNodeSearchWindow(draggedPort);
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                DialogueGraphView graph = graphView as DialogueGraphView;

                NodePort input = edge.input as NodePort;
                NodePort output = edge.output as NodePort;

                List<GraphElement> m_EdgesToDelete = new List<GraphElement>();

                if (input.capacity == Capacity.Single)
                {
                    foreach (Edge edgeToDelete in input.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                }

                if (output.capacity == Capacity.Single)
                {
                    foreach (Edge edgeToDelete in output.connections)
                        if (edgeToDelete != edge)
                            m_EdgesToDelete.Add(edgeToDelete);
                }

                if (m_EdgesToDelete.Count > 0)
                    graph.DeleteElements(m_EdgesToDelete);

                if (input != null && output != null)
                {
                    graph.Connect(output, input);
                    DialogueEditorWindow.RegisterUndo("Connect");
                }
            }
        }
    }
}