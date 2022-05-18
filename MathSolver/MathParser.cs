using System.Globalization;

namespace MathSolver
{
    public class MathParser
    {
        private readonly string _equation;
        private readonly string? _coefficient;
        private readonly List<SimpleExpression> _expressions;

        private int index;

        public MathParser(string equation, string? coefficient = null)
        {
            _equation = equation.ToLower().TrimEnd(' ') + " ";
            _coefficient = coefficient;
            _expressions = new List<SimpleExpression>();
            index = 0;
        }

        public MathExpression Parse()
        {
            while (index < _equation.Length)
            {
                CheckNextLetter();
            }

            InsertForSymbol(0, 0);
            InsertForSymbol(_expressions.Count - 1, _expressions.Count);
            CheckIfValidEquation();

            int expressionIndex = IndexOfImportantSymbol();

            while (expressionIndex != -1)
            {
                SimplifyExpression(expressionIndex);
                expressionIndex = IndexOfImportantSymbol();
            }

            while (_expressions.Count > 1)
            {
                SimplifyExpression(1);
            }

            // This can happen if the expression is only a number
            if (_expressions[0].Type != ExpressionType.MathExpression)
            {
                _expressions.Add(new SimpleExpression(MathSymbol.Addition));
                _expressions.Add(new SimpleExpression(0d));
                SimplifyExpression(1);
            }

            SimpleExpression expression = _expressions[0];

            expression.MathExpression!.OverrideCoefficient(_coefficient);

            return expression.MathExpression!;
        }

        private void CheckNextLetter()
        {
            char letter = _equation[index];

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
                throw new ArgumentException($"The provided letter {letter} was not valid.");
            }
        }

