using System.Collections.Generic;
using System.Linq.Expressions;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Helpers;

namespace MathSolver.Converters.ExpressionConverters
{
    internal class VariableExpressionConverter
    {
        private readonly List<ParameterExpression> parameters;

        public VariableExpressionConverter(List<ParameterExpression> parameters)
        {
            this.parameters = parameters;
        }

        public Expression Convert(MathExpression mathExpression)
        {
            VariableMathExpression variableMath = (VariableMathExpression)mathExpression;
            string variableAsString = variableMath.Variable.ToString();
            ParameterExpression parameter = parameters.Find(f => f.Name == variableAsString)!;

            Expression exp = parameter;

            foreach (MathSuffixSymbol suffixSymbol in variableMath.SuffixSymbols)
            {
                exp = suffixSymbol switch
                {
                    MathSuffixSymbol.Factorial => Expression.Call(typeof(MathHelper), nameof(MathHelper.Factorial), null, exp),
                    MathSuffixSymbol.Percent => Expression.Divide(exp, Expression.Constant(100d)),
                    _ => throw new InvalidMathExpressionException($"Internal exception: {nameof(VariableExpressionConverter)} class does not implement {nameof(MathSuffixSymbol)}.")
                };
            }

            return exp;
        }
    }
}
