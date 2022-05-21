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

                    if (variableMath.IsPercent || variableMath.IsFactorial)
                    {
                        return Expression.Call(
                            typeof(MathHelper),
                            nameof(MathHelper.CalculateNumberSuffix),
                            null,
                            parameter,
                            Expression.Constant(variableMath.IsPercent),
                            Expression.Constant(variableMath.IsFactorial)
                        );
                    }
                    else
                    {
                        return parameter;
                    }
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

                        if (constantMath.IsPercent || constantMath.IsFactorial)
                        {
                            return Expression.Call(
                                typeof(MathHelper),
                                nameof(MathHelper.CalculateNumberSuffix),
                                null,
                                Expression.Constant(constantMath.Number),
                                Expression.Constant(constantMath.IsPercent),
                                Expression.Constant(constantMath.IsFactorial)
                            );
                        }
                        else
                        {
                            return Expression.Constant(constantMath.Number);
                        }
                    }
                }
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryMath = (UnaryMathExpression)mathExpression;

                    Expression leftExpression = ParseMathToCSharp(unaryMath.LeftOperand);
                    Expression rightExpression = ParseMathToCSharp(unaryMath.RightOperand);

                    Expression resultExpression = CreateEquation(unaryMath, leftExpression, rightExpression);

                    if (unaryMath.IsPercent || unaryMath.IsFactorial)
                    {
                        resultExpression = Expression.Call(
                            typeof(MathHelper),
                            nameof(MathHelper.CalculateNumberSuffix),
                            null,
                            resultExpression,
                            Expression.Constant(unaryMath.IsPercent),
                            Expression.Constant(unaryMath.IsFactorial)
                        );
                    }

                    if (!string.IsNullOrEmpty(unaryMath.Coefficient))
                    {
                        (string method, Expression[] parameters) = CreateCoefficientCall(unaryMath.Coefficient, resultExpression);

                        resultExpression = Expression.Call(typeof(Math), method, null, parameters);
                    }

                    return resultExpression;
                }
                default:
                    throw new Exception($"Internal Exception: The {nameof(ParseMathToCSharp)} method did not have a {nameof(MathExpressionType)} implemented.");
            }
        }

        private static Expression CreateEquation(UnaryMathExpression unaryMath, Expression leftExpression, Expression rightExpression)
        {
            if (unaryMath.LeftOperand.IsPercent || unaryMath.RightOperand.IsPercent)
            {
                if (unaryMath.LeftOperand.IsPercent && !unaryMath.RightOperand.IsPercent)
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
                _ => throw new Exception($"Internal Exception: Method {nameof(CreateEquation)} does not have a {nameof(MathSymbol)} implemented."),
            };
        }

        private static (string, Expression[]) CreateCoefficientCall(string coefficient, Expression expression)
        {
            if (coefficient.StartsWith("log"))
            {
                if (coefficient == "log2")
                {
                    return (nameof(Math.Log2), new[] { expression });
                }
                else if (coefficient == "log10")
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

                ConstantExpression nthRoot = Expression.Constant(int.Parse(coefficient.Replace("sqrt", string.Empty)));
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
                _ => throw new InvalidExpressionException($"The provided coefficient {coefficient} was not valid."),
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

                    goto default;
                }
                case MathExpressionType.Variable:
                    return new HashSet<char> { ((VariableMathExpression)expression).Variable };
                default:
                    return null;
            }
        }
    }
}
