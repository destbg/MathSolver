using System.Collections.Generic;
using System.Linq.Expressions;
using MathSolver.Converters;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Models;

namespace MathSolver
{
    public static class MathParser
    {
        public static MathExpression Parse(string equation)
        {
            return Parse(equation, null, BracketType.None, new List<MathSuffixSymbol>());
        }

        public static MathExpression Simplify(MathExpression expression)
        {
            switch (expression.Type)
            {
                case MathExpressionType.Condition:
                {
                    ConditionMathExpression conditionExpression = (ConditionMathExpression)expression;

                    MathExpression leftExpression = Simplify(conditionExpression.LeftCheck);
                    MathExpression rightExpression = Simplify(conditionExpression.RightCheck);

                    MathExpression ifTrueExpression = Simplify(conditionExpression.IfTrue);
                    MathExpression ifFalseExpression = Simplify(conditionExpression.IfFalse);

                    ConditionMathExpression newExpression = new ConditionMathExpression(conditionExpression.IsEqual, leftExpression, rightExpression, ifTrueExpression, ifFalseExpression);

                    if (leftExpression.Type == MathExpressionType.Constant && rightExpression.Type == MathExpressionType.Constant
                        && ifTrueExpression.Type == MathExpressionType.Constant && ifFalseExpression.Type == MathExpressionType.Constant)
                    {
                        return new SolvedConstantMathExpression(newExpression.Solve(), conditionExpression.SuffixSymbols);
                    }
                    else
                    {
                        return newExpression;
                    }
                }
                case MathExpressionType.Variable:
                {
                    return expression;
                }
                case MathExpressionType.Constant:
                {
                    return new SolvedConstantMathExpression(expression.Solve(), expression.SuffixSymbols);
                }
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryExpression = (UnaryMathExpression)expression;

                    MathExpression leftOperand = Simplify(unaryExpression.LeftOperand);
                    MathExpression rightOperand = Simplify(unaryExpression.RightOperand);

                    UnaryMathExpression newExpression = new UnaryMathExpression(leftOperand, rightOperand, unaryExpression.Symbol, unaryExpression.Bracket, unaryExpression.SuffixSymbols, unaryExpression.Coefficient);

                    if (leftOperand.Type == MathExpressionType.Constant && rightOperand.Type == MathExpressionType.Constant)
                    {
                        return new SolvedConstantMathExpression(newExpression.Solve(), unaryExpression.SuffixSymbols);
                    }
                    else
                    {
                        return newExpression;
                    }
                }
                case MathExpressionType.Single:
                {
                    SingleMathExpression singleExpression = (SingleMathExpression)expression;

                    MathExpression operand = Simplify(singleExpression.Operand);

                    SingleMathExpression newExpression = new SingleMathExpression(operand, singleExpression.Bracket, singleExpression.SuffixSymbols, singleExpression.Coefficient);

                    if (operand.Type == MathExpressionType.Constant)
                    {
                        return new SolvedConstantMathExpression(newExpression.Solve(), singleExpression.SuffixSymbols);
                    }
                    else
                    {
                        return newExpression;
                    }
                }
                default:
                    throw new InvalidMathExpressionException($"Internal Exception: The {nameof(Simplify)} method did not have a {nameof(MathExpressionType)} implemented.");
            }
        }

        public static (Expression Expression, List<ParameterExpression> Parameters) ConvertToCSharpExpression(MathExpression mathExpression)
        {
            MathExpressionToExpressionConverter mathExpressionToExpressionConverter = new MathExpressionToExpressionConverter(mathExpression);

            return (mathExpressionToExpressionConverter.Convert(mathExpression), mathExpressionToExpressionConverter.Parameters);
        }

        internal static MathExpression Parse(string equation, string? coefficient, BracketType bracketType, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            TextToEquationConverter parser = new TextToEquationConverter(equation);

            List<EquationPart> expressions = parser.Convert();

            EquationToMathExpressionConveter converter = new EquationToMathExpressionConveter(expressions, coefficient, bracketType, suffixSymbols);

            return converter.Convert();
        }

        internal static MathExpression Parse(List<EquationPart> expressions)
        {
            EquationToMathExpressionConveter converter = new EquationToMathExpressionConveter(expressions, null, BracketType.None, new List<MathSuffixSymbol>());

            return converter.Convert();
        }
    }
}
