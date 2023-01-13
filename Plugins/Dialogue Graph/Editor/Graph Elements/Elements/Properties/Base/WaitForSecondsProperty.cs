using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueEditor.Properties
{
    using DialogueEditor;

    using DialogueSystem;

    [SearchEntry("Wait for Seconds")]
    [Input("Input", typeof(float), Name = "Duration", PortSingleCapacity = true)]
    public class WaitForSeconds : AbstractBlockProperty
    {
        /* Serializable */

        [SerializeField] private ObjectReference<WaitForSecondsProperty> m_SerializedReference;

        /* Non-Serializable */

        private FloatField m_durationField;

        /* Properties */

        public override string Title => "Wait for Seconds";

        /* Initialization */

        public WaitForSeconds()
        {
            contentContainer.Add(new Label("Duration : "));

            m_durationField = new FloatField();
            contentContainer.Add(m_durationField);
        }

        public override void Validate()
        {
            if (m_SerializedReference != null)
            {
                m_durationField.value = m_SerializedReference.Reference.duration;
                ObjectToSelect = m_SerializedReference.Reference;
            }

            base.Validate();
        }

        public override Object GetObject()
        {
            if (m_SerializedReference == null)
            {
                m_SerializedReference = new ObjectReference<WaitForSecondsProperty>();
            }

            return m_SerializedReference.Reference;
        }

        public override void Compile()
        {
            if (!GetPort("Input").connected)
            {
                m_SerializedReference.Reference.duration = m_durationField.value;
            }
            else
            {
                Debug.Log("Set reference to object");
            }
        }
    }
}