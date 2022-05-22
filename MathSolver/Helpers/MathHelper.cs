using System.Numerics;

namespace MathSolver.Helpers
{
    internal static class MathHelper
    {
        public static double CalculateCoefficient(string coefficient, double num)
        {
            if (coefficient.StartsWith("log"))
            {
                if (coefficient == "log2")
                {
                    return Math.Log2(num);
                }
                else if (coefficient == "log10")
                {
                    return Math.Log10(num);
                }

                return Math.Log(num, int.Parse(coefficient.Replace("log", string.Empty)));
            }

            if (coefficient.StartsWith("sqrt"))
            {
                if (coefficient == "sqrt")
                {
                    return Math.Sqrt(num);
                }

                double nthRoot = 1d / int.Parse(coefficient.Replace("sqrt", string.Empty));

                return Math.Pow(num, nthRoot);
            }

            return coefficient switch
            {
                "abs" => Math.Abs(num),
                "acos" => Math.Acos(num),
                "acosh" => Math.Acosh(num),
                "asin" => Math.Asin(num),
                "asinh" => Math.Asinh(num),
                "atan" => Math.Atan(num),
                "atanh" => Math.Atanh(num),
                "cbrt" => Math.Cbrt(num),
                "ceil" => Math.Ceiling(num),
                "cos" => Math.Cos(num),
                "cosh" => Math.Cosh(num),
                "floor" => Math.Floor(num),
                "round" => Math.Round(num),
                "sign" => Math.Sign(num),
                "sin" => Math.Sin(num),
                "sinh" => Math.Sinh(num),
                "tan" => Math.Tan(num),
                "tanh" => Math.Tanh(num),
                "trunc" => Math.Truncate(num),
                _ => throw new InvalidExpressionException($"The provided coefficient {coefficient} was not valid."),
            };
        }

        public static double Factorial(double num)
        {
            return InternalFactorial((long)Math.Round(num));
        }

        private static double InternalFactorial(long num)
        {
            BigInteger sum = num;
            BigInteger result = num;

            for (long i = num - 2; i > 1; i -= 2)
            {
                sum += i;
                result *= sum;
            }

            if (num % 2 != 0)
            {
                result *= num / 2 + 1;
            }

            return (double)result;
        }
    }
}
