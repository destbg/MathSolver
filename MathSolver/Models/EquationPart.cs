namespace MathSolver.Models
{
    internal abstract class EquationPart
    {
        public EquationPart(EquationType type)
        {
            Type = type;
            SuffixSymbols = new List<MathSuffixSymbol>();
        }

        public EquationType Type { get; }

        public List<MathSuffixSymbol> SuffixSymbols { get; }
    }
}
