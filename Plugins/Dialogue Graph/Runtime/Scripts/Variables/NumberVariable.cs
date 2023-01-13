using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Number", menuName = "Dialogue System/Variables/Number")]
    public class NumberVariable : BaseVariable
    {
        public int number;
    }
}