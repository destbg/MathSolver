using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions;

public class ConstantMathExpression : MathExpression
{
    public ConstantMathExpression(double number, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        : base(MathExpressionType.Constant, suffixSymbols)
    {
        Number = number;
    }

    public double Number { get; }

    public override double Solve(params MathVariable[] variables)
    {
        double result = Number;

        foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
        {
            result = suffixSymbol switch
            {
                MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                MathSuffixSymbol.Percent => result / 100,
                _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
            };
        }

        return result;
    }

    public override string ToString()
    {
        return this.Suffix(Number.ToString(), BracketType.None);
    }
}
