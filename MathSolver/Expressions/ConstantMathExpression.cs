using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class ConstantMathExpression : MathExpression
    {
        private readonly double number;

        public ConstantMathExpression(double number, bool isPercent)
        {
            this.number = number;
            IsPercent = isPercent;
        }

        public override double Solve(MathVariable[] variables)
        {
            return !string.IsNullOrEmpty(Coefficient) ?
                MathHelper.CalculateCoefficient(Coefficient, number)
                : number;
        }

        public override string ToString()
        {
            return number.ToString();
        }
    }
}
