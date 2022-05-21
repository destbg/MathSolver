using System.Numerics;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class UnaryMathExpression : MathExpression
    {
        private readonly MathExpression leftOperand;
        private readonly MathExpression rightOperand;
        private readonly MathSymbol symbol;

        public UnaryMathExpression(MathExpression leftOperand, MathExpression rightOperand, MathSymbol symbol)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
            this.symbol = symbol;
        }

        public override double Solve(MathVariable[] variables)
        {
            double leftNumber = leftOperand.Solve(variables);
            double rightNumber = rightOperand.Solve(variables);

            if (leftOperand.IsPercent || rightOperand.IsPercent)
            {
                if (leftOperand.IsPercent && rightOperand.IsPercent)
                {
                    leftNumber /= 100;
                    rightNumber = CalculatePercentage(leftNumber, rightNumber, symbol);
                }
                else if (leftOperand.IsPercent)
                {
                    leftNumber = CalculatePercentage(rightNumber, leftNumber, symbol);
                }
                else
                {
                    rightNumber = CalculatePercentage(leftNumber, rightNumber, symbol);
                }
            }

            return symbol switch
            {
                MathSymbol.Addition => leftNumber + rightNumber,
                MathSymbol.Subraction => leftNumber - rightNumber,
                MathSymbol.Multiplication => leftNumber * rightNumber,
                MathSymbol.Division => leftNumber / rightNumber,
                MathSymbol.Power => Math.Pow(leftNumber, rightNumber),
                MathSymbol.Factorial => Factorial((long)Math.Round(leftNumber)),
                _ => throw new InvalidExpressionException($"The provided symbol {symbol} was not valid."),
            };
        }

        public override string ToString()
        {
            return $"{Coefficient ?? string.Empty}({leftOperand} {MathHelper.SymbolEnumToChar(symbol)} {rightOperand})";
        }

        private static double CalculatePercentage(double num, double percent, MathSymbol symbol)
        {
            return symbol switch
            {
                MathSymbol.Addition or MathSymbol.Subraction => num / 100 * percent,
                MathSymbol.Multiplication or MathSymbol.Division or MathSymbol.Power => percent / 100,
                _ => throw new InvalidExpressionException($"The provided symbol {symbol} was not valid."),
            };
        }

        private static double Factorial(long num)
        {
            BigInteger sum = num;
            BigInteger result = num;

            for (long i = num - 2; i > 1; i -= 2)
            {
                sum += i;
                result *= sum;
            }

            if (num % 2 != 0)
            {
                result *= num / 2 + 1;
            }

            return (double)result;
        }
    }
}
