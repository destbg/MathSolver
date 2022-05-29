using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Models;

namespace MathSolver.Converters
{
    internal class EquationToMathExpressionConveter
    {
        private readonly string? coefficient;
        private readonly BracketType bracketType;
        private readonly IReadOnlyList<MathSuffixSymbol> suffixSymbols;
        private readonly List<EquationPart> expressions;

        public EquationToMathExpressionConveter(List<EquationPart> expressions, string? coefficient, BracketType bracketType, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            this.expressions = expressions;
            this.coefficient = coefficient;
            this.bracketType = bracketType;
            this.suffixSymbols = suffixSymbols;
        }

        public MathExpression Convert()
        {
            InsertForSymbol(0, 0);
            InsertForSymbol(expressions.Count - 1, expressions.Count);

            (ConditionType equalConditionType, int equalConditionIndex) = IndexOfEqualCondition();

            if (equalConditionIndex != -1)
            {
                int ifTrueConditionIndex = IndexOfTrueCondition(equalConditionIndex + 1);

                if (equalConditionIndex == -1)
                {
                    throw new InvalidMathExpressionException("The provided equation was not valid.");
                }

                int ifFalseConditionIndex = IndexOfFalseCondition(ifTrueConditionIndex + 1);

                if (ifFalseConditionIndex == -1)
                {
                    throw new InvalidMathExpressionException("The provided equation was not valid.");
                }

                List<EquationPart> leftCheckExpressions = expressions.GetRange(0, equalConditionIndex);
                List<EquationPart> rightCheckExpressions = expressions.GetRange(equalConditionIndex + 1, ifTrueConditionIndex - equalConditionIndex - 1);
                List<EquationPart> ifTrueExpressions = expressions.GetRange(ifTrueConditionIndex + 1, ifFalseConditionIndex - ifTrueConditionIndex - 1);
                List<EquationPart> ifFalseExpressions = expressions.GetRange(ifFalseConditionIndex + 1, expressions.Count - ifFalseConditionIndex - 1);

                MathExpression leftCheck = MathParser.Parse(leftCheckExpressions);
                MathExpression rightCheck = MathParser.Parse(rightCheckExpressions);
                MathExpression ifTrue = MathParser.Parse(ifTrueExpressions);
                MathExpression ifFalse = MathParser.Parse(ifFalseExpressions);

                ConditionMathExpression expression = new ConditionMathExpression(equalConditionType == ConditionType.Equal, leftCheck, rightCheck, ifTrue, ifFalse);

                return new SingleMathExpression(expression, bracketType, suffixSymbols, coefficient);
            }

            CheckIfValidEquation();

            int expressionIndex = IndexOfImportantSymbol();

            while (expressionIndex != -1)
            {
                SimplifyExpression(expressionIndex);
                expressionIndex = IndexOfImportantSymbol();
            }

            while (expressions.Count > 1)
            {
                SimplifyExpression(1);
            }

            // This can happen if the expression is only a number
            if (expressions[0].Type != EquationType.MathExpression)
            {
                MathExpression expression = ConvertExpression(expressions[0]);

                return new SingleMathExpression(expression, bracketType, suffixSymbols, coefficient);
            }
            else
            {
                ExpressionEquationPart expression = (ExpressionEquationPart)expressions[0];
                UnaryMathExpression unaryExpression = (UnaryMathExpression)expression.MathExpression;

                return new UnaryMathExpression(unaryExpression.LeftOperand, unaryExpression.RightOperand, unaryExpression.Symbol, bracketType, suffixSymbols, coefficient);
            }
        }

        private void CheckIfValidEquation()
        {
            if (expressions.Count % 2 == 0)
            {
                throw new InvalidMathExpressionException("The provided equation was not valid.");
            }

            bool atSymbol = false;

            for (int i = 0; i < expressions.Count; i++)
            {
                EquationType EquationType = expressions[i].Type;

                if (atSymbol && EquationType == EquationType.Symbol)
                {
                    atSymbol = false;
                }
                else if (!atSymbol && EquationType != EquationType.Symbol)
                {
                    atSymbol = true;
                }
                else
                {
                    throw new InvalidMathExpressionException("The provided equation was not valid.");
                }
            }
        }

