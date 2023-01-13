using System.Collections.Generic;
using System;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueThread
    {
        public event Action OnThreadEnd;


        public DialogueManager Manager { get; set; }

        public DialogueAsset Asset { get; set; }

        public Node CurrentNode { get; set; }

        public Node NextNode { get; set; }

        private Property m_currentProperty = null;

        private int m_currentPropertyIndex = 0;

        private bool m_waitingForChoice = false;

        public DialogueThread(DialogueManager manager, DialogueAsset asset)
        {
            this.Manager = manager;
            this.Asset = asset;

            if (!Asset.Nodes.IsNullOrEmpty() && Asset.Nodes[0].IsConditionsMet())
            {
                CurrentNode = Asset.Nodes[0];
            }
            else
            {
                Manager.EndThread(this);
                OnThreadEnd?.Invoke();
            }
        }

        public void OnTick()
        {
            if (m_waitingForChoice)
            {
                return;
            }

            if (CurrentNode == null)
            {
                Debug.LogError("Node is null");
                Manager.EndThread(this);
                OnThreadEnd?.Invoke();
                return;
            }

            if (m_currentProperty == null && CurrentNode.Properties.Length > 0)
            {
                m_currentProperty = CurrentNode.Properties[m_currentPropertyIndex];
                m_currentProperty.thread = this;
                m_currentProperty.OnBegin();

                m_currentPropertyIndex++;
            }

            if (m_currentProperty != null)
            {
                if (m_currentProperty.OnEvaluate())
                {
                    return;
                }
                else
                {
                    m_currentProperty.OnEnd();
                    m_currentProperty.thread = null;
                    m_currentProperty = null;
                }
            }

            if (m_currentPropertyIndex >= CurrentNode.Properties.Length)
            {
                if (NextNode != null)
                {
                    SetCurrentNode(NextNode);
                    NextNode = null;
                }
                else
                {
                    bool choicesOnly = false;
                    List<Node> choiceNodes = new List<Node>();

                    for (int i = 0; i < CurrentNode.Connections.Length; i++)
                    {
                        Node node = CurrentNode.Connections[i].Node;

                        if (!node.IsConditionsMet())
                        {
                            continue;
                        }

                        if (node.IsChoice)
                        {
                            choicesOnly = true;
                            choiceNodes.Add(node);
                        }
                        else if (choicesOnly == false)
                        {
                            SetCurrentNode(node);
                            return;
                        }
                    }

                    if (choicesOnly && choiceNodes.Count > 0)
                    {
                        m_waitingForChoice = true;
                        Manager.SetChoices(this, choiceNodes);
                        return;
                    }
                    else
                    {
                        Manager.EndThread(this);
                        OnThreadEnd?.Invoke();
                        return;
                    }
                }
            }
        }

        public void SetCurrentNode(Node newCurrentNode)
        {
            m_currentProperty = null;
            m_currentPropertyIndex = 0;
            m_waitingForChoice = false;

            CurrentNode = newCurrentNode;

            Manager.SetLine(CurrentNode.Line);
        }
    }
}