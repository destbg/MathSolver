using System.Globalization;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters
{
    internal class TextToEquationConverter
    {
        private readonly string equation;
        private readonly List<EquationPart> expressions;

        private int index;

        public TextToEquationConverter(string equation)
        {
            this.equation = equation.ToLower().TrimEnd(' ') + " ";
            expressions = new List<EquationPart>();
            index = 0;
        }

        public List<EquationPart> Convert()
        {
            while (index < equation.Length)
            {
                char letter = equation[index];

                if (char.IsNumber(letter))
                {
                    FoundNumber(letter);
                }
                else if (letter is '+' or '-' or '*' or '/' or '^' or '.')
                {
                    FoundMathSymbol(letter);
                }
                else if (letter == '!')
                {
                    FoundFactorial();
                }
                else if (letter == '%')
                {
                    FoundPercentage();
                }
                else if (char.IsLetter(letter))
                {
                    FoundVariableOrCoefficient(letter);
                }
                else if (letter == '(')
                {
                    FoundBracket();
                }
                else if (letter == ' ')
                {
                    index++;
                }
                else
                {
                    throw new InvalidExpressionException($"The provided letter {letter} was not valid.");
                }
            }

            return expressions;
        }

        private void FoundNumber(char letter)
        {
            int startIndex = index;

            while (char.IsNumber(letter) || letter == '.' || letter == ',')
            {
                letter = equation[++index];
            }

            // . can also be used as a multiplication symbol
            if (equation[index - 1] == '.')
            {
                index--;
            }

            string numberRange = equation[startIndex..index];

            if (double.TryParse(numberRange, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                expressions.Add(new ConstantEquationPart(number));
            }
            else
            {
                throw new InvalidExpressionException($"The provided number {numberRange} was not valid.");
            }
        }

        private void FoundMathSymbol(char letter)
        {
            MathSymbol symbol = letter switch
            {
                '+' => MathSymbol.Addition,
                '-' => MathSymbol.Subraction,
                '*' => MathSymbol.Multiplication,
                '.' => MathSymbol.Multiplication,
                '/' => MathSymbol.Division,
                '^' => MathSymbol.Power,
                _ => throw new InvalidExpressionException($"The provided math symbol {letter} was not valid."),
            };

            expressions.Add(new SymbolEquationPart(symbol));

            index++;
        }

        private void FoundFactorial()
        {
            if (expressions.Count < 1)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            EquationPart lastExpression = expressions[^1];

            if (lastExpression.Type == EquationType.Symbol || lastExpression.IsPercent)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            lastExpression.IsFactorial = true;

            index++;
        }

        private void FoundPercentage()
        {
            if (expressions.Count < 1)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            EquationPart lastExpression = expressions[^1];

            if (lastExpression.Type == EquationType.Symbol || lastExpression.IsPercent)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            lastExpression.IsPercent = true;

            index++;
        }

        private void FoundVariableOrCoefficient(char letter)
        {
            int startIndex = index;

            while (char.IsLetter(letter) || char.IsNumber(letter)) // Checking for number because of log{num} and sqrt{num}
            {
                letter = equation[++index];
            }

            if (startIndex == index - 1)
            {
                char variable = equation[startIndex];

                if (variable == 'e')
                {
                    expressions.Add(new ConstantEquationPart(Math.E));
                }
                else
                {
                    expressions.Add(new VariableEquationPart(variable));
                }
            }
            else
            {
                string coefficientRange = equation[startIndex..index];

                if (coefficientRange == "pi")
                {
                    expressions.Add(new ConstantEquationPart(Math.PI));
                }
                else if (coefficientRange == "tau")
                {
                    expressions.Add(new ConstantEquationPart(Math.Tau));
                }
                else if (!HasBracket())
                {
                    throw new InvalidExpressionException($"The provided coefficient {coefficientRange} was not followed by a bracket.");
                }
                else if (!IsValidCoefficient(coefficientRange))
                {
                    throw new InvalidExpressionException($"The provided coefficient {coefficientRange} was not valid.");
                }
                else
                {
                    FoundBracket(coefficientRange);
                }
            }
        }

        private void FoundBracket(string? coefficient = null)
        {
            int startIndex = index;
            int openingBracketCount = 1;

            while (openingBracketCount > 0)
            {
                char letter = equation[++index];

                if (letter == '(')
                {
                    openingBracketCount++;
                }
                else if (letter == ')')
                {
                    openingBracketCount--;
                }
            }

            string expressionRange = equation[(startIndex + 1)..index];

            if (expressions.Count > 0 && expressions[^1].Type != EquationType.Symbol)
            {
                expressions.Add(new SymbolEquationPart(MathSymbol.Multiplication));
            }

            expressions.Add(new SubEquationPart(coefficient, expressionRange));
            index++;
        }

        private bool HasBracket()
        {
            for (; index < equation.Length; index++)
            {
                char c = equation[index];

                if (c != ' ')
                {
                    return c == '(';
                }
            }

            return false;
        }

        private static bool IsValidCoefficient(string coefficient)
        {
            if (coefficient.StartsWith("log"))
            {
                return int.TryParse(coefficient.Replace("log", string.Empty), out _);
            }

            if (coefficient.StartsWith("sqrt"))
            {
                return coefficient == "sqrt"
                    || int.TryParse(coefficient.Replace("sqrt", string.Empty), out _);
            }

            return coefficient switch
            {
                "abs" => true,
                "acos" => true,
                "acosh" => true,
                "asin" => true,
                "asinh" => true,
                "atan" => true,
                "atanh" => true,
                "cbrt" => true,
                "ceil" => true,
                "cos" => true,
                "cosh" => true,
                "floor" => true,
                "round" => true,
                "sign" => true,
                "sin" => true,
                "sinh" => true,
                "tan" => true,
                "tanh" => true,
                "trunc" => true,
                _ => false,
            };
        }
    }
}
