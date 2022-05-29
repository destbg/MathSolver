using System;
using System.Numerics;
using MathSolver.Exceptions;

namespace MathSolver.Helpers
{
    public static class MathHelper
    {
        public static double Coefficient(string coefficient, double num)
        {
            if (coefficient.StartsWith("log"))
            {
                if (coefficient == "log10")
                {
                    return Math.Log10(num);
                }

                int logBase = int.Parse(coefficient.Replace("log", string.Empty));

                return Math.Log(num, logBase);
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
                _ => throw new InvalidMathExpressionException($"The provided coefficient {coefficient} was not valid."),
            };
        }

        public static double Factorial(double num)
        {
            return num % 1 == 0
                ? IntegerFactorial((long)num)
                : Gamma(num + 1);
        }

        private static double IntegerFactorial(long num)
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

        // From package https://www.nuget.org/packages/MathNet.Numerics/
        private static double Gamma(double num)
        {
            const int GammaN = 10;
            const double GammaR = 10.900511;

            double[] GammaDk =
            {
                2.48574089138753565546e-5,
                1.05142378581721974210,
                -3.45687097222016235469,
                4.51227709466894823700,
                -2.98285225323576655721,
                1.05639711577126713077,
                -1.95428773191645869583e-1,
                1.70970543404441224307e-2,
                -5.71926117404305781283e-4,
                4.63399473359905636708e-6,
                -2.71994908488607703910e-9
            };

            if (num < 0.5)
            {
                double s = GammaDk[0];
                for (int i = 1; i <= GammaN; i++)
                {
                    s += GammaDk[i] / (i - num);
                }

                return Math.PI / (Math.Sin(Math.PI * num)
                    * s
                    * 1.8603827342052657173362492472666631120594218414085755
                    * Math.Pow((0.5 - num + GammaR) / Math.E, 0.5 - num));
            }
            else
            {
                double s = GammaDk[0];
                for (int i = 1; i <= GammaN; i++)
                {
                    s += GammaDk[i] / (num + i - 1.0);
                }

                return s * 1.8603827342052657173362492472666631120594218414085755 * Math.Pow((num - 0.5 + GammaR) / Math.E, num - 0.5);
            }
        }
    }
}
