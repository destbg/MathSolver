using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Helpers;

namespace MathSolver.Converters.ExpressionConverters;

internal class SingleExpressionConverter
{
    private readonly List<ParameterExpression> parameters;
    private readonly Func<MathExpression, Expression> convert;

    public SingleExpressionConverter(List<ParameterExpression> parameters, Func<MathExpression, Expression> convert)
    {
        this.parameters = parameters;
        this.convert = convert;
    }

    public Expression Convert(MathExpression mathExpression)
    {
        SingleMathExpression singleMath = (SingleMathExpression)mathExpression;

        Expression exp = convert(singleMath.Operand);

        foreach (MathSuffixSymbol suffixSymbol in singleMath.SuffixSymbols)
        {
            exp = suffixSymbol switch
            {
                MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(SingleExpressionConverter)} class does not implement {nameof(MathSuffixSymbol)}.")
            };
        }

        if (singleMath.Bracket == BracketType.Straight)
        {
            exp = Expression.Call(typeof(Math), nameof(Math.Abs), null, exp);
        }

        if (!string.IsNullOrEmpty(singleMath.Coefficient))
        {
            (string method, Expression[] parameters) = MathExpressionHelper.CreateCoefficientCall(singleMath.Coefficient, exp);

            exp = Expression.Call(typeof(Math), method, null, parameters);
        }

        return exp;
    }
}
