using System.Text;
using MathSolver.Enums;
using MathSolver.Expressions;

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
            else
            {
                builder = builder.Append(str);
            }

            if (expression.IsFactorial)
            {
                builder = builder.Append('!');
            }

            if (expression.IsPercent)
            {
                builder = builder.Append('%');
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
