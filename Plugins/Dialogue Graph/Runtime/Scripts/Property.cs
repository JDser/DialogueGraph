using UnityEngine;

namespace DialogueSystem
{
    public class Property : ScriptableObject
    {
        public DialogueThread thread;

        public virtual void OnBegin()
        {

        }

        public virtual bool OnEvaluate()
        {
            return false;
        }

        public virtual void OnEnd()
        {

        }

        private void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }
    }
}