using System;
using System.Linq.Expressions;
using MathSolver.Expressions;

namespace MathSolver.Converters.ExpressionConverters
{
    internal class ConditionExpressionConverter
    {
        private readonly Func<MathExpression, Expression> convert;

        public ConditionExpressionConverter(Func<MathExpression, Expression> convert)
        {
            this.convert = convert;
        }

        public Expression Convert(MathExpression mathExpression)
        {
            ConditionMathExpression conditionMath = (ConditionMathExpression)mathExpression;

            Expression leftExpression = convert(conditionMath.LeftCheck);
            Expression rightExpression = convert(conditionMath.RightCheck);

            Expression ifTrueExpression = convert(conditionMath.IfTrue);
            Expression ifFalseExpression = convert(conditionMath.IfFalse);

            return Expression.Condition(
                conditionMath.IsEqual
                    ? Expression.Equal(leftExpression, rightExpression)
                    : Expression.NotEqual(leftExpression, rightExpression),
                ifTrueExpression,
                ifFalseExpression
            );
        }
    }
}
