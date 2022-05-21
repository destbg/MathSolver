using MathSolver;
using MathSolver.Expressions;
using MathSolver.Models;

MathExpression math = MathParser.Parse("sqrt(10% + x)");

var simplified = MathParser.Simplify(math);

Console.WriteLine(simplified.Solve(new MathVariable('x', 5d), new MathVariable('y', 1000d)));