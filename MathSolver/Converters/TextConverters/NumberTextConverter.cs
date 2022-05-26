using System.Globalization;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal class NumberTextConverter : BaseTextConverter
    {
        public override bool IsValid(TextConverterModel model)
        {
            return char.IsNumber(model.Current);
        }

        public override void Convert(TextConverterModel model)
        {
            int startIndex = model.Index;
            char letter = model.Current;

            while (char.IsNumber(letter) || letter == '.' || letter == ',')
            {
                model.Index++;
                letter = model.Current;
            }

            // . can also be used as a multiplication symbol
            if (model.Equation[model.Index - 1] == '.')
            {
                model.Index--;
            }

            string numberRange = model.Equation[startIndex..model.Index];

            if (double.TryParse(numberRange, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                model.Add(new ConstantEquationPart(number));
            }
            else
            {
                throw new InvalidMathExpressionException($"The provided number {numberRange} was not valid.");
            }
        }
    }
}
