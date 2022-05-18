namespace MathSolver
{
    public class MathExpression
    {
        private readonly MathExpression? _leftExpression;
        private readonly MathExpression? _rightExpression;
        private readonly MathNumber? _leftNumber;
        private readonly MathNumber? _rightNumber;
        private readonly MathSymbol _symbol;

        private string? _coefficient;

        public MathExpression(string? coefficient, MathExpression leftExpression, MathSymbol symbol, MathExpression rightExpression)
        {
            _coefficient = coefficient;
            _symbol = symbol;
            _leftExpression = leftExpression;
            _rightExpression = rightExpression;
        }

        public MathExpression(string? coefficient, MathNumber leftNumber, MathSymbol symbol, MathExpression rightExpression)
        {
            _coefficient = coefficient;
            _symbol = symbol;
            _leftNumber = leftNumber;
            _rightExpression = rightExpression;
        }

        public MathExpression(string? coefficient, MathExpression leftExpression, MathSymbol symbol, MathNumber rightNumber)
        {
            _coefficient = coefficient;
            _symbol = symbol;
            _leftExpression = leftExpression;
            _rightNumber = rightNumber;
        }

        public MathExpression(string? coefficient, MathNumber leftNumber, MathSymbol symbol, MathNumber rightNumber)
        {
            _coefficient = coefficient;
            _symbol = symbol;
            _leftNumber = leftNumber;
            _rightNumber = rightNumber;
        }

        public void OverrideCoefficient(string? coefficient)
        {
            _coefficient = coefficient;
        }

        public double Solve(MathVariable[] variables)
        {
            double leftNumber = _leftNumber.HasValue
                ? _leftNumber.Value[variables]
                : _leftExpression!.Solve(variables);

            double rightNumber = _rightNumber.HasValue
                ? _rightNumber.Value[variables]
                : _rightExpression!.Solve(variables);

            double result = MathHelpers.CalculateSymbol(leftNumber, _symbol, rightNumber);

            return MathHelpers.CalculateCoefficient(_coefficient, result);
        }
    }
}
