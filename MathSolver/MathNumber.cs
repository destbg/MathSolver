namespace MathSolver
{
    public struct MathNumber
    {
        public MathNumber(double number, bool isPercent)
        {
            Number = number;
            Variable = '\0';
            IsNumber = true;
            IsPercent = isPercent;
        }

        public MathNumber(char variable, bool isPercent)
        {
            Number = 0;
            Variable = variable;
            IsNumber = false;
            IsPercent = isPercent;
        }

        public double Number { get; }
        public char Variable { get; }
        public bool IsNumber { get; }
        public bool IsPercent { get; }

        public SolvedNumber Get(MathVariable[] variables)
        {
            if (IsNumber)
            {
                return new SolvedNumber(Number, IsPercent);
            }

            foreach (MathVariable variable in variables)
            {
                if (variable.Variable == Variable)
                {
                    return new SolvedNumber(variable.Number, IsPercent);
                }
            }

            throw new InvalidExpressionException($"The provided variable {Variable} was not found in the list of variables {variables}.");
        }
    }
}
