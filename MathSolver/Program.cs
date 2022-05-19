using MathSolver;

MathExpression math = MathParser.Parse("5^x% + sqrt10(y - 1) * y(10 + 10)");

Console.WriteLine(math.Solve(new MathVariable[]
{
    new MathVariable('x', 5d),
    new MathVariable('y', 1000d)
}));