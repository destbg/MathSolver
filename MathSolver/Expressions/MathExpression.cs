using MathSolver.Models;

namespace MathSolver.Expressions
{
    public abstract class MathExpression
    {
        public string? Coefficient { get; set; }
        public bool IsPercent { get; set; }

        public abstract double Solve(MathVariable[] variables);
    }
}
