namespace MathSolver
{
    public struct SolvedNumber
    {
        public SolvedNumber(double number, bool isPercent)
        {
            Number = number;
            IsPercent = isPercent;
        }

        public double Number { get; }
        public bool IsPercent { get; }
    }
}
