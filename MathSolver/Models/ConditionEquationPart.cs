using MathSolver.Enums;

namespace MathSolver.Models
{
    internal class ConditionEquationPart : EquationPart
    {
        public ConditionEquationPart(EquationPart leftCheck, string rightCheck, string ifTrue, string ifFalse)
             : base(EquationType.Condition)
        {
            LeftCheck = leftCheck;
            RightCheck = rightCheck;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public EquationPart LeftCheck { get; }
        public string RightCheck { get; }

        public string IfTrue { get; }
        public string IfFalse { get; }
    }
}
