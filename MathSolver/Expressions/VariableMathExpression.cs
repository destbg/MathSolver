namespace MathSolver.Expressions
{
    public class VariableMathExpression : MathExpression
    {
        public VariableMathExpression(char variable, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
            : base(MathExpressionType.Variable, suffixSymbols)
        {
            Variable = variable;
        }

        public char Variable { get; }

        public override double Solve(params MathVariable[] variables)
        {
            foreach (MathVariable variable in variables)
            {
                if (variable.Variable == Variable)
                {
                    double result = variable.Number;

                    foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
                    {
                        result = suffixSymbol switch
                        {
                            MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                            MathSuffixSymbol.Percent => result / 100,
                            _ => throw new Exception($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
                        };
                    }

                    return result;
                }
            }

            throw new InvalidExpressionException($"The provided variable {Variable} was not found in the list of variables {variables}.");
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix(Variable.ToString(), this);
        }
    }
}
