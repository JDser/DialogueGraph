using UnityEngine;
using DialogueSystem;

[CreateAssetMenu(fileName = "TEST Numeric Comparison Data", menuName = "TEST/Numeric Comparison Data")]
public class NumericComparisonData : BaseConditionData
{
    public NumberVariable numberVarA;

    public NumberVariable numberVarB;

    [SerializeField] private int valueA;

    [SerializeField] private int valueB;

    public NumericComparison comparison;

    public int ValueA
    {
        get
        {
            return numberVarA != null ? numberVarA.number : valueA;
        }
    }
   
    public int ValueB
    {
        get
        {
            return numberVarB != null ? numberVarB.number : valueB;
        }
    }


    public override bool ConditionMet()
    {
        return comparison switch
        {
            NumericComparison.Is => ValueA == ValueB,
            NumericComparison.IsNot => ValueA != ValueB,
            NumericComparison.Less => ValueA < ValueB,
            NumericComparison.LessEqual => ValueA <= ValueB,
            NumericComparison.Greater => ValueA > ValueB,
            NumericComparison.GreaterEqual => ValueA >= ValueB,
            _ => false,
        };
    }

}