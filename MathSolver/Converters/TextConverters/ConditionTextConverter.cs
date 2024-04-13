using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters;

internal class ConditionTextConverter : BaseTextConverter
{
    public override bool IsValid(TextConverterModel model)
    {
        char c = model.Current;

        return c == '=' || c == '?' || c == ':' || (c == '!' && model.Length != model.Index + 1 && model.Equation[model.Index + 1] == '=');
    }

    public override void Convert(TextConverterModel model)
    {
        if (model.Count < 1 || model.Parts[^1].Type == EquationType.Symbol || model.Length == model.Index + 1)
        {
            throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
        }

        switch (model.Current)
        {
            case '=':
                if (model.Equation[model.Index + 1] == '=')
                {
                    model.Index++;
                }

                model.Add(new ConditionEquationPart(ConditionType.Equal));
                break;
            case '!':
                model.Index++;

                model.Add(new ConditionEquationPart(ConditionType.NotEqual));
                break;
            case '?':
                model.Add(new ConditionEquationPart(ConditionType.True));
                break;
            case ':':
                model.Add(new ConditionEquationPart(ConditionType.False));
                break;
            default:
                throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
        }

        model.Index++;
    }
}
