using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathSolver.Converters.ExpressionConverters;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;

namespace MathSolver.Converters;

internal class MathExpressionToExpressionConverter
{
    private readonly ConditionExpressionConverter conditionExpressionConverter;
    private readonly ConstantExpressionConverter constantExpressionConverter;
    private readonly SingleExpressionConverter singleExpressionConverter;
    private readonly UnaryExpressionConverter unaryExpressionConverter;
    private readonly VariableExpressionConverter variableExpressionConverter;

    public MathExpressionToExpressionConverter(MathExpression mathExpression)
    {
        Parameters = (VariableFinder(mathExpression)?.ToList() ?? [])
            .ConvertAll(f => Expression.Parameter(typeof(double), f.ToString()));

        conditionExpressionConverter = new ConditionExpressionConverter(Convert);
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
            MathExpressionType.Condition => conditionExpressionConverter.Convert(mathExpression),
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
            case MathExpressionType.Condition:
            {
                ConditionMathExpression conditionExpression = (ConditionMathExpression)expression;

                HashSet<char> leftVariables = VariableFinder(conditionExpression.LeftCheck) ?? [];
                HashSet<char> rightVariables = VariableFinder(conditionExpression.RightCheck) ?? [];

                HashSet<char> ifTrueVariables = VariableFinder(conditionExpression.IfTrue) ?? [];
                HashSet<char> ifFalseVariables = VariableFinder(conditionExpression.IfFalse) ?? [];

                foreach (char item in rightVariables)
                {
                    _ = leftVariables.Add(item);
                }
                foreach (char item in ifTrueVariables)
                {
                    _ = leftVariables.Add(item);
                }
                foreach (char item in ifFalseVariables)
                {
                    _ = leftVariables.Add(item);
                }

                return leftVariables.Count == 0 ? null : leftVariables;
            }
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
                return [((VariableMathExpression)expression).Variable];
            case MathExpressionType.Constant:
                return null;
            default:
                throw new InvalidMathExpressionException($"Internal exception: {nameof(VariableFinder)} does not implement {nameof(MathExpressionType)}.");
        }
    }
}