        private void FoundNumber(char letter)
        {
            int startIndex = index;

            while (char.IsNumber(letter) || letter == '.' || letter == ',')
            {
                letter = _equation[++index];
            }

            // . can also be used as a multiplication symbol
            if (_equation[index - 1] == '.')
            {
                index--;
            }

            string numberRange = _equation[startIndex..index];

            if (double.TryParse(numberRange, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                _expressions.Add(new SimpleExpression(number));
            }
            else
            {
                throw new ArgumentException($"The provided number {numberRange} was not valid.");
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
                _ => throw new Exception($"The provided math symbol {letter} was not valid."),
            };

            _expressions.Add(new SimpleExpression(symbol));

            if (symbol == MathSymbol.Factorial)
            {
                _expressions.Add(new SimpleExpression(0d));
            }

            index++;
        }

        private void FoundPercentage()
        {
            if (_expressions.Count < 1)
            {
                throw new InvalidExpressionException(_equation);
            }

            //SimpleExpression number = _expressions[^3];
            //SimpleExpression symbol = _expressions[^2];
            _expressions[^1].IsPercent = true;

            //if (number.Type == ExpressionType.Symbol || symbol.Type != ExpressionType.Symbol || latestNumber.Type == ExpressionType.Symbol)
            //{
            //    throw new InvalidExpressionException(_equation);
            //}

            //if (symbol.Symbol!.Value == MathSymbol.Addition || symbol.Symbol!.Value == MathSymbol.Subraction)
            //{
            //    (MathExpression? expression, MathNumber? mathNumber) = ConvertExpression(number);

            //    MathExpression mathExpression;

            //    if (expression != null)
            //    {
            //        mathExpression = new MathExpression(null, expression, MathSymbol.Division, new MathNumber(100d));
            //    }
            //    else
            //    {
            //        mathExpression = new MathExpression(null, mathNumber!.Value, MathSymbol.Division, new MathNumber(100d));
            //    }

            //    _expressions.Add(new SimpleExpression(MathSymbol.Multiplication));
            //    _expressions.Add(new SimpleExpression(mathExpression));
            //}
            //else if (symbol.Symbol!.Value == MathSymbol.Division)
            //{
            //    (MathExpression? expression, MathNumber? mathNumber) = ConvertExpression(latestNumber);

            //    MathExpression mathExpression;

            //    if (expression != null)
            //    {
            //        mathExpression = new MathExpression(null, expression, MathSymbol.Division, new MathNumber(100d));
            //    }
            //    else
            //    {
            //        mathExpression = new MathExpression(null, mathNumber!.Value, MathSymbol.Division, new MathNumber(100d));
            //    }

            //    _expressions[^1] = new SimpleExpression(mathExpression);
            //}
            //else
            //{
            //    _expressions.Add(new SimpleExpression(MathSymbol.Division));
            //    _expressions.Add(new SimpleExpression(100d));
            //}

            index++;
        }

        private void FoundVariableOrCoefficient(char letter)
        {
            int startIndex = index;

            while (char.IsLetter(letter) || char.IsNumber(letter)) // Checking for number because of log10
            {
                letter = _equation[++index];
            }

            if (startIndex == index - 1)
            {
                char variable = _equation[startIndex];

                if (variable == 'e')
                {
                    _expressions.Add(new SimpleExpression(Math.E));
                }
                else
                {
                    _expressions.Add(new SimpleExpression(variable));
                }
            }
            else
            {
                string coefficientRange = _equation[startIndex..index];

                if (coefficientRange == "pi")
                {
                    _expressions.Add(new SimpleExpression(Math.PI));
                }
                else if (coefficientRange == "tau")
                {
                    _expressions.Add(new SimpleExpression(Math.Tau));
                }
                else if (!HasBracket())
                {
                    throw new ArgumentException($"The provided coefficient {coefficientRange} was not followed by a bracket.");
                }
                else if (!MathHelpers.IsValidCoefficient(coefficientRange))
                {
                    throw new ArgumentException($"The provided coefficient {coefficientRange} was not valid.");
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
                char letter = _equation[++index];

                if (letter == '(')
                {
                    openingBracketCount++;
                }
                else if (letter == ')')
                {
                    openingBracketCount--;
                }
            }

            string expressionRange = _equation[(startIndex + 1)..index];

            _expressions.Add(new SimpleExpression(coefficient, expressionRange));
            index++;
        }

        private bool HasBracket()
        {
            for (; index < _equation.Length; index++)
            {
                char c = _equation[index];

                if (c != ' ')
                {
                    return c == '(';
                }
            }

            return false;
        }

        private void CheckIfValidEquation()
        {
            if (_expressions.Count % 2 == 0)
            {
                throw new InvalidExpressionException(_equation);
            }

            bool atSymbol = false;

            for (int i = 0; i < _expressions.Count; i++)
            {
                ExpressionType expressionType = _expressions[i].Type;

                if (atSymbol && expressionType == ExpressionType.Symbol)
                {
                    atSymbol = false;
                }
                else if (!atSymbol && expressionType != ExpressionType.Symbol)
                {
                    atSymbol = true;
                }
                else
                {
                    throw new InvalidExpressionException(_equation);
                }
            }
        }

        private void InsertForSymbol(int indexToCheck, int indexToInsert)
        {
            SimpleExpression expression = _expressions[indexToCheck];

            if (expression.Type == ExpressionType.Symbol)
            {
                MathSymbol expressionSymbol = expression.Symbol!.Value;

                if (expressionSymbol == MathSymbol.Addition || expressionSymbol == MathSymbol.Subraction || expressionSymbol == MathSymbol.Factorial)
                {
                    _expressions.Insert(indexToInsert, new SimpleExpression(0d));
                }
                else
                {
                    throw new ArgumentException($"The equation {_equation} cannot start or end with a multiplication, division or power symbol.");
                }
            }
        }

        private int IndexOfImportantSymbol()
        {
            for (int i = 0; i < _expressions.Count; i++)
            {
                SimpleExpression expression = _expressions[i];

                if (expression.Type == ExpressionType.Symbol)
                {
                    if (expression.Symbol != MathSymbol.Addition && expression.Symbol != MathSymbol.Subraction)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void SimplifyExpression(int expressionIndex)
        {
            SimpleExpression simpleLeftExpression = _expressions[expressionIndex - 1];
            SimpleExpression simpleExpressionSymbol = _expressions[expressionIndex];
            SimpleExpression simpleRightExpression = _expressions[expressionIndex + 1];

            (MathExpression? leftExpression, MathNumber? leftNumber) = ConvertExpression(simpleLeftExpression);
            (MathExpression? rightExpression, MathNumber? rightNumber) = ConvertExpression(simpleRightExpression);

            MathSymbol symbol = simpleExpressionSymbol.Symbol!.Value;

            MathExpression mathExpression;

            if (leftExpression != null && rightExpression != null)
            {
                mathExpression = new MathExpression(simpleLeftExpression.Coefficient, leftExpression, symbol, rightExpression);
            }
            else if (leftNumber != null && rightExpression != null)
            {
                mathExpression = new MathExpression(simpleLeftExpression.Coefficient, leftNumber.Value, symbol, rightExpression);
            }
            else if (leftExpression != null && rightNumber != null)
            {
                mathExpression = new MathExpression(simpleLeftExpression.Coefficient, leftExpression, symbol, rightNumber.Value);
            }
            else
            {
                mathExpression = new MathExpression(simpleLeftExpression.Coefficient, leftNumber!.Value, symbol, rightNumber!.Value);
            }

            SimpleExpression newExpression = new(mathExpression);

            _expressions.RemoveAt(expressionIndex - 1);
            _expressions.RemoveAt(expressionIndex - 1);
            _expressions.RemoveAt(expressionIndex - 1);
            _expressions.Insert(expressionIndex - 1, newExpression);
        }

        private (MathExpression?, MathNumber?) ConvertExpression(SimpleExpression expression)
        {
            if (expression.Type == ExpressionType.Expression)
            {
                MathParser mathParser = new(expression.Expression!, expression.Coefficient);
                return (mathParser.Parse(), null);
            }
            else if (expression.Type == ExpressionType.Variable)
            {
                return (null, new MathNumber(expression.Variable!.Value));
            }
            else if (expression.Type == ExpressionType.Number)
            {
                return (null, new MathNumber(expression.Number!.Value));
            }
            else if (expression.Type == ExpressionType.MathExpression)
            {
                return (expression.MathExpression, null);
            }

            throw new InvalidExpressionException(_equation);
        }
    }
}
