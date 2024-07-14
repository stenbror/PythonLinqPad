
namespace PythonCore
{
    public class SyntaxError(int position, string text) : Exception
    {
        public int Position { get; private set; } = position;
        public string Text { get; private set; } = text;
    }
}
