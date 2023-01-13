using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public UIManager UIManager;

        private DialogueThread m_FocusedThread;

        private List<DialogueThread> m_DialogueThreads = new List<DialogueThread>();

        public bool IsTalking => m_FocusedThread != null;

        private void Update()
        {
            if (m_DialogueThreads.Count == 0)
            {
                this.enabled = false;
                return;
            }

            for (int i = 0; i < m_DialogueThreads.Count; i++)
            {
                m_DialogueThreads[i].OnTick();
            }
        }

        public void StartDialogue(DialogueAsset asset, Action onThreadEndAction)
        {
            Debug.Log("StartDialogue");

            DialogueThread newThread = new DialogueThread(this, asset);
            newThread.OnThreadEnd += onThreadEndAction;

            m_DialogueThreads.Add(newThread);
            enabled = true;
        }

        public void StartFocusedDialogue(DialogueTrigger trigger)
        {
            if (!trigger.IsBusy)
            {
                m_FocusedThread = new DialogueThread(this, trigger.Asset);
                m_FocusedThread.OnThreadEnd += () =>
                {
                    trigger.IsBusy = false;
                    UIManager.ClearSubtitle();
                };

                Debug.Log("StartDialogueFocused");
                m_DialogueThreads.Add(m_FocusedThread);

                enabled = true;
                trigger.IsBusy = true;
            }
        }

        public void CancelFocusedDialogue()
        {
            if (m_FocusedThread != null)
            {
                Debug.Log("CancelDialogue");
                EndThread(m_FocusedThread);
                m_FocusedThread = null;
            }
        }

        //Difference between bark and startDialogue (usual dialogue) is that bark can be intercepted and dialogue not
        public void Bark(DialogueTrigger trigger)
        {
            //if (!m_DialogueThreads.Any(e => e.Asset == trigger.Asset))
            if (!trigger.IsBusy)
            {
                Debug.Log("Bark");

                DialogueThread newThread = new DialogueThread(this, trigger.Asset);
                newThread.OnThreadEnd += () =>
                {
                    trigger.IsBusy = false;
                    UIManager.ClearSubtitle();
                };

                m_DialogueThreads.Add(newThread);

                enabled = true;
                trigger.IsBusy = true;
            }
        }

        public void EndThread(DialogueThread threadToStop)
        {
            m_DialogueThreads.Remove(threadToStop);
        }

        public void SetLine(string line)
        {
            UIManager.SetSubtitle(line);
            Debug.Log(line);
        }

        public async void SetChoices(DialogueThread thread, List<Node> choiceNodes)
        {
            int delay = UnityEngine.Random.Range(2000, 10000);
            Debug.LogError($"Waiting for {delay} ms");

            await System.Threading.Tasks.Task.Delay(delay);

            //For now we will pick randomly
            int random = UnityEngine.Random.Range(0, choiceNodes.Count);
            thread.SetCurrentNode(choiceNodes[random]);
        }




        private void OnValidate()
        {
            //Debug.LogWarning("Make copyPaste blockNodes with properties");
            //Debug.LogWarning("Fix double GetObjects when saving");
            Debug.LogWarning("Fix virtualConnecton padding offset by port's parent");
            Debug.LogWarning("Add float field");
            Debug.LogWarning("Add blackboard");
            Debug.LogWarning("Add inspector");
            //Debug.LogWarning("Make contentContainer and portContainer for blockFields");
            //Debug.LogWarning("Make nodes connecting to blockFields");
            Debug.LogWarning("Make dynamic value propagation working with blockFields");
            Debug.LogWarning("(Dynamic value propagation) Collect all connected nodes and check if there any non dynamic nodes");
            //Done : Debug.LogWarning("Make AbstractBlockProperty copyPste to block under the cursor");
            //Done : Debug.LogWarning("Make CalculateMeanPostion (1085, DialogueGrapgView.cs) ignore AbstractBlockProperty (or any other non-movable elements)");

            Debug.LogWarning("Node templates");
            Debug.LogWarning("Global variables");
            Debug.LogWarning("Collapsable groups");
            //Debug.LogWarning("Change color of the groups");

            //Debug.LogWarning("Connected to property edges weirdly collapsing (same with toggleProperties)");

            Debug.LogWarning("DialogueGraphView ApplyColor perfomance bottleneck (line : 780)");
            Debug.LogWarning("It's not a problem but could be : reorder of connected nodes if i want to have a specific one");
            Debug.LogWarning("Bookmarks, variable bookmarks and dialogue transitions");

            Debug.LogWarning("On making new Properties: assigning name must be automatic, create new properties too difficult");
            Debug.LogWarning("Node templates");

        }
    }
}