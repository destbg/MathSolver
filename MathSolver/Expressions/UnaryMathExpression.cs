using System;
using System.Collections.Generic;
using System.Data;
using MathSolver.Enums;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions;

public class UnaryMathExpression : MathExpression
{
    public UnaryMathExpression(MathExpression leftOperand, MathExpression rightOperand, MathSymbol symbol, BracketType bracket, IReadOnlyList<MathSuffixSymbol> suffixSymbols, string? coefficient = null)
        : base(MathExpressionType.Unary, suffixSymbols)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        Symbol = symbol;
        Bracket = bracket;
        Coefficient = coefficient;
    }

    public MathExpression LeftOperand { get; }
    public MathExpression RightOperand { get; }
    public MathSymbol Symbol { get; }
    public BracketType Bracket { get; }
    public string? Coefficient { get; }

    public override double Solve(params MathVariable[] variables)
    {
        double leftNumber = LeftOperand.Solve(variables);
        double rightNumber = RightOperand.Solve(variables);

        bool leftHasPercent = LeftOperand.SuffixSymbols.Count != 0 && LeftOperand.SuffixSymbols[^1] == MathSuffixSymbol.Percent;
        bool rightHasPercent = RightOperand.SuffixSymbols.Count != 0 && RightOperand.SuffixSymbols[^1] == MathSuffixSymbol.Percent;

        if (leftHasPercent || rightHasPercent)
        {
            if (leftHasPercent && rightHasPercent)
            {
                rightNumber = FindPercent(leftNumber, rightNumber, Symbol);
            }
            else if (leftHasPercent)
            {
                leftNumber = FindPercent(rightNumber, leftNumber, Symbol);
            }
            else
            {
                rightNumber = FindPercent(leftNumber, rightNumber, Symbol);
            }
        }

        double result = Symbol switch
        {
            MathSymbol.Addition => leftNumber + rightNumber,
            MathSymbol.Subraction => leftNumber - rightNumber,
            MathSymbol.Multiplication => leftNumber * rightNumber,
            MathSymbol.Division => leftNumber / rightNumber,
            MathSymbol.Power => Math.Pow(leftNumber, rightNumber),
            _ => throw new InvalidExpressionException($"The provided symbol {Symbol} was not valid."),
        };

        foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
        {
            result = suffixSymbol switch
            {
                MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                MathSuffixSymbol.Percent => result / 100,
                _ => throw new InvalidExpressionException($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
            };
        }

        if (Bracket == BracketType.Straight)
        {
            result = Math.Abs(result);
        }

        if (!string.IsNullOrEmpty(Coefficient))
        {
            result = MathHelper.Coefficient(Coefficient, result);
        }

        return result;
    }

    public override string ToString()
    {
        return this.Suffix($"{LeftOperand} {ToStringHelper.SymbolEnumToChar(Symbol)} {RightOperand}", Bracket);
    }

    private static double FindPercent(double num, double percent, MathSymbol symbol)
    {
        return symbol is MathSymbol.Addition or MathSymbol.Subraction
            ? num * percent
            : percent;
    }
}
