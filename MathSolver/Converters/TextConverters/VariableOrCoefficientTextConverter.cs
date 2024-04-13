using System;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters;

internal class VariableOrCoefficientTextConverter : BaseTextConverter
{
    public override bool IsValid(TextConverterModel model)
    {
        return char.IsLetter(model.Current);
    }

    public override void Convert(TextConverterModel model)
    {
        int startIndex = model.Index;
        char letter = model.Current;

        while (char.IsLetter(letter) || char.IsNumber(letter)) // Checking for number because of log{num} and sqrt{num}
        {
            model.Index++;
            letter = model.Current;
        }

        if (startIndex == model.Index - 1)
        {
            char variable = model.Equation[startIndex];

            if (variable == 'e')
            {
                model.Add(new ConstantEquationPart(Math.E));
            }
            else
            {
                model.Add(new VariableEquationPart(variable));
            }
        }
        else
        {
            string coefficientRange = model.Equation[startIndex..model.Index].ToLower();

            if (coefficientRange == "pi")
            {
                model.Add(new ConstantEquationPart(Math.PI));
            }
            else if (coefficientRange == "tau")
            {
                model.Add(new ConstantEquationPart(6.2831853071795862));
            }
            else if (!HasBracket(model))
            {
                throw new InvalidMathExpressionException($"The provided coefficient {coefficientRange} was not followed by a bracket.");
            }
            else if (!MathParseHelper.IsValidCoefficient(coefficientRange))
            {
                throw new InvalidMathExpressionException($"The provided coefficient {coefficientRange} was not valid.");
            }
            else
            {
                model.Coefficient = coefficientRange;
            }
        }
    }

    private bool HasBracket(TextConverterModel model)
    {
        for (; model.Index < model.Length; model.Index++)
        {
            char c = model.Current;

            if (c != ' ')
            {
                return c is '(' or '|' or '[' or '<' or '{';
            }
        }

        return false;
    }
}
