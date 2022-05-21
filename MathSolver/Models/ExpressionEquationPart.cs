using MathSolver.Enums;
using MathSolver.Expressions;

namespace MathSolver.Models
{
    internal class ExpressionEquationPart : EquationPart
    {
        public ExpressionEquationPart(MathExpression mathExpression)
            : base(EquationType.MathExpression)
        {
            MathExpression = mathExpression;
        }

        public MathExpression MathExpression { get; }
    }
}
