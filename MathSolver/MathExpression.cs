namespace MathSolver
{
    public class MathExpression
    {
        private readonly MathExpression? leftExpression;
        private readonly MathExpression? rightExpression;
        private readonly MathNumber? leftNumber;
        private readonly MathNumber? rightNumber;
        private readonly MathSymbol symbol;

        private string? coefficient;

        public MathExpression(string? coefficient, MathExpression leftExpression, MathSymbol symbol, MathExpression rightExpression)
        {
            this.coefficient = coefficient;
            this.symbol = symbol;
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }

        public MathExpression(string? coefficient, MathNumber leftNumber, MathSymbol symbol, MathExpression rightExpression)
        {
            this.coefficient = coefficient;
            this.symbol = symbol;
            this.leftNumber = leftNumber;
            this.rightExpression = rightExpression;
        }

        public MathExpression(string? coefficient, MathExpression leftExpression, MathSymbol symbol, MathNumber rightNumber)
        {
            this.coefficient = coefficient;
            this.symbol = symbol;
            this.leftExpression = leftExpression;
            this.rightNumber = rightNumber;
        }

        public MathExpression(string? coefficient, MathNumber leftNumber, MathSymbol symbol, MathNumber rightNumber)
        {
            this.coefficient = coefficient;
            this.symbol = symbol;
            this.leftNumber = leftNumber;
            this.rightNumber = rightNumber;
        }

        public bool IsPecent { get; set; }

        public void OverrideCoefficient(string? coefficient)
        {
            this.coefficient = coefficient;
        }

        public double Solve(MathVariable[] variables)
        {
            SolvedNumber result = SolveInternally(variables);

            return result.IsPercent
                ? (result.Number / 100)
                : result.Number;
        }

        private SolvedNumber SolveInternally(MathVariable[] variables)
        {
            SolvedNumber solvedLeftNumber = leftNumber.HasValue
                ? leftNumber.Value.Get(variables)
                : leftExpression!.SolveInternally(variables);

            SolvedNumber solvedRightNumber = rightNumber.HasValue
                ? rightNumber.Value.Get(variables)
                : rightExpression!.SolveInternally(variables);

            double calculatedNumber = CalculateSymbol(solvedLeftNumber, symbol, solvedRightNumber);
            double result = MathHelpers.CalculateCoefficient(coefficient, calculatedNumber);

            return new SolvedNumber(result, IsPecent);
        }

        private static double CalculateSymbol(SolvedNumber leftNum, MathSymbol symbol, SolvedNumber rightNum)
        {
            double leftNumber;
            double rightNumber;

            if (!leftNum.IsPercent && !rightNum.IsPercent)
            {
                leftNumber = leftNum.Number;
                rightNumber = rightNum.Number;
            }
            else if (leftNum.IsPercent && rightNum.IsPercent)
            {
                leftNumber = leftNum.Number / 100;
                rightNumber = CalculatePercentage(leftNumber, rightNum.Number, symbol);
            }
            else if (leftNum.IsPercent)
            {
                leftNumber = CalculatePercentage(rightNum.Number, leftNum.Number, symbol);
                rightNumber = rightNum.Number;
            }
            else
            {
                leftNumber = leftNum.Number;
                rightNumber = CalculatePercentage(leftNum.Number, rightNum.Number, symbol);
            }

            return symbol switch
            {
                MathSymbol.Addition => leftNumber + rightNumber,
                MathSymbol.Subraction => leftNumber - rightNumber,
                MathSymbol.Multiplication => leftNumber * rightNumber,
                MathSymbol.Division => leftNumber / rightNumber,
                MathSymbol.Power => Math.Pow(leftNumber, rightNumber),
                MathSymbol.Factorial => MathHelpers.Factorial(leftNumber),
                _ => throw new InvalidExpressionException($"The provided symbol {symbol} was not valid."),
            };
        }

        private static double CalculatePercentage(double num, double percent, MathSymbol symbol)
        {
            switch (symbol)
            {
                case MathSymbol.Addition:
                case MathSymbol.Subraction:
                    return num / 100 * percent;
                case MathSymbol.Multiplication:
                case MathSymbol.Division:
                case MathSymbol.Power:
                    return percent / 100;
                default:
                    throw new InvalidExpressionException($"The provided symbol {symbol} was not valid.");
            }
        }
    }
}
