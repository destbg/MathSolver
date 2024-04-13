using System.Collections.Generic;

namespace MathSolver.Models;

internal class TextConverterModel
{
    public TextConverterModel(string equation)
    {
        Equation = equation;
        Parts = [];
        Length = equation.Length;
    }

    public string Equation { get; }
    public List<EquationPart> Parts { get; }

    public int Index { get; set; }
    public string? Coefficient { get; set; }

    public char Current => Equation[Index];
    public int Count => Parts.Count;
    public int Length { get; }

    public void Add(EquationPart part)
    {
        Parts.Add(part);
    }
}
