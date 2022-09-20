using System;

namespace ProcessArguments
{
    public sealed class IncorrectProcessArgumentException : InvalidProcessArgumentException
    {
        public IncorrectProcessArgumentException(string message) : base(message)
        {
        }
    }
}
