using System;

namespace ProcessArguments
{
    public class InvalidProcessArgumentException : Exception
    {
        public InvalidProcessArgumentException(string message) : base(message)
        {
        }
    }
}
