using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [field: SerializeField]
        public DialogueManager Manager { get; private set; }

        [field: SerializeField]
        public DialogueAsset Asset { get; private set; }

        [field: SerializeField]
        public bool IsBusy { get; /*private*/ set; }


        public virtual void OnUse()
        {

        }


    }
}