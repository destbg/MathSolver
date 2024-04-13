using System;

namespace MathSolver.Exceptions;

public class InvalidMathExpressionException : Exception
{
    public InvalidMathExpressionException(string message)
        : base(message) { }
}
