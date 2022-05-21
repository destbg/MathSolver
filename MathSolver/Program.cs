using System.Linq.Expressions;
using MathSolver;
using MathSolver.Expressions;
using MathSolver.Models;

MathExpression math = MathParser.Parse("10% + x^(5 + x)");

MathExpression simplified = MathParser.Simplify(math);

(Expression expression, List<ParameterExpression> parameters) = MathParser.ConvertToCSharpExpression(math);

Func<double, double> labda = Expression.Lambda<Func<double, double>>(expression, parameters).Compile();

Console.WriteLine(simplified.Solve(new MathVariable('x', 5d), new MathVariable('y', 1000d)));
Console.WriteLine(labda(5d));
