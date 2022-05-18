namespace MathSolver
{
    public struct MathNumber
    {
        public MathNumber(double number)
        {
            Number = number;
            Variable = '\0';
            IsNumber = true;
        }

        public MathNumber(char variable)
        {
            Number = 0;
            Variable = variable;
            IsNumber = false;
        }

        public double Number { get; }
        public char Variable { get; }
        public bool IsNumber { get; }

        public double this[MathVariable[] variables]
        {
            get
            {
                if (IsNumber)
                {
                    return Number;
                }

                foreach (MathVariable variable in variables)
                {
                    if (variable.Variable == Variable)
                    {
                        return variable.Number;
                    }
                }

                throw new ArgumentException($"The provided variable {Variable} was not found in the list of variables {variables}.");
            }
        }
    }
}
