using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Helpers;

namespace MathSolver.Converters
{
    internal class MathExpressionToExpressionConverter
    {
        private readonly MathExpression rootMathExpression;

        public MathExpressionToExpressionConverter(MathExpression mathExpression)
        {
            rootMathExpression = mathExpression;
            Parameters = (VariableFinder(mathExpression)?.ToList() ?? new List<char>())
                .ConvertAll(f => Expression.Parameter(typeof(double), f.ToString())); ;
        }

        public List<ParameterExpression> Parameters { get; }

        public Expression Convert()
        {
            return ParseMathToCSharp(rootMathExpression);
        }

        private Expression ParseMathToCSharp(MathExpression mathExpression)
        {
            switch (mathExpression.Type)
            {
                case MathExpressionType.Variable:
                {
                    VariableMathExpression variableMath = (VariableMathExpression)mathExpression;
                    string variableAsString = variableMath.Variable.ToString();
                    ParameterExpression parameter = Parameters.Find(f => f.Name == variableAsString)!;

                    Expression exp = parameter;

                    foreach (MathSuffixSymbol suffixSymbol in variableMath.SuffixSymbols)
                    {
                        exp = suffixSymbol switch
                        {
                            MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                            MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                            _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(ParseMathToCSharp)} method does not implement {nameof(MathSuffixSymbol)}.")
                        };
                    }

                    return exp;
                }
                case MathExpressionType.Constant:
                {
                    if (mathExpression is SolvedConstantMathExpression solvedConstantMath)
                    {
                        return Expression.Constant(solvedConstantMath.Number);
                    }
                    else
                    {
                        ConstantMathExpression constantMath = (ConstantMathExpression)mathExpression;

                        Expression exp = Expression.Constant(constantMath.Number);

                        foreach (MathSuffixSymbol suffixSymbol in constantMath.SuffixSymbols)
                        {
                            exp = suffixSymbol switch
                            {
                                MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                                MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                                _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(ParseMathToCSharp)} method does not implement {nameof(MathSuffixSymbol)}.")
                            };
                        }

                        return exp;
                    }
                }
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryMath = (UnaryMathExpression)mathExpression;

                    Expression leftExpression = ParseMathToCSharp(unaryMath.LeftOperand);
                    Expression rightExpression = ParseMathToCSharp(unaryMath.RightOperand);

                    Expression exp = CreateEquation(unaryMath, leftExpression, rightExpression);

                    foreach (MathSuffixSymbol suffixSymbol in unaryMath.SuffixSymbols)
                    {
                        exp = suffixSymbol switch
                        {
                            MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                            MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                            _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(ParseMathToCSharp)} method does not implement {nameof(MathSuffixSymbol)}.")
                        };
                    }

                    if (unaryMath.Bracket == BracketType.Straight)
                    {
                        exp = Expression.Call(typeof(Math), nameof(Math.Abs), null, exp);
                    }

                    if (!string.IsNullOrEmpty(unaryMath.Coefficient))
                    {
                        (string method, Expression[] parameters) = CreateCoefficientCall(unaryMath.Coefficient, exp);

                        exp = Expression.Call(typeof(Math), method, null, parameters);
                    }

                    return exp;
                }
                case MathExpressionType.Single:
                {
                    SingleMathExpression singleMath = (SingleMathExpression)mathExpression;

                    Expression exp = ParseMathToCSharp(singleMath.Operand);

                    foreach (MathSuffixSymbol suffixSymbol in singleMath.SuffixSymbols)
                    {
                        exp = suffixSymbol switch
                        {
                            MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                            MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                            _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(ParseMathToCSharp)} method does not implement {nameof(MathSuffixSymbol)}.")
                        };
                    }

                    if (singleMath.Bracket == BracketType.Straight)
                    {
                        exp = Expression.Call(typeof(Math), nameof(Math.Abs), null, exp);
                    }

                    if (!string.IsNullOrEmpty(singleMath.Coefficient))
                    {
                        (string method, Expression[] parameters) = CreateCoefficientCall(singleMath.Coefficient, exp);

                        exp = Expression.Call(typeof(Math), method, null, parameters);
                    }

                    return exp;
                }
                default:
                    throw new InvalidMathExpressionException($"Internal Exception: The {nameof(ParseMathToCSharp)} method did not have a {nameof(MathExpressionType)} implemented.");
            }
        }

        private static Expression CreateEquation(UnaryMathExpression unaryMath, Expression leftExpression, Expression rightExpression)
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

        private static (string, Expression[]) CreateCoefficientCall(string coefficient, Expression expression)
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

        private static HashSet<char>? VariableFinder(MathExpression expression)
        {
            switch (expression.Type)
            {
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryExpression = (UnaryMathExpression)expression;

                    HashSet<char>? leftVariables = VariableFinder(unaryExpression.LeftOperand);
                    HashSet<char>? rightVariables = VariableFinder(unaryExpression.RightOperand);

                    if (leftVariables != null && rightVariables != null)
                    {
                        foreach (char item in rightVariables)
                        {
                            _ = leftVariables.Add(item);
                        }

                        return leftVariables;
                    }
                    else if (leftVariables != null)
                    {
                        return leftVariables;
                    }
                    else if (rightVariables != null)
                    {
                        return rightVariables;
                    }

                    return null;
                }
                case MathExpressionType.Single:
                {
                    SingleMathExpression singleExpression = (SingleMathExpression)expression;

                    return VariableFinder(singleExpression.Operand);
                }
                case MathExpressionType.Variable:
                    return new HashSet<char> { ((VariableMathExpression)expression).Variable };
                case MathExpressionType.Constant:
                    return null;
                default:
                    throw new InvalidMathExpressionException($"Internal exception: {nameof(VariableFinder)} does not implement {nameof(MathExpressionType)}.");
            }
        }
    }
}
