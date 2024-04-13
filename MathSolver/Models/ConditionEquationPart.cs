using MathSolver.Enums;

namespace MathSolver.Models;

internal class ConditionEquationPart : EquationPart
{
    public ConditionEquationPart(ConditionType condition)
         : base(EquationType.Condition)
    {
        Condition = condition;
    }

    public ConditionType Condition { get; }
}
