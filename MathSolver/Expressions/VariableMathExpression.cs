using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class VariableMathExpression : MathExpression
    {
        public VariableMathExpression(char variable, bool isPercent, bool isFactorial)
            : base(MathExpressionType.Variable)
        {
            Variable = variable;
            IsPercent = isPercent;
            IsFactorial = isFactorial;
        }

        public char Variable { get; }

        public override double Solve(params MathVariable[] variables)
        {
            foreach (MathVariable variable in variables)
            {
                if (variable.Variable == Variable)
                {
                    return MathHelper.CalculateNumberSuffix(variable.Number, this);
                }
            }

            throw new InvalidExpressionException($"The provided variable {Variable} was not found in the list of variables {variables}.");
        }

        public override string ToString()
        {
            return ToStringHelper.ExpressionSuffix(Variable.ToString(), this);
        }
    }
}
