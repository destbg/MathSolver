using MathSolver;

MathParser exp = new("5 * 5 + 5%");

MathExpression math = exp.Parse();

Console.WriteLine(math.Solve(new MathVariable[0]));