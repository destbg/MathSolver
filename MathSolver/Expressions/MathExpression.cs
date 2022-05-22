using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public abstract class MathExpression
    {
        public MathExpression(MathExpressionType type, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            Type = type;
            SuffixSymbols = suffixSymbols;
        }

        public MathExpressionType Type { get; }

        public IReadOnlyList<MathSuffixSymbol> SuffixSymbols { get; }

        public abstract double Solve(params MathVariable[] variables);

        public override string ToString()
        {
            return Type + " not implemented.";
        }
    }
}
