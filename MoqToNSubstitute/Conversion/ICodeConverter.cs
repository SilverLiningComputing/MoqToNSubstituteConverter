namespace MoqToNSubstitute.Conversion
{
    internal interface ICodeConverter
    {
        void Convert(string path = "", bool transform = false);
    }
}
