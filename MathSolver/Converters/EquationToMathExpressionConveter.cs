namespace MathSolver.Converters
{
    internal class EquationToMathExpressionConveter
    {
        private readonly string equation;
        private readonly string? coefficient;
        private readonly IReadOnlyList<MathSuffixSymbol> suffixSymbols;
        private readonly List<EquationPart> expressions;

        public EquationToMathExpressionConveter(string equation, List<EquationPart> expressions, string? coefficient, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            this.equation = equation;
            this.expressions = expressions;
            this.coefficient = coefficient;
            this.suffixSymbols = suffixSymbols;
        }

        public MathExpression Convert()
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
            if (expressions[0].Type != EquationType.MathExpression)
            {
                MathExpression expression = ConvertExpression(expressions[0]);

                return new SingleMathExpression(expression, suffixSymbols)
                {
                    Coefficient = coefficient
                };
            }
            else
            {
                ExpressionEquationPart expression = (ExpressionEquationPart)expressions[0];
                UnaryMathExpression unaryExpression = (UnaryMathExpression)expression.MathExpression;

                unaryExpression.Coefficient = coefficient;
                unaryExpression.SuffixSymbols = suffixSymbols;

                return unaryExpression;
            }
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
                EquationType EquationType = expressions[i].Type;

                if (atSymbol && EquationType == EquationType.Symbol)
                {
                    atSymbol = false;
                }
                else if (!atSymbol && EquationType != EquationType.Symbol)
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
            EquationPart expression = expressions[indexToCheck];

            if (expression.Type == EquationType.Symbol)
            {
                MathSymbol symbol = ((SymbolEquationPart)expression).Symbol;

                if (symbol is MathSymbol.Addition or MathSymbol.Subraction)
                {
                    expressions.Insert(indexToInsert, new ConstantEquationPart(0d));
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
                EquationPart expression = expressions[i];

                if (expression.Type == EquationType.Symbol)
                {
                    if (((SymbolEquationPart)expression).Symbol is not MathSymbol.Addition and not MathSymbol.Subraction)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void SimplifyExpression(int expressionIndex)
        {
            EquationPart simpleLeft = expressions[expressionIndex - 1];
            EquationPart simpleSymbol = expressions[expressionIndex];
            EquationPart simpleRight = expressions[expressionIndex + 1];

            MathExpression leftExpression = ConvertExpression(simpleLeft);
            MathExpression rightExpression = ConvertExpression(simpleRight);

            MathSymbol symbol = ((SymbolEquationPart)simpleSymbol).Symbol;

            MathExpression mathExpression = new UnaryMathExpression(leftExpression, rightExpression, symbol, new List<MathSuffixSymbol>());

            ExpressionEquationPart newExpression = new(mathExpression);

            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.Insert(expressionIndex - 1, newExpression);
        }

        private MathExpression ConvertExpression(EquationPart expression)
        {
            if (expression.Type == EquationType.Expression)
            {
                SubEquationPart subEquation = (SubEquationPart)expression;

                MathExpression mathExpression = MathParser.Parse(subEquation.Expression, subEquation.Coefficient, subEquation.SuffixSymbols);

                return mathExpression;
            }
            else if (expression.Type == EquationType.Variable)
            {
                VariableEquationPart variableEquation = (VariableEquationPart)expression;

                return new VariableMathExpression(variableEquation.Variable, variableEquation.SuffixSymbols);
            }
            else if (expression.Type == EquationType.Number)
            {
                ConstantEquationPart constantEquation = (ConstantEquationPart)expression;

                return new ConstantMathExpression(constantEquation.Number, constantEquation.SuffixSymbols);
            }
            else if (expression.Type == EquationType.MathExpression)
            {
                return ((ExpressionEquationPart)expression).MathExpression;
            }

            throw new InvalidExpressionException($"The provided equation {equation} was not valid.");
        }
    }
}
