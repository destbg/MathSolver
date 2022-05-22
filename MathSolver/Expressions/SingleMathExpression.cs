using System;
using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class SingleMathExpression : MathExpression
    {
        public SingleMathExpression(MathExpression operand, BracketType bracket, IReadOnlyList<MathSuffixSymbol> suffixSymbols, string? coefficient = null)
            : base(MathExpressionType.Single, suffixSymbols)
        {
            Operand = operand;
            Bracket = bracket;
            Coefficient = coefficient;
        }

        public MathExpression Operand { get; }
        public BracketType Bracket { get; }
        public string? Coefficient { get; }

        public override double Solve(params MathVariable[] variables)
        {
            double result = Operand.Solve(variables);

            foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
            {
                result = suffixSymbol switch
                {
                    MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                    MathSuffixSymbol.Percent => result / 100,
                    _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
                };
            }

            if (Bracket == BracketType.Straight)
            {
                result = Math.Abs(result);
            }

            if (!string.IsNullOrEmpty(Coefficient))
            {
                result = MathHelper.CalculateCoefficient(Coefficient, result);
            }

            return result;
        }

        public override string ToString()
        {
            return this.Suffix(Operand.ToString(), Bracket);
        }
    }
}
