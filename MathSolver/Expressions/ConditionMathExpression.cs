using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class ConditionMathExpression : MathExpression
    {
        public ConditionMathExpression(bool isEqual, MathExpression leftCheck, MathExpression rightCheck, MathExpression ifTrue, MathExpression ifFalse)
            : base(MathExpressionType.Condition, new List<MathSuffixSymbol>())
        {
            IsEqual = isEqual;
            LeftCheck = leftCheck;
            RightCheck = rightCheck;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public bool IsEqual { get; }
        public MathExpression LeftCheck { get; }
        public MathExpression RightCheck { get; }
        public MathExpression IfTrue { get; }
        public MathExpression IfFalse { get; }

        public override double Solve(params MathVariable[] variables)
        {
            MathExpression ifTrue;
            MathExpression ifFalse;

            if (IsEqual)
            {
                ifTrue = IfTrue;
                ifFalse = IfFalse;
            }
            else
            {
                ifTrue = IfFalse;
                ifFalse = IfTrue;
            }

            return LeftCheck.Solve(variables) == RightCheck.Solve(variables)
                ? ifTrue.Solve(variables)
                : ifFalse.Solve(variables);
        }

        public override string ToString()
        {
            return this.Suffix($"{LeftCheck} = {RightCheck} ? {IfTrue} : {IfFalse}", BracketType.None);
        }
    }
}
