using System.Linq.Expressions;
using MathSolver.Converters;
using MathSolver.Enums;
using MathSolver.Expressions;
using MathSolver.Models;

namespace MathSolver
{
    public static class MathParser
    {
        public static MathExpression Parse(string equation)
        {
            return Parse(equation, null, false, false);
        }

        public static MathExpression Simplify(MathExpression expression)
        {
            switch (expression.Type)
            {
                case MathExpressionType.Variable:
                {
                    return expression;
                }
                case MathExpressionType.Constant:
                {
                    return new SolvedConstantMathExpression(expression.Solve(), expression.IsPercent, expression.IsFactorial);
                }
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryExpression = (UnaryMathExpression)expression;

                    MathExpression leftOperand = Simplify(unaryExpression.LeftOperand);
                    MathExpression rightOperand = Simplify(unaryExpression.RightOperand);

                    UnaryMathExpression newExpression = new(leftOperand, rightOperand, unaryExpression.Symbol)
                    {
                        Coefficient = unaryExpression.Coefficient,
                        IsFactorial = unaryExpression.IsFactorial,
                        IsPercent = unaryExpression.IsPercent
                    };

                    if (leftOperand.Type == MathExpressionType.Constant && rightOperand.Type == MathExpressionType.Constant)
                    {
                        return new ConstantMathExpression(newExpression.Solve(), false, false);
                    }
                    else
                    {
                        return newExpression;
                    }
                }
                default:
                    throw new Exception($"Internal Exception: The {nameof(Simplify)} method did not have a {nameof(MathExpressionType)} implemented.");
            }
        }

        public static (Expression Expression, List<ParameterExpression> Parameters) ConvertToCSharpExpression(MathExpression mathExpression)
        {
            MathExpressionToExpressionConverter mathExpressionToExpressionConverter = new(mathExpression);

            return (mathExpressionToExpressionConverter.Convert(), mathExpressionToExpressionConverter.Parameters);
        }

        internal static MathExpression Parse(string equation, string? coefficient, bool isPercent, bool isFactorial)
        {
            TextToEquationConverter parser = new(equation);

            List<EquationPart> expressions = parser.Convert();

            EquationToMathExpressionConveter converter = new(equation, expressions, coefficient, isPercent, isFactorial);

            return converter.Convert();
        }
    }
}
