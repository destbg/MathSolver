using System.Globalization;

namespace MathSolver
{
    public class MathExpressionParser
    {
        private readonly string equation;
        private readonly List<SimpleExpression> expressions;

        private int index;

        public MathExpressionParser(string equation)
        {
            this.equation = equation.ToLower().TrimEnd(' ') + " ";
            expressions = new List<SimpleExpression>();
            index = 0;
        }

        public List<SimpleExpression> Parse()
        {
            while (index < equation.Length)
            {
                char letter = equation[index];

                if (char.IsNumber(letter))
                {
                    FoundNumber(letter);
                }
                else if (MathHelpers.IsMathSymbol(letter))
                {
                    FoundMathSymbol(letter);
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
                expressions.Add(new SimpleExpression(number));
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
                '!' => MathSymbol.Factorial,
                _ => throw new InvalidExpressionException($"The provided math symbol {letter} was not valid."),
            };

            expressions.Add(new SimpleExpression(symbol));

            if (symbol == MathSymbol.Factorial)
            {
                // Check if the last number is percent or a symbol as well
                if (expressions.Count == 0 || expressions[^1].Type == ExpressionType.Symbol || expressions[^1].IsPercent)
                {
                    throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
                }

                expressions.Add(new SimpleExpression(0d));
            }

            index++;
        }

        private void FoundPercentage()
        {
            if (expressions.Count < 1)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            SimpleExpression lastExpression = expressions[^1];

            if (IsInvalidPercent(lastExpression))
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            lastExpression.IsPercent = true;

            index++;
        }

        private void FoundVariableOrCoefficient(char letter)
        {
            int startIndex = index;

            while (char.IsLetter(letter) || char.IsNumber(letter)) // Checking for number because of log10
            {
                letter = equation[++index];
            }

            if (startIndex == index - 1)
            {
                char variable = equation[startIndex];

                if (variable == 'e')
                {
                    expressions.Add(new SimpleExpression(Math.E));
                }
                else
                {
                    expressions.Add(new SimpleExpression(variable));
                }
            }
            else
            {
                string coefficientRange = equation[startIndex..index];

                if (coefficientRange == "pi")
                {
                    expressions.Add(new SimpleExpression(Math.PI));
                }
                else if (coefficientRange == "tau")
                {
                    expressions.Add(new SimpleExpression(Math.Tau));
                }
                else if (!HasBracket())
                {
                    throw new InvalidExpressionException($"The provided coefficient {coefficientRange} was not followed by a bracket.");
                }
                else if (!MathHelpers.IsValidCoefficient(coefficientRange))
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

            if (expressions.Count > 0 && expressions[^1].Type != ExpressionType.Symbol)
            {
                expressions.Add(new SimpleExpression(MathSymbol.Multiplication));
            }

            expressions.Add(new SimpleExpression(coefficient, expressionRange));
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

        private bool IsInvalidPercent(SimpleExpression lastExpression)
        {
            if (expressions.Count > 1)
            {
                // Third check is when the number is factorial, in which case it's the same as checking if it's ExpressionType.Symbol
                return lastExpression.Type == ExpressionType.Symbol
                    || lastExpression.IsPercent
                    || (expressions.Count > 1
                        && expressions[^2].Type == ExpressionType.Symbol
                        && expressions[^2].Symbol == MathSymbol.Factorial);
            }
            else
            {
                return lastExpression.Type == ExpressionType.Symbol
                    || lastExpression.IsPercent;
            }
        }
    }
}
