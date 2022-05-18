namespace MathSolver
{
    public class SimpleExpression
    {
        public SimpleExpression(double number)
        {
            Number = number;
            Type = ExpressionType.Number;
        }

        public SimpleExpression(char variable)
        {
            Variable = variable;
            Type = ExpressionType.Variable;
        }

        public SimpleExpression(MathSymbol symbol)
        {
            Symbol = symbol;
            Type = ExpressionType.Symbol;
        }

        public SimpleExpression(string? coefficient, string expression)
        {
            Coefficient = coefficient;
            Expression = expression;
            Type = ExpressionType.Expression;
        }

        public SimpleExpression(MathExpression mathExpression)
        {
            MathExpression = mathExpression;
            Type = ExpressionType.MathExpression;
        }

        public ExpressionType Type { get; }

        public double? Number { get; }
        public char? Variable { get; }
        public MathSymbol? Symbol { get; }
        public string? Coefficient { get; }
        public string? Expression { get; }
        public MathExpression? MathExpression { get; }

        public bool IsPercent { get; set; }
    }
}
