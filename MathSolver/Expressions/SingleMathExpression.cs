namespace MathSolver.Expressions
{
    public class SingleMathExpression : MathExpression
    {
        public SingleMathExpression(MathExpression operand, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
            : base(MathExpressionType.Single, suffixSymbols)
        {
            Operand = operand;
        }

        public MathExpression Operand { get; }

        public string? Coefficient { get; internal set; }

        public override double Solve(params MathVariable[] variables)
        {
            double result = Operand.Solve(variables);

            foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
            {
                result = suffixSymbol switch
                {
                    MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                    MathSuffixSymbol.Percent => result / 100,
                    _ => throw new Exception($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
                };
            }

            if (!string.IsNullOrEmpty(Coefficient))
            {
                result = MathHelper.CalculateCoefficient(Coefficient, result);
            }

            return result;
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix($"({Operand})", this, false);
        }
    }
}
