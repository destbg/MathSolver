using MathSolver;

MathExpression math = MathParser.Parse("5^x%");

Console.WriteLine(math.Solve(new MathVariable[] { new MathVariable('x', 5d) }));