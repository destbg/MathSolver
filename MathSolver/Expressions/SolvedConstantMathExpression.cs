using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class SolvedConstantMathExpression : ConstantMathExpression
    {
        public SolvedConstantMathExpression(double number, bool isPercent, bool isFactorial)
            : base(number, isPercent, isFactorial) { }

        public override double Solve(params MathVariable[] variables)
        {
            return Number;
        }

        public override string ToString()
        {
            if (IsPercent)
            {
                return (Number * 100).ToString() + '%';
            }

            return Number.ToString();
        }
    }
}
