using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Helpers;

namespace MathSolver.Converters.ExpressionConverters
{
    internal class ConstantExpressionConverter
    {
        public Expression Convert(MathExpression mathExpression)
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
                        _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(ConstantExpressionConverter)} class does not implement {nameof(MathSuffixSymbol)}.")
                    };
                }

                return exp;
            }
        }
    }
}
