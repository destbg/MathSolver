using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Models;

namespace MathSolver.Expressions;

public class SolvedConstantMathExpression : ConstantMathExpression
{
    public SolvedConstantMathExpression(double number, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        : base(number, suffixSymbols) { }

    public override double Solve(params MathVariable[] variables)
    {
        return Number;
    }

    public override string ToString()
    {
        return Number.ToString();
    }
}
