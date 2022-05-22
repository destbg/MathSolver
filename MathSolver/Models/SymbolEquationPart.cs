namespace MathSolver.Models
{
    internal class SymbolEquationPart : EquationPart
    {
        public SymbolEquationPart(MathSymbol symbol)
            : base(EquationType.Symbol)
        {
            Symbol = symbol;
        }

        public MathSymbol Symbol { get; }
    }
}
