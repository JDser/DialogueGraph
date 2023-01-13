using UnityEngine;
using DialogueSystem;

[CreateAssetMenu(fileName = "TEST Quest State Data",menuName = "TEST/Quest State Data")]
public class QuestStateData : BaseConditionData
{
    public QuestVariable Quest;

    public Comparison comparison;

    public QuestState requiredState;

    public override bool ConditionMet()
    {
        return comparison == Comparison.Is ? Quest.State == requiredState : Quest.State != requiredState;
    }

    /* Use the same BaseConditionData to execute scripts */
    //public override void SetCondition()
    //{
    //    Quest.State = requiredState;
    //}

}