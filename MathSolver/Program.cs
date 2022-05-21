using MathSolver;
using MathSolver.Expressions;
using MathSolver.Models;

MathExpression math = MathParser.Parse("10% + (x + 5!)");

var simplified = MathParser.Simplify(math);

Console.WriteLine(simplified.Solve(new MathVariable('x', 5d), new MathVariable('y', 1000d)));