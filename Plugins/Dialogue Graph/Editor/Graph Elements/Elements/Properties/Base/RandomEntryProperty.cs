using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueEditor.Properties
{
    using DialogueSystem;

    [SearchEntry("Random Entry")]
    public class RandomEntryProperty : AbstractBlockProperty
    {
        [SerializeField] private ObjectReference<RandomEntry> m_SerializedReference;

        public override string Title => "Random Entry";

        public override void Compile()
        {

        }

        public override Object GetObject()
        {
            if (m_SerializedReference == null)
            {
                m_SerializedReference = new ObjectReference<RandomEntry>();
            }

            return m_SerializedReference.Reference;
        }
    }
}