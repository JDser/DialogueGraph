using System;

public enum ConditionRule
{
    All,
    Any
}

public enum Comparison
{
    Is,
    IsNot
} 

public enum NumericComparison
{
    Is,
    IsNot,
    Less,
    LessEqual,
    Greater,
    GreaterEqual
}

[Serializable]
public class Condition
{
    public ConditionRule Rule;

    public BaseConditionData[] RequiredConditions;

    public bool IsMet()
    {
        bool result = true;

        for (int i = 0; i < RequiredConditions.Length; i++)
        {
            if (Rule == ConditionRule.All)
            {
                if (RequiredConditions[i].ConditionMet() == false)
                {
                    return false;
                }
            }
            else if (Rule == ConditionRule.Any)
            {
                if (RequiredConditions[i].ConditionMet())
                {
                    return true;
                }

                result = false;
            }
        }

        return result;
    }
}