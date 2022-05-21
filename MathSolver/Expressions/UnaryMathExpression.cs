using System.Linq.Expressions;
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
                    rightNumber = CalculatePercentage(leftNumber, rightNumber, Symbol);
                }
                else if (LeftOperand.IsPercent)
                {
                    leftNumber = CalculatePercentage(rightNumber, leftNumber, Symbol);
                }
                else
                {
                    rightNumber = CalculatePercentage(leftNumber, rightNumber, Symbol);
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

            if (!string.IsNullOrEmpty(Coefficient))
            {
                result = MathHelper.CalculateCoefficient(Coefficient, result);
            }

            result = MathHelper.CalculateNumberSuffix(result, this);

            return result;
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix($"({LeftOperand} {ToStringHelper.SymbolEnumToChar(Symbol)} {RightOperand})", this, false);
        }

        private static double CalculatePercentage(double num, double percent, MathSymbol symbol)
        {
            return symbol switch
            {
                MathSymbol.Addition or MathSymbol.Subraction => num * percent,
                _ => percent,
            };
        }
    }
}
