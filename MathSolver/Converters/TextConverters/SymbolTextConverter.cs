using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal class SymbolTextConverter : BaseTextConverter
    {
        public override bool IsValid(TextConverterModel model)
        {
            char c = model.Current;

            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '.';
        }

        public override void Convert(TextConverterModel model)
        {
            MathSymbol symbol = model.Current switch
            {
                '+' => MathSymbol.Addition,
                '-' => MathSymbol.Subraction,
                '*' => MathSymbol.Multiplication,
                '.' => MathSymbol.Multiplication,
                '/' => MathSymbol.Division,
                '^' => MathSymbol.Power,
                _ => throw new InvalidMathExpressionException($"The provided math symbol {model.Current} was not valid."),
            };

            model.Add(new SymbolEquationPart(symbol));

            model.Index++;
        }
    }
}
