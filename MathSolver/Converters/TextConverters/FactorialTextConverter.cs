using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal class FactorialTextConverter : BaseTextConverter
    {
        public override bool IsValid(TextConverterModel model)
        {
            return model.Current == '!';
        }

        public override void Convert(TextConverterModel model)
        {
            if (model.Count < 1)
            {
                throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
            }

            EquationPart lastExpression = model.Parts[^1];

            if (lastExpression.Type == EquationType.Symbol)
            {
                throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
            }

            lastExpression.SuffixSymbols.Add(MathSuffixSymbol.Factorial);

            model.Index++;
        }
    }
}
