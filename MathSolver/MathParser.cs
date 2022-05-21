using MathSolver.Expressions;
using MathSolver.Models;

namespace MathSolver
{
    public static class MathParser
    {
        public static MathExpression Parse(string equation)
        {
            return Parse(equation, null, false);
        }

        internal static MathExpression Parse(string equation, string? coefficient, bool isPercent)
        {
            MathEquationParser parser = new(equation);

            List<EquationPart> expressions = parser.Parse();

            EquationToExpressionConveter converter = new(equation, expressions, coefficient, isPercent);

            return converter.Convert();
        }
    }
}
