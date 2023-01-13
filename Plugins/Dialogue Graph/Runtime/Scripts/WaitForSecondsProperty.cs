using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class WaitForSecondsProperty : Property
    {
        public float duration;


        private float m_endTime;

        public override void OnBegin()
        {
            m_endTime = Time.time + duration;
        }

        public override bool OnEvaluate()
        {
            return m_endTime > Time.time;
        }
    }
}