﻿namespace Sharpiler
{
    public class LexicalException : Exception
    {
        public LexicalException(string message): base(message)
        {
        }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException(string message): base(message)
        {
        }
    }
    
    public class SemanticException : Exception
    {
        public SemanticException(string message): base(message)
        {
        }
    }
    
    public class ReturnException : Exception
    {
        public dynamic Value { get; }
    
        public ReturnException(dynamic value)
        {
            Value = value;
        }
    }
}