        private void InsertForSymbol(int indexToCheck, int indexToInsert)
        {
            EquationPart expression = expressions[indexToCheck];

            if (expression.Type == EquationType.Symbol)
            {
                MathSymbol symbol = ((SymbolEquationPart)expression).Symbol;

                if (symbol == MathSymbol.Addition || symbol == MathSymbol.Subraction)
                {
                    expressions.Insert(indexToInsert, new ConstantEquationPart(0d));
                }
                else
                {
                    throw new InvalidMathExpressionException("The equation cannot start or end with a multiplication, division or power symbol.");
                }
            }
        }

        private int IndexOfImportantSymbol()
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                EquationPart expression = expressions[i];

                if (expression.Type == EquationType.Symbol)
                {
                    MathSymbol symbol = ((SymbolEquationPart)expression).Symbol;

                    if (symbol != MathSymbol.Addition && symbol != MathSymbol.Subraction)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private (ConditionType, int) IndexOfEqualCondition()
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                EquationPart expression = expressions[i];

                if (expression.Type == EquationType.Condition)
                {
                    ConditionType condition = ((ConditionEquationPart)expression).Condition;

                    if (condition == ConditionType.Equal || condition == ConditionType.NotEqual)
                    {
                        return (condition, i);
                    }
                }
            }

            return (ConditionType.Equal, -1);
        }

        private int IndexOfTrueCondition(int i)
        {
            int innerConditionsCount = 0;

            for (; i < expressions.Count; i++)
            {
                EquationPart expression = expressions[i];

                if (expression.Type == EquationType.Condition)
                {
                    ConditionType condition = ((ConditionEquationPart)expression).Condition;

                    switch (condition)
                    {
                        case ConditionType.Equal:
                            innerConditionsCount++;
                            break;
                        case ConditionType.True:
                            if (innerConditionsCount == 0)
                            {
                                return i;
                            }

                            break;
                        case ConditionType.False:
                            innerConditionsCount--;
                            break;
                    }
                }
            }

            return -1;
        }

        private int IndexOfFalseCondition(int i)
        {
            int innerConditionsCount = 0;

            for (; i < expressions.Count; i++)
            {
                EquationPart expression = expressions[i];

                if (expression.Type == EquationType.Condition)
                {
                    ConditionType condition = ((ConditionEquationPart)expression).Condition;

                    switch (condition)
                    {
                        case ConditionType.Equal:
                            innerConditionsCount++;
                            break;
                        case ConditionType.False:
                            if (innerConditionsCount == 0)
                            {
                                return i;
                            }

                            innerConditionsCount--;
                            break;
                    }
                }
            }

            return -1;
        }

        private void SimplifyExpression(int expressionIndex)
        {
            EquationPart simpleLeft = expressions[expressionIndex - 1];
            EquationPart simpleSymbol = expressions[expressionIndex];
            EquationPart simpleRight = expressions[expressionIndex + 1];

            MathExpression leftExpression = ConvertExpression(simpleLeft);
            MathExpression rightExpression = ConvertExpression(simpleRight);

            MathSymbol symbol = ((SymbolEquationPart)simpleSymbol).Symbol;

            MathExpression mathExpression = new UnaryMathExpression(leftExpression, rightExpression, symbol, BracketType.None, new List<MathSuffixSymbol>());

            ExpressionEquationPart newExpression = new ExpressionEquationPart(mathExpression);

            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.RemoveAt(expressionIndex - 1);
            expressions.Insert(expressionIndex - 1, newExpression);
        }

        private MathExpression ConvertExpression(EquationPart expression)
        {
            switch (expression.Type)
            {
                case EquationType.Number:
                {
                    ConstantEquationPart constantEquation = (ConstantEquationPart)expression;

                    return new ConstantMathExpression(constantEquation.Number, constantEquation.SuffixSymbols);
                }
                case EquationType.Variable:
                {
                    VariableEquationPart variableEquation = (VariableEquationPart)expression;

                    return new VariableMathExpression(variableEquation.Variable, variableEquation.SuffixSymbols);
                }
                case EquationType.Expression:
                {
                    SubEquationPart subEquation = (SubEquationPart)expression;

                    MathExpression mathExpression = MathParser.Parse(subEquation.Expression, subEquation.Coefficient, subEquation.Bracket, subEquation.SuffixSymbols);

                    return mathExpression;
                }
                case EquationType.MathExpression:
                {
                    return ((ExpressionEquationPart)expression).MathExpression;
                }
                // Should never enter as these
                // case EquationType.Symbol:
                // case EquationType.Condition:
                default:
                    throw new InvalidMathExpressionException("The provided equation was not valid.");
            }
        }
    }
}
