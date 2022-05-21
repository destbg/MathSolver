using MathSolver.Enums;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public abstract class MathExpression
    {
        public MathExpression(MathExpressionType type)
        {
            Type = type;
        }

        public MathExpressionType Type { get; }

        public bool IsPercent { get; set; }
        public bool IsFactorial { get; set; }

        public abstract double Solve(params MathVariable[] variables);
    }
}
