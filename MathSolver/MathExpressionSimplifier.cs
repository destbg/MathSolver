namespace MathSolver
{
    public class MathExpressionSimplifier
    {
        private readonly string equation;
        private readonly string? coefficient;
        private readonly bool isPercent;
        private readonly List<SimpleExpression> expressions;

        public MathExpressionSimplifier(string equation, List<SimpleExpression> expressions, string? coefficient, bool isPercent)
        {
            this.equation = equation;
            this.expressions = expressions;
            this.coefficient = coefficient;
            this.isPercent = isPercent;
        }

        public MathExpression Simplify()
        {
            InsertForSymbol(0, 0);
            InsertForSymbol(expressions.Count - 1, expressions.Count);
            CheckIfValidEquation();

            int expressionIndex = IndexOfImportantSymbol();

            while (expressionIndex != -1)
            {
                SimplifyExpression(expressionIndex);
                expressionIndex = IndexOfImportantSymbol();
            }

            while (expressions.Count > 1)
            {
                SimplifyExpression(1);
            }

            // This can happen if the expression is only a number
            if (expressions[0].Type != ExpressionType.MathExpression)
            {
                expressions.Add(new SimpleExpression(MathSymbol.Multiplication));
                expressions.Add(new SimpleExpression(1d));
                SimplifyExpression(1);
            }

            SimpleExpression expression = expressions[0];

            expression.MathExpression!.OverrideCoefficient(coefficient);
            expression.MathExpression!.IsPecent = isPercent;

            return expression.MathExpression!;
        }

        private void CheckIfValidEquation()
        {
            if (expressions.Count % 2 == 0)
            {
                throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
            }

            bool atSymbol = false;

            for (int i = 0; i < expressions.Count; i++)
            {
                ExpressionType expressionType = expressions[i].Type;

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
                    throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
                }
            }
        }

        private void InsertForSymbol(int indexToCheck, int indexToInsert)
        {
            SimpleExpression expression = expressions[indexToCheck];

            if (expression.Type == ExpressionType.Symbol)
            {
                MathSymbol expressionSymbol = expression.Symbol!.Value;

                if (expressionSymbol == MathSymbol.Addition || expressionSymbol == MathSymbol.Subraction || expressionSymbol == MathSymbol.Factorial)
                {
                    expressions.Insert(indexToInsert, new SimpleExpression(0d));
                }
                else
                {
                    throw new InvalidExpressionException($"The equation {equation} cannot start or end with a multiplication, division or power symbol.");
                }
            }
        }

        private int IndexOfImportantSymbol()
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                SimpleExpression expression = expressions[i];

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
            SimpleExpression simpleLeftExpression = expressions[expressionIndex - 1];
            SimpleExpression simpleExpressionSymbol = expressions[expressionIndex];
            SimpleExpression simpleRightExpression = expressions[expressionIndex + 1];

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

            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.Insert(expressionIndex - 1, newExpression);
        }

        private (MathExpression?, MathNumber?) ConvertExpression(SimpleExpression expression)
        {
            if (expression.Type == ExpressionType.Expression)
            {
                MathExpression mathExpression = MathParser.Parse(expression.Expression!, expression.Coefficient, expression.IsPercent);

                return (mathExpression, null);
            }
            else if (expression.Type == ExpressionType.Variable)
            {
                return (null, new MathNumber(expression.Variable!.Value, expression.IsPercent));
            }
            else if (expression.Type == ExpressionType.Number)
            {
                return (null, new MathNumber(expression.Number!.Value, expression.IsPercent));
            }
            else if (expression.Type == ExpressionType.MathExpression)
            {
                return (expression.MathExpression, null);
            }

            throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
        }
    }
}
