namespace MathSolver
{
    public class InvalidExpressionException : Exception
    {
        public InvalidExpressionException(string equation)
            : base($"The provided equation {equation} was not valid.")
        {

        }
    }
}
