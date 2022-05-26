using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class ConditionMathExpression : MathExpression
    {
        public ConditionMathExpression(MathExpression leftCheck, MathExpression rightCheck, MathExpression ifTrue, MathExpression ifFalse)
            : base(MathExpressionType.Condition, new List<MathSuffixSymbol>())
        {
            LeftCheck = leftCheck;
            RightCheck = rightCheck;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public MathExpression LeftCheck { get; }
        public MathExpression RightCheck { get; }
        public MathExpression IfTrue { get; }
        public MathExpression IfFalse { get; }

        public override double Solve(params MathVariable[] variables)
        {
            return LeftCheck.Solve(variables) == RightCheck.Solve(variables)
                ? IfTrue.Solve(variables)
                : IfFalse.Solve(variables);
        }

        public override string ToString()
        {
            return this.Suffix($"{LeftCheck} = {RightCheck} ? {IfTrue} : {IfFalse}", BracketType.None);
        }
    }
}
