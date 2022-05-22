using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            this.equation = equation.TrimEnd(' ') + " ";
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
                else if (letter == '+' || letter == '-' || letter == '*' || letter == '/' || letter == '^' || letter == '.')
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
                else if (letter == '(' || letter == '[' || letter == '<' || letter == '{' || letter == '|')
                {
                    FoundBracket();
                }
                else if (letter == ' ')
                {
                    index++;
                }
                else
                {
                    throw new InvalidMathExpressionException($"The provided letter {letter} was not valid.");
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
                throw new InvalidMathExpressionException($"The provided number {numberRange} was not valid.");
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
                _ => throw new InvalidMathExpressionException($"The provided math symbol {letter} was not valid."),
            };

            expressions.Add(new SymbolEquationPart(symbol));

            index++;
        }

        private void FoundFactorial()
        {
            if (expressions.Count < 1)
            {
                throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
            }

            EquationPart lastExpression = expressions[^1];

            if (lastExpression.Type == EquationType.Symbol)
            {
                throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
            }

            lastExpression.SuffixSymbols.Add(MathSuffixSymbol.Factorial);

            index++;
        }

        private void FoundPercentage()
        {
            if (expressions.Count < 1)
            {
                throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
            }

            EquationPart lastExpression = expressions[^1];

            if (lastExpression.Type == EquationType.Symbol)
            {
                throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
            }

            lastExpression.SuffixSymbols.Add(MathSuffixSymbol.Percent);

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
                string coefficientRange = equation[startIndex..index].ToLower();

                if (coefficientRange == "pi")
                {
                    expressions.Add(new ConstantEquationPart(Math.PI));
                }
                else if (coefficientRange == "tau")
                {
                    expressions.Add(new ConstantEquationPart(6.2831853071795862));
                }
                else if (!HasBracket())
                {
                    throw new InvalidMathExpressionException($"The provided coefficient {coefficientRange} was not followed by a bracket.");
                }
                else if (!IsValidCoefficient(coefficientRange))
                {
                    throw new InvalidMathExpressionException($"The provided coefficient {coefficientRange} was not valid.");
                }
                else
                {
                    FoundBracket(coefficientRange);
                }
            }
        }

        private void FoundBracket(string? coefficient = null)
        {
            BracketType bracketType = MapToBracketType(equation[index]);
            int startIndex = index;

            Dictionary<BracketType, int> brackets = new Dictionary<BracketType, int>
            {
                { BracketType.Straight, 0 },
                { BracketType.Parentheses, 0 },
                { BracketType.Square, 0 },
                { BracketType.Angle, 0 },
                { BracketType.Curly, 0 }
            };

            brackets[bracketType] = 1;

            while (brackets.Values.Any(f => f != 0) && equation.Length > index)
            {
                char c = equation[++index];

                if (c == '|')
                {
                    if (brackets[BracketType.Straight] == 0 || IsOpeningStraightBracket())
                    {
                        brackets[BracketType.Straight]++;
                    }
                    else
                    {
                        brackets[BracketType.Straight]--;
                    }
                }
                else if (c == '(' || c == '[' || c == '<' || c == '{')
                {
                    brackets[MapToBracketType(c)]++;
                }
                else if (c == ')' || c == ']' || c == '>' || c == '}')
                {
                    brackets[MapToBracketType(c)]--;
                }
            }

            string expressionRange = equation[(startIndex + 1)..index];

            if (expressions.Count > 0 && expressions[^1].Type != EquationType.Symbol)
            {
                expressions.Add(new SymbolEquationPart(MathSymbol.Multiplication));
            }

            expressions.Add(new SubEquationPart(coefficient, expressionRange, bracketType));
            index++;
        }

        private bool HasBracket()
        {
            for (; index < equation.Length; index++)
            {
                char c = equation[index];

                if (c != ' ')
                {
                    return c == '(' || c == '|' || c == '[' || c == '<' || c == '{';
                }
            }

            return false;
        }

        private bool IsOpeningStraightBracket()
        {
            int i = index - 1;

            while (equation[i] == ' ') { i--; }

            char c = equation[i];

            if (char.IsLetter(c) || char.IsNumber(c))
            {
                int coefficientIndex = i + 1;

                while (char.IsLetter(c) || char.IsNumber(c))
                {
                    c = equation[--i];
                }

                return IsValidCoefficient(equation[(i + 1)..coefficientIndex]);
            }
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '.')
            {
                return true;
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

        private static BracketType MapToBracketType(char c)
        {
            switch (c)
            {
                case '(':
                case ')':
                    return BracketType.Parentheses;
                case '[':
                case ']':
                    return BracketType.Square;
                case '<':
                case '>':
                    return BracketType.Angle;
                case '}':
                case '{':
                    return BracketType.Curly;
                case '|':
                    return BracketType.Straight;
                default:
                    throw new InvalidMathExpressionException($"Internal Exception: The method {nameof(MapToBracketType)} did not have a {nameof(BracketType)} implemented.");
            }
        }
    }
}
