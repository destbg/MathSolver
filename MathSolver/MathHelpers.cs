namespace MathSolver
{
    public static class MathHelpers
    {
        public static double CalculateCoefficient(string? coefficient, double num)
        {
            if (coefficient == null)
            {
                return num;
            }

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

                if (int.TryParse(coefficient.Replace("log", string.Empty), out int logBase))
                {
                    return Math.Log(num, logBase);
                }

                throw new InvalidExpressionException($"The provided coefficient {coefficient} was not valid.");
            }

            if (coefficient.StartsWith("sqrt"))
            {
                if (coefficient == "sqrt")
                {
                    return Math.Sqrt(num);
                }

                if (int.TryParse(coefficient.Replace("sqrt", string.Empty), out int nthRoot))
                {
                    return Math.Pow(num, 1 / nthRoot);
                }

                throw new InvalidExpressionException($"The provided coefficient {coefficient} was not valid.");
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
            double result = 1;

            while (num != 1)
            {
                result *= num;
                num--;
            }

            return result;
        }

        public static bool IsMathSymbol(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '!' || c == '.';
        }

        public static bool IsValidCoefficient(string coefficient)
        {
            if (coefficient.StartsWith("log"))
            {
                if (int.TryParse(coefficient.Replace("log", string.Empty), out _))
                {
                    return true;
                }

                return false;
            }

            if (coefficient.StartsWith("sqrt"))
            {
                if (coefficient == "sqrt")
                {
                    return true;
                }

                if (int.TryParse(coefficient.Replace("sqrt", string.Empty), out _))
                {
                    return true;
                }

                return false;
            }

            return coefficient switch
            {
                "abs" => true,
                "acos" => true,
                "acosh" => true,
                "asin" => true,
                "asinh" => true,
                "atan" => true,
                "atanh" => true,
                "cbrt" => true,
                "ceil" => true,
                "cos" => true,
                "cosh" => true,
                "floor" => true,
                "round" => true,
                "sign" => true,
                "sin" => true,
                "sinh" => true,
                "tan" => true,
                "tanh" => true,
                "trunc" => true,
                _ => false,
            };
        }
    }
}
