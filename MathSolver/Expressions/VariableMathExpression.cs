using System.Collections.Generic;
using System.Linq;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Helpers;
using MathSolver.Models;

namespace MathSolver.Expressions
{
    public class VariableMathExpression : MathExpression
    {
        public VariableMathExpression(char variable, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
            : base(MathExpressionType.Variable, suffixSymbols)
        {
            Variable = variable;
        }

        public char Variable { get; }

        public override double Solve(params MathVariable[] variables)
        {
            foreach (MathVariable variable in variables)
            {
                if (variable.Variable == Variable)
                {
                    double result = variable.Number;

                    foreach (MathSuffixSymbol suffixSymbol in SuffixSymbols)
                    {
                        result = suffixSymbol switch
                        {
                            MathSuffixSymbol.Factorial => MathHelper.Factorial(result),
                            MathSuffixSymbol.Percent => result / 100,
                            _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(Solve)} method does not implement {nameof(MathSuffixSymbol)}.")
                        };
                    }

                    return result;
                }
            }

            throw new InvalidMathExpressionException($"The provided variable {Variable} was not found in the list of variables {string.Join(", ", variables.Select(f => f.Variable))}.");
        }

        public override string ToString()
        {
            return this.Suffix(Variable.ToString(), BracketType.None);
        }
    }
}
