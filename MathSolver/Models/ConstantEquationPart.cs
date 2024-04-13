using MathSolver.Enums;

namespace MathSolver.Models;

internal class ConstantEquationPart : EquationPart
{
    public ConstantEquationPart(double number)
        : base(EquationType.Number)
    {
        Number = number;
    }

    public double Number { get; }
}
