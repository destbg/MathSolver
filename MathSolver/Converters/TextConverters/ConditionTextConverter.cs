using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal class ConditionTextConverter : BaseTextConverter
    {
        public override bool IsValid(TextConverterModel model)
        {
            return model.Current == '=';
        }

        public override void Convert(TextConverterModel model)
        {
            if (model.Count < 1)
            {
                throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
            }

            EquationPart lastExpression = model.Parts[^1];

            if (lastExpression.Type == EquationType.Symbol)
            {
                throw new InvalidMathExpressionException($"The provided equation {model.Equation} was not valid.");
            }

            char c = model.Current;

            if (model.Equation[model.Index + 1] == '=')
            {
                model.Index++;
            }

            // TODO: Currently doesn't support condition inside a condition
            // TODO: Maybe if false should support brackets

            int startIndex = model.Index + 1;

            while (c != '?')
            {
                model.Index++;
                c = model.Current;
            }

            string rightCheck = model.Equation[startIndex..(model.Index - 1)];

            startIndex = model.Index + 1;

            while (c != ':')
            {
                model.Index++;
                c = model.Current;
            }

            string ifTrue = model.Equation[startIndex..(model.Index - 1)];

            startIndex = model.Index + 1;

            while (c == ' ')
            {
                model.Index++;
            }

            string ifFalse = model.Equation[startIndex..(model.Length - 1)];

            model.Parts[^1] = new ConditionEquationPart(lastExpression, rightCheck, ifTrue, ifFalse);

            model.Index = model.Length - 1;
        }
    }
}
