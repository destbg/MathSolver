namespace MathSolver.Models;

public class MathVariable
{
    public MathVariable(char variable, double number)
    {
        Variable = variable;
        Number = number;
    }

    public char Variable { get; set; }
    public double Number { get; set; }
}
