using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal abstract class BaseTextConverter
    {
        public abstract bool IsValid(TextConverterModel model);

        public abstract void Convert(TextConverterModel model);
    }
}
