using MathSolver.Enums;

namespace MathSolver.Models
{
    internal abstract class EquationPart
    {
        public EquationPart(EquationType type)
        {
            Type = type;
        }

        public EquationType Type { get; }

        public bool IsPercent { get; set; }
    }
}
