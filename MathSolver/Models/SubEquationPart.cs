using MathSolver.Enums;

namespace MathSolver.Models
{
    internal class SubEquationPart : EquationPart
    {
        public SubEquationPart(string? coefficient, string expression)
            : base(EquationType.Expression)
        {
            Coefficient = coefficient;
            Expression = expression;
        }

        public string? Coefficient { get; }
        public string? Expression { get; }
    }
}
