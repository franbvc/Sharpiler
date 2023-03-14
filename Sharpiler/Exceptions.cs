namespace Sharpiler
{
    public class LexicalException : Exception
    {
        public LexicalException(string message)
        {
        }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException(string message)
        {
        }
    }
    
    public class SemanticException : Exception
    {
        public SemanticException(string message)
        {
        }
    }
}
