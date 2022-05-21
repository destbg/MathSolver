using MathSolver.Enums;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class ConstantMathExpression : MathExpression
    {
        public ConstantMathExpression(double number, bool isPercent, bool isFactorial)
            : base(MathExpressionType.Constant)
        {
            Number = number;
            IsPercent = isPercent;
            IsFactorial = isFactorial;
        }

        public double Number { get; }

        public override double Solve(params MathVariable[] variables)
        {
            return MathHelper.CalculateNumberSuffix(Number, this);
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix(Number.ToString(), this);
        }
    }
}
