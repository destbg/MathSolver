using System.Globalization;
using System.Linq.Expressions;
using MathSolver;
using MathSolver.Expressions;
using MathSolver.Models;

Console.WriteLine("Declare variables that you can later use to play with.");
Console.Write("Variables (x=10) = ");
string variables = Console.ReadLine()!;

MathVariable[] mathVariables = variables.Split(' ')
    .Select(f =>
    {
        string[] split = f.Split('=');

        return new MathVariable(split[0][0], double.Parse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture));
    }).ToArray();

Console.WriteLine();

while (true)
{
    Console.Write("func = ");
    string input = Console.ReadLine()!;
    MathExpression math = MathParser.Parse(input);

    MathExpression simplified = MathParser.Simplify(math);

    (Expression expression, List<ParameterExpression> parameters) = MathParser.ConvertToCSharpExpression(math);
    (Expression simplifiedExpression, List<ParameterExpression> simplifiedParameters) = MathParser.ConvertToCSharpExpression(simplified);

    // Func<double, double> func = Expression.Lambda<Func<double, double>>(expression, parameters).Compile();
    Delegate lambda = Expression.Lambda(expression, parameters).Compile();

    Delegate simplifiedLambda = Expression.Lambda(simplifiedExpression, simplifiedParameters).Compile();

    object[] invokeParameters = parameters
        .Select(f => mathVariables.First(s => s.Variable == f.Name![0]).Number as object)
        .ToArray();

    object[] simplifiedInvokeParameters = simplifiedParameters
        .Select(f => mathVariables.First(s => s.Variable == f.Name![0]).Number as object)
        .ToArray();

    string mathString = math.ToString();
    string simplifiedString = simplified.ToString();
    string expressionString = expression.ToString();
    string simplifiedExpressionString = simplifiedExpression.ToString();

    int longest = new[] { mathString.Length, simplifiedString.Length, expressionString.Length, simplifiedExpressionString.Length }.Max();

    Console.WriteLine();
    Console.WriteLine($"  {nameof(MathExpression)}: {mathString}{new string(' ', longest - mathString.Length)} = {math.Solve(mathVariables)}");
    Console.WriteLine($"S {nameof(MathExpression)}: {simplifiedString}{new string(' ', longest - simplifiedString.Length)} = {simplified.Solve(mathVariables)}");
    Console.WriteLine($"      {nameof(Expression)}: {expressionString}{new string(' ', longest - expressionString.Length)} = {lambda.DynamicInvoke(invokeParameters)}");
    Console.WriteLine($"S     {nameof(Expression)}: {simplifiedExpressionString}{new string(' ', longest - simplifiedExpressionString.Length)} = {lambda.DynamicInvoke(simplifiedInvokeParameters)}");
    Console.WriteLine();
}
