using System.Collections.Generic;
using MathSolver.Enums;
using MathSolver.Exceptions;
using MathSolver.Expressions;
using MathSolver.Models;

namespace MathSolver.Converters
{
    internal class EquationToMathExpressionConveter
    {
        private readonly string equation;
        private readonly string? coefficient;
        private readonly BracketType bracketType;
        private readonly IReadOnlyList<MathSuffixSymbol> suffixSymbols;
        private readonly List<EquationPart> expressions;

        public EquationToMathExpressionConveter(string equation, List<EquationPart> expressions, string? coefficient, BracketType bracketType, IReadOnlyList<MathSuffixSymbol> suffixSymbols)
        {
            this.equation = equation;
            this.expressions = expressions;
            this.coefficient = coefficient;
            this.bracketType = bracketType;
            this.suffixSymbols = suffixSymbols;
        }

        public MathExpression Convert()
        {
            InsertForSymbol(0, 0);
            InsertForSymbol(expressions.Count - 1, expressions.Count);
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
                throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
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
                    throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
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
                    throw new InvalidMathExpressionException($"The equation {equation} cannot start or end with a multiplication, division or power symbol.");
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
            if (expression.Type == EquationType.Expression)
            {
                SubEquationPart subEquation = (SubEquationPart)expression;

                MathExpression mathExpression = MathParser.Parse(subEquation.Expression, subEquation.Coefficient, subEquation.Bracket, subEquation.SuffixSymbols);

                return mathExpression;
            }
            else if (expression.Type == EquationType.Variable)
            {
                VariableEquationPart variableEquation = (VariableEquationPart)expression;

                return new VariableMathExpression(variableEquation.Variable, variableEquation.SuffixSymbols);
            }
            else if (expression.Type == EquationType.Number)
            {
                ConstantEquationPart constantEquation = (ConstantEquationPart)expression;

                return new ConstantMathExpression(constantEquation.Number, constantEquation.SuffixSymbols);
            }
            else if (expression.Type == EquationType.Condition)
            {
                ConditionEquationPart conditionEquation = (ConditionEquationPart)expression;

                MathExpression leftMathExpression = ConvertExpression(conditionEquation.LeftCheck);
                MathExpression rightMathExpression = MathParser.Parse(conditionEquation.RightCheck);

                MathExpression ifTrueMathExpression = MathParser.Parse(conditionEquation.IfTrue);
                MathExpression ifFalseMathExpression = MathParser.Parse(conditionEquation.IfFalse);

                return new ConditionMathExpression(leftMathExpression, rightMathExpression, ifTrueMathExpression, ifFalseMathExpression);
            }
            else if (expression.Type == EquationType.MathExpression)
            {
                return ((ExpressionEquationPart)expression).MathExpression;
            }

            throw new InvalidMathExpressionException($"The provided equation {equation} was not valid.");
        }
    }
}
