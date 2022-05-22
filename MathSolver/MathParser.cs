using System.Linq.Expressions;

namespace MathSolver
{
    public static class MathParser
    {
        public static MathExpression Parse(string equation)
        {
            return Parse(equation, null, new List<MathSuffixSymbol>());
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
                    return new SolvedConstantMathExpression(expression.Solve(), expression.SuffixSymbols);
                }
                case MathExpressionType.Unary:
                {
                    UnaryMathExpression unaryExpression = (UnaryMathExpression)expression;

                    MathExpression leftOperand = Simplify(unaryExpression.LeftOperand);
                    MathExpression rightOperand = Simplify(unaryExpression.RightOperand);

                    UnaryMathExpression newExpression = new(leftOperand, rightOperand, unaryExpression.Symbol, unaryExpression.SuffixSymbols)
                    {
                        Coefficient = unaryExpression.Coefficient,
                    };

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

                    SingleMathExpression newExpression = new(operand, singleExpression.SuffixSymbols)
                    {
                        Coefficient = singleExpression.Coefficient,
                    };

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
                    throw new Exception($"Internal Exception: The {nameof(Simplify)} method did not have a {nameof(MathExpressionType)} implemented.");
            }
        }

        public static (Expression Expression, List<ParameterExpression> Parameters) ConvertToCSharpExpression(MathExpression mathExpression)
        {
            MathExpressionToExpressionConverter mathExpressionToExpressionConverter = new(mathExpression);

            return (mathExpressionToExpressionConverter.Convert(), mathExpressionToExpressionConverter.Parameters);
        }

        internal static MathExpression Parse(string equation, string? coefficient, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            TextToEquationConverter parser = new(equation);

            List<EquationPart> expressions = parser.Convert();

            EquationToMathExpressionConveter converter = new(equation, expressions, coefficient, suffixSymbols);

            return converter.Convert();
        }
    }
}
