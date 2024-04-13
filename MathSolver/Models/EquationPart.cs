using System.Collections.Generic;
using MathSolver.Enums;

namespace MathSolver.Models;

internal abstract class EquationPart
{
    public EquationPart(EquationType type)
    {
        Type = type;
        SuffixSymbols = [];
    }

    public EquationType Type { get; }

    public List<MathSuffixSymbol> SuffixSymbols { get; }
}
