namespace MathSolver.Helpers
{
    internal static class MathParseHelper
    {
        public static bool IsValidCoefficient(string coefficient)
        {
            if (coefficient.StartsWith("log"))
            {
                return int.TryParse(coefficient.Replace("log", string.Empty), out _);
            }

            if (coefficient.StartsWith("sqrt"))
            {
                return coefficient == "sqrt"
                    || int.TryParse(coefficient.Replace("sqrt", string.Empty), out _);
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
