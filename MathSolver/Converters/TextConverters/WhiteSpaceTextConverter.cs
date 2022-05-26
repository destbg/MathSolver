using MathSolver.Models;

namespace MathSolver.Converters.TextConverters
{
    internal class WhiteSpaceTextConverter : BaseTextConverter
    {
        public override bool IsValid(TextConverterModel model)
        {
            return model.Current == ' ';
        }

        public override void Convert(TextConverterModel model)
        {
            model.Index++;
        }
    }
}
