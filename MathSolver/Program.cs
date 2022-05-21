using MathSolver;
using MathSolver.Expressions;
using MathSolver.Models;

MathExpression math = MathParser.Parse("15 / 5!");

Console.WriteLine(math.Solve(new MathVariable[]
{
    new MathVariable('x', 5d),
    new MathVariable('y', 1000d)
}));