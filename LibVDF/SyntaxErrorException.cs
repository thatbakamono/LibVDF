using System;

namespace LibVDF
{
    public class SyntaxErrorException : Exception
    {
        public int Line { get; }

        public SyntaxErrorException(string message, int line) : base(message) 
        {
            Line = line;
        }
    }
}