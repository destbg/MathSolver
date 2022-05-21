using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class VariableMathExpression : MathExpression
    {
        private readonly char variable;

        public VariableMathExpression(char variable, bool isPercent)
        {
            this.variable = variable;
            IsPercent = isPercent;
        }

        public override double Solve(MathVariable[] variables)
        {
            foreach (MathVariable variable in variables)
            {
                if (variable.Variable == this.variable)
                {
                    return !string.IsNullOrEmpty(Coefficient) ?
                        MathHelper.CalculateCoefficient(Coefficient, variable.Number)
                        : variable.Number;
                }
            }

            throw new InvalidExpressionException($"The provided variable {variable} was not found in the list of variables {variables}.");
        }

        public override string ToString()
        {
            return variable.ToString();
        }
    }
}
