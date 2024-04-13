using System.Collections.Generic;
using System.Linq;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters;

internal class BracketTextConverter : BaseTextConverter
{
    public override bool IsValid(TextConverterModel model)
    {
        char c = model.Current;

        return c is '(' or '[' or '<' or '{' or '|';
    }

    public override void Convert(TextConverterModel model)
    {
        BracketType bracketType = MapToBracketType(model.Current);
        int startIndex = model.Index;

        Dictionary<BracketType, int> brackets = new()
        {
            { BracketType.Straight, 0 },
            { BracketType.Parentheses, 0 },
            { BracketType.Square, 0 },
            { BracketType.Angle, 0 },
            { BracketType.Curly, 0 }
        };
        brackets[bracketType] = 1;

        while (brackets.Values.Any(f => f != 0) && model.Length > model.Index)
        {
            model.Index++;
            char c = model.Current;

            if (c == '|')
            {
                if (brackets[BracketType.Straight] == 0 || IsOpeningStraightBracket(model))
                {
                    brackets[BracketType.Straight]++;
                }
                else
                {
                    brackets[BracketType.Straight]--;
                }
            }
            else if (c is '(' or '[' or '<' or '{')
            {
                brackets[MapToBracketType(c)]++;
            }
            else if (c is ')' or ']' or '>' or '}')
            {
                brackets[MapToBracketType(c)]--;
            }
        }

        string expressionRange = model.Equation[(startIndex + 1)..model.Index];
        if (model.Count > 0 && model.Parts[^1].Type != EquationType.Symbol)
        {
            model.Add(new SymbolEquationPart(MathSymbol.Multiplication));
        }

        model.Add(new SubEquationPart(model.Coefficient, expressionRange, bracketType));
        model.Index++;
        model.Coefficient = null;
    }

    private bool IsOpeningStraightBracket(TextConverterModel model)
    {
        int i = model.Index - 1;

        while (model.Equation[i] == ' ') { i--; }

        char c = model.Equation[i];

        if (char.IsLetter(c) || char.IsNumber(c))
        {
            int coefficientIndex = i + 1;

            while (char.IsLetter(c) || char.IsNumber(c))
            {
                c = model.Equation[--i];
            }

            return MathParseHelper.IsValidCoefficient(model.Equation[(i + 1)..coefficientIndex]);
        }
        else if (c is '+' or '-' or '*' or '/' or '^' or '.')
        {
            return true;
        }

        return false;
    }

    private static BracketType MapToBracketType(char c)
    {
        return c switch
        {
            '(' or ')' => BracketType.Parentheses,
            '[' or ']' => BracketType.Square,
            '<' or '>' => BracketType.Angle,
            '}' or '{' => BracketType.Curly,
            '|' => BracketType.Straight,
            _ => throw new InvalidMathExpressionException($"Internal Exception: The method {nameof(MapToBracketType)} did not have a {nameof(BracketType)} implemented."),
        };
    }
}
