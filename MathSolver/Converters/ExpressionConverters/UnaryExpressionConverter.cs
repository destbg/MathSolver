using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Helpers;

namespace MathSolver.Converters.ExpressionConverters;

internal class UnaryExpressionConverter
{
    private readonly List<ParameterExpression> parameters;
    private readonly Func<MathExpression, Expression> convert;

    public UnaryExpressionConverter(List<ParameterExpression> parameters, Func<MathExpression, Expression> convert)
    {
        this.parameters = parameters;
        this.convert = convert;
    }

    public Expression Convert(MathExpression mathExpression)
    {
        UnaryMathExpression unaryMath = (UnaryMathExpression)mathExpression;

        Expression leftExpression = convert(unaryMath.LeftOperand);
        Expression rightExpression = convert(unaryMath.RightOperand);

        Expression exp = MathExpressionHelper.CreateEquation(unaryMath, leftExpression, rightExpression);

        foreach (MathSuffixSymbol suffixSymbol in unaryMath.SuffixSymbols)
        {
            exp = suffixSymbol switch
            {
                MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(UnaryExpressionConverter)} class does not implement {nameof(MathSuffixSymbol)}.")
            };
        }

        if (unaryMath.Bracket == BracketType.Straight)
        {
            exp = Expression.Call(typeof(Math), nameof(Math.Abs), null, exp);
        }

        if (!string.IsNullOrEmpty(unaryMath.Coefficient))
        {
            (string method, Expression[] parameters) = MathExpressionHelper.CreateCoefficientCall(unaryMath.Coefficient, exp);

            exp = Expression.Call(typeof(Math), method, null, parameters);
        }

        return exp;
    }
}
