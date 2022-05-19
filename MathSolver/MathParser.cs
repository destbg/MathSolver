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
            MathExpressionParser expressionParser = new(equation);

            List<SimpleExpression> expressions = expressionParser.Parse();

            MathExpressionSimplifier simplifier = new(equation, expressions, coefficient, isPercent);

            return simplifier.Simplify();
        }
    }
}
