using UnityEngine;

namespace DialogueSystem
{
    public enum QuestState
    {
        Unassigned,
        Active,
        Success,
        Failure,
        Done,
        Abandoned,
        Grantable,
        ReturnToNPC
    }

    [System.Serializable]
    public struct QuestSubTask
    {
        [field: SerializeField]
        public QuestState State;

        [field: SerializeField]
        public string Text;
    }

    [CreateAssetMenu(fileName = "New Quest", menuName = "Dialogue System/Variables/Quest")]
    public class QuestVariable : BaseVariable
    {
        [field: SerializeField]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        public string Description { get; private set; }

        [field: SerializeField]
        public string SuccessDescription { get; private set; }

        [field: SerializeField]
        public string FailureDescription { get; private set; }

        public QuestState State;

        public QuestSubTask[] SubTasks;
    }
}