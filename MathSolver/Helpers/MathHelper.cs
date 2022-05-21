using System.Numerics;

namespace MathSolver.Helpers
{
    internal static class MathHelper
    {
        public static double CalculateNumberSuffix(double number, bool isPercent, bool isFactorial)
        {
            if (isFactorial)
            {
                number = Factorial((long)Math.Round(number));
            }

            if (isPercent)
            {
                number /= 100;
            }

            return number;
        }

        private static double Factorial(long num)
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
