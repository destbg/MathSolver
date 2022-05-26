using System;
using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;

namespace MathSolver.Helpers
{
    internal static class MathExpressionHelper
    {
        public static Expression CreateEquation(UnaryMathExpression unaryMath, Expression leftExpression, Expression rightExpression)
        {
            bool leftHasPercent = unaryMath.LeftOperand.SuffixSymbols.Count != 0 && unaryMath.LeftOperand.SuffixSymbols[^1] == MathSuffixSymbol.Percent;
            bool rightHasPercent = unaryMath.RightOperand.SuffixSymbols.Count != 0 && unaryMath.RightOperand.SuffixSymbols[^1] == MathSuffixSymbol.Percent;

            if (leftHasPercent || rightHasPercent)
            {
                if (leftHasPercent && !rightHasPercent)
                {
                    (leftExpression, rightExpression) = (rightExpression, leftExpression);
                }

                if (unaryMath.Symbol == MathSymbol.Addition)
                {
                    return Expression.Multiply(leftExpression, Expression.Add(Expression.Constant(1d), rightExpression));
                }
                else if (unaryMath.Symbol == MathSymbol.Subraction)
                {
                    return Expression.Multiply(leftExpression, Expression.Subtract(Expression.Constant(1d), rightExpression));
                }
            }

            return unaryMath.Symbol switch
            {
                MathSymbol.Addition => Expression.Add(leftExpression, rightExpression),
                MathSymbol.Subraction => Expression.Subtract(leftExpression, rightExpression),
                MathSymbol.Multiplication => Expression.Multiply(leftExpression, rightExpression),
                MathSymbol.Division => Expression.Divide(leftExpression, rightExpression),
                MathSymbol.Power => Expression.Call(typeof(Math), nameof(Math.Pow), null, leftExpression, rightExpression),
                _ => throw new InvalidMathExpressionException($"Internal Exception: Method {nameof(CreateEquation)} does not have a {nameof(MathSymbol)} implemented."),
            };
        }

        public static (string, Expression[]) CreateCoefficientCall(string coefficient, Expression expression)
        {
            if (coefficient.StartsWith("log"))
            {
                if (coefficient == "log10")
                {
                    return (nameof(Math.Log10), new[] { expression });
                }

                return (nameof(Math.Log), new[] { expression, Expression.Constant(int.Parse(coefficient.Replace("log", string.Empty))) });
            }

            if (coefficient.StartsWith("sqrt"))
            {
                if (coefficient == "sqrt")
                {
                    return (nameof(Math.Sqrt), new[] { expression });
                }

                ConstantExpression nthRoot = Expression.Constant((double)int.Parse(coefficient.Replace("sqrt", string.Empty)));
                BinaryExpression division = Expression.Divide(Expression.Constant(1d), nthRoot);

                return (nameof(Math.Pow), new[] { expression, division });
            }

            return coefficient switch
            {
                "abs" => (nameof(Math.Abs), new[] { expression }),
                "acos" => (nameof(Math.Acos), new[] { expression }),
                "acosh" => (nameof(Math.Acosh), new[] { expression }),
                "asin" => (nameof(Math.Asin), new[] { expression }),
                "asinh" => (nameof(Math.Asinh), new[] { expression }),
                "atan" => (nameof(Math.Atan), new[] { expression }),
                "atanh" => (nameof(Math.Atanh), new[] { expression }),
                "cbrt" => (nameof(Math.Cbrt), new[] { expression }),
                "ceil" => (nameof(Math.Ceiling), new[] { expression }),
                "cos" => (nameof(Math.Cos), new[] { expression }),
                "cosh" => (nameof(Math.Cosh), new[] { expression }),
                "floor" => (nameof(Math.Floor), new[] { expression }),
                "round" => (nameof(Math.Round), new[] { expression }),
                "sign" => (nameof(Math.Sign), new[] { expression }),
                "sin" => (nameof(Math.Sin), new[] { expression }),
                "sinh" => (nameof(Math.Sinh), new[] { expression }),
                "tan" => (nameof(Math.Tan), new[] { expression }),
                "tanh" => (nameof(Math.Tanh), new[] { expression }),
                "trunc" => (nameof(Math.Truncate), new[] { expression }),
                _ => throw new InvalidMathExpressionException($"The provided coefficient {coefficient} was not valid."),
            };
        }
    }
}
