using System.Text;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;

namespace MathSolver.Helpers
{
    internal static class ToStringHelper
    {
        public static string Suffix(this MathExpression expression, string str, BracketType bracketType = BracketType.None)
        {
            StringBuilder builder = new StringBuilder();

            if (expression is UnaryMathExpression unaryExpression && !string.IsNullOrEmpty(unaryExpression.Coefficient))
            {
                builder = builder.Append(unaryExpression.Coefficient);
            }
            else if (expression is SingleMathExpression singleExpression && !string.IsNullOrEmpty(singleExpression.Coefficient))
            {
                builder = builder.Append(singleExpression.Coefficient);
            }

            builder = builder.ApplyBracket(str, bracketType);

            foreach (MathSuffixSymbol suffixSymbol in expression.SuffixSymbols)
            {
                builder = suffixSymbol switch
                {
                    MathSuffixSymbol.Factorial => builder.Append('!'),
                    MathSuffixSymbol.Percent => builder.Append('%'),
                    _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(Suffix)} method does not implement {nameof(MathSuffixSymbol)}.")
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

        private static StringBuilder ApplyBracket(this StringBuilder builder, string str, BracketType bracketType)
        {
            return bracketType switch
            {
                BracketType.None => builder.Append(str),
                BracketType.Parentheses => builder.Append('(').Append(str).Append(')'),
                BracketType.Square => builder.Append('[').Append(str).Append(']'),
                BracketType.Angle => builder.Append('<').Append(str).Append('>'),
                BracketType.Curly => builder.Append('{').Append(str).Append('}'),
                BracketType.Straight => builder.Append('|').Append(str).Append('|'),
                _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(SymbolEnumToChar)} method does not implement {nameof(BracketType)}."),
            };
        }
    }
}
