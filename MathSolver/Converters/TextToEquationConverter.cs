using System.Collections.Generic;
using MathSolver.Converters.TextConverters;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters;

internal class TextToEquationConverter
{
    private readonly TextConverterModel converterModel;
    private readonly BaseTextConverter[] converters;

    public TextToEquationConverter(string equation)
    {
        converterModel = new TextConverterModel(equation.Trim() + ' ');

        converters = new BaseTextConverter[]
        {
            new NumberTextConverter(),
            new SymbolTextConverter(),
            new FactorialTextConverter(),
            new PercentageTextConverter(),
            new ConditionTextConverter(),
            new VariableOrCoefficientTextConverter(),
            new BracketTextConverter(),
            new WhiteSpaceTextConverter(),
        };
    }

    public List<EquationPart> Convert()
    {
        int length = converters.Length;

        while (converterModel.Index < converterModel.Length)
        {
            bool wasConverted = false;

            for (int i = 0; i < length; i++)
            {
                BaseTextConverter converter = converters[i];

                if (converter.IsValid(converterModel))
                {
                    converter.Convert(converterModel);
                    wasConverted = true;
                    break;
                }
            }

            if (!wasConverted)
            {
                throw new InvalidMathExpressionException($"The provided letter {converterModel.Current} was not valid.");
            }
        }

        return converterModel.Parts;
    }
}
