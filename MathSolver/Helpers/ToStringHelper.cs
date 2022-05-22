using System.Text;

namespace MathSolver.Helpers
{
    internal static class ToStringHelper
    {
        public static string ExpressionSuffix(string str, MathExpression expression, bool useBrackets = true)
        {
            StringBuilder builder = new();

            if (expression is UnaryMathExpression unaryExpression && !string.IsNullOrEmpty(unaryExpression.Coefficient))
            {
                if (useBrackets)
                {
                    builder = builder.Append(unaryExpression.Coefficient)
                        .Append('(')
                        .Append(str)
                        .Append(')');
                }
                else
                {
                    builder = builder.Append(unaryExpression.Coefficient).Append(str);
                }
            }
            else if (expression is SingleMathExpression singleExpression && !string.IsNullOrEmpty(singleExpression.Coefficient))
            {
                if (useBrackets)
                {
                    builder = builder.Append(singleExpression.Coefficient)
                        .Append('(')
                        .Append(str)
                        .Append(')');
                }
                else
                {
                    builder = builder.Append(singleExpression.Coefficient).Append(str);
                }
            }
            else
            {
                builder = builder.Append(str);
            }

            foreach (MathSuffixSymbol suffixSymbol in expression.SuffixSymbols)
            {
                builder = suffixSymbol switch
                {
                    MathSuffixSymbol.Factorial => builder.Append('!'),
                    MathSuffixSymbol.Percent => builder.Append('%'),
                    _ => throw new Exception($"Internal exception: {nameof(ExpressionSuffix)} method does not implement {nameof(MathSuffixSymbol)}.")
                };
            }

            return builder.ToString();
        }

        public static char SymbolEnumToChar(MathSymbol symbol)
        {
            return symbol switch
            {
                MathSymbol.Addition => '+',
                MathSymbol.Subraction => '-',
                MathSymbol.Multiplication => '*',
                MathSymbol.Division => '/',
                MathSymbol.Power => '^',
                _ => '?',
            };
        }
    }
}
