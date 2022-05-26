using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathSolver.Converters.ExpressionConverters;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;

namespace MathSolver.Converters
{
    internal class MathExpressionToExpressionConverter
    {
        private readonly ConstantExpressionConverter constantExpressionConverter;
        private readonly SingleExpressionConverter singleExpressionConverter;
        private readonly UnaryExpressionConverter unaryExpressionConverter;
        private readonly VariableExpressionConverter variableExpressionConverter;

        public MathExpressionToExpressionConverter(MathExpression mathExpression)
        {
            Parameters = (VariableFinder(mathExpression)?.ToList() ?? new List<char>())
                .ConvertAll(f => Expression.Parameter(typeof(double), f.ToString()));

            constantExpressionConverter = new ConstantExpressionConverter();
            singleExpressionConverter = new SingleExpressionConverter(Parameters, Convert);
            unaryExpressionConverter = new UnaryExpressionConverter(Parameters, Convert);
            variableExpressionConverter = new VariableExpressionConverter(Parameters);
        }

        public List<ParameterExpression> Parameters { get; }

        public Expression Convert(MathExpression mathExpression)
        {
            return mathExpression.Type switch
            {
                MathExpressionType.Constant => constantExpressionConverter.Convert(mathExpression),
                MathExpressionType.Unary => unaryExpressionConverter.Convert(mathExpression),
                MathExpressionType.Variable => variableExpressionConverter.Convert(mathExpression),
                MathExpressionType.Single => singleExpressionConverter.Convert(mathExpression),
                _ => throw new InvalidMathExpressionException($"Internal Exception: The {nameof(Convert)} method did not have a {nameof(MathExpressionType)} implemented."),
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
                    else
                    {
                        return leftVariables ?? rightVariables;
                    }
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
