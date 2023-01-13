using UnityEngine;

public class BaseConditionData : ScriptableObject
{
    public virtual bool ConditionMet()
    {
        throw new System.NotImplementedException();
    }
}