using MathSolver.Enums;

namespace MathSolver.Models;

internal class SubEquationPart : EquationPart
{
    public SubEquationPart(string? coefficient, string expression, BracketType bracket)
        : base(EquationType.Expression)
    {
        Coefficient = coefficient;
        Expression = expression;
        Bracket = bracket;
    }

    public string? Coefficient { get; }
    public string Expression { get; }
    public BracketType Bracket { get; }
}
