using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class UnaryMathExpression : MathExpression
    {
        public UnaryMathExpression(MathExpression leftOperand, MathExpression rightOperand, MathSymbol symbol)
            : base(MathExpressionType.Unary)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Symbol = symbol;
        }

        public MathExpression LeftOperand { get; }
        public MathExpression RightOperand { get; }
        public MathSymbol Symbol { get; }

        public string? Coefficient { get; set; }

        public override double Solve(params MathVariable[] variables)
        {
            double leftNumber = LeftOperand.Solve(variables);
            double rightNumber = RightOperand.Solve(variables);

            if (LeftOperand.IsPercent || RightOperand.IsPercent)
            {
                if (LeftOperand.IsPercent && RightOperand.IsPercent)
                {
                    rightNumber = FindPercent(leftNumber, rightNumber, Symbol);
                }
                else if (LeftOperand.IsPercent)
                {
                    leftNumber = FindPercent(rightNumber, leftNumber, Symbol);
                }
                else
                {
                    rightNumber = FindPercent(leftNumber, rightNumber, Symbol);
                }
            }

            double result = Symbol switch
            {
                MathSymbol.Addition => leftNumber + rightNumber,
                MathSymbol.Subraction => leftNumber - rightNumber,
                MathSymbol.Multiplication => leftNumber * rightNumber,
                MathSymbol.Division => leftNumber / rightNumber,
                MathSymbol.Power => Math.Pow(leftNumber, rightNumber),
                _ => throw new InvalidExpressionException($"The provided symbol {Symbol} was not valid."),
            };

            result = MathHelper.CalculateNumberSuffix(result, IsPercent, IsFactorial);

            if (!string.IsNullOrEmpty(Coefficient))
            {
                result = CalculateCoefficient(Coefficient, result);
            }

            return result;
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix($"({LeftOperand} {ToStringHelper.SymbolEnumToChar(Symbol)} {RightOperand})", this, false);
        }

        private static double FindPercent(double num, double percent, MathSymbol symbol)
        {
            return symbol switch
            {
                MathSymbol.Addition or MathSymbol.Subraction => num * percent,
                _ => percent,
            };
        }

        private static double CalculateCoefficient(string coefficient, double num)
        {
            if (coefficient.StartsWith("log"))
            {
                if (coefficient == "log2")
                {
                    return Math.Log2(num);
                }
                else if (coefficient == "log10")
                {
                    return Math.Log10(num);
                }

                return Math.Log(num, int.Parse(coefficient.Replace("log", string.Empty)));
            }

            if (coefficient.StartsWith("sqrt"))
            {
                if (coefficient == "sqrt")
                {
                    return Math.Sqrt(num);
                }

                return Math.Pow(num, 1 / int.Parse(coefficient.Replace("sqrt", string.Empty)));
            }

            return coefficient switch
            {
                "abs" => Math.Abs(num),
                "acos" => Math.Acos(num),
                "acosh" => Math.Acosh(num),
                "asin" => Math.Asin(num),
                "asinh" => Math.Asinh(num),
                "atan" => Math.Atan(num),
                "atanh" => Math.Atanh(num),
                "cbrt" => Math.Cbrt(num),
                "ceil" => Math.Ceiling(num),
                "cos" => Math.Cos(num),
                "cosh" => Math.Cosh(num),
                "floor" => Math.Floor(num),
                "round" => Math.Round(num),
                "sign" => Math.Sign(num),
                "sin" => Math.Sin(num),
                "sinh" => Math.Sinh(num),
                "tan" => Math.Tan(num),
                "tanh" => Math.Tanh(num),
                "trunc" => Math.Truncate(num),
                _ => throw new InvalidExpressionException($"The provided coefficient {coefficient} was not valid."),
            };
        }
    }
}
