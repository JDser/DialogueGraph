using DialogueSystem;
using UnityEngine;

public class TEST_DialogueTrigger : MonoBehaviour
{
    [field: SerializeField]
    public TEST_DialogueManager Manager { get; private set; }

    [field: SerializeField]
    public DialogueAsset Asset { get; private set; }

    [field: SerializeField]
    public bool IsBusy { get; private set; }

    public Condition Condition;

    public virtual void OnUse()
    {
        if (IsBusy)
        {
            return;
        }

        if (Condition.IsMet())
        {
            IsBusy = true;
            Manager.StartDialogue(Asset, OnDialogueEnd);
        }
    }

    public virtual void OnDialogueEnd()
    {
        IsBusy = false;
    }



    private void Start()
    {
        Debug.Log(Condition.IsMet());
    }
}