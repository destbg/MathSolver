using MathSolver.Enums;

namespace MathSolver.Models;

internal class VariableEquationPart : EquationPart
{
    public VariableEquationPart(char variable)
        : base(EquationType.Variable)
    {
        Variable = variable;
    }

    public char Variable { get; }
}
