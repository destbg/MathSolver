﻿namespace MathSolver.Expressions
{
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
                    _ => throw new Exception($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
                };
            }

            return result;
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix(Number.ToString(), this);
        }
    }
}
