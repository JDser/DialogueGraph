using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;

public class TEST_DialogueManager : MonoBehaviour
{
    private TEST_DialogueThread m_FocusedThread;

    private List<TEST_DialogueThread> m_DialogueThreads = new List<TEST_DialogueThread>();

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

        TEST_DialogueThread newThread = new TEST_DialogueThread(this, asset);
        newThread.OnThreadEnd += onThreadEndAction;

        m_DialogueThreads.Add(newThread);
        enabled = true;
    }

    public void StartFocusedDialogue(DialogueTrigger trigger)
    {
        if (!trigger.IsBusy)
        {
            m_FocusedThread = new TEST_DialogueThread(this, trigger.Asset);
            m_FocusedThread.OnThreadEnd += () =>
            {
                trigger.IsBusy = false;
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

            TEST_DialogueThread newThread = new TEST_DialogueThread(this, trigger.Asset);
            newThread.OnThreadEnd += () =>
            {
                trigger.IsBusy = false;
            };

            m_DialogueThreads.Add(newThread);

            enabled = true;
            trigger.IsBusy = true;
        }
    }

    public void EndThread(TEST_DialogueThread threadToStop)
    {
        m_DialogueThreads.Remove(threadToStop);
    }








    public void SetLine(string line)
    {
        Debug.Log(line);
    }

    public async void SetChoices(TEST_DialogueThread thread, List<Node> choiceNodes)
    {
        int delay = UnityEngine.Random.Range(2000, 10000);
        Debug.LogError($"Waiting for {delay} ms");

        await System.Threading.Tasks.Task.Delay(delay);

        //For now we will pick randomly
        int random = UnityEngine.Random.Range(0, choiceNodes.Count);
        thread.SetCurrentNode(choiceNodes[random]);
    }
}
