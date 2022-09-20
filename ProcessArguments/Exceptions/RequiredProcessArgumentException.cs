using System;

namespace ProcessArguments.Exceptions
{
    public sealed class RequiredProcessArgumentException : InvalidProcessArgumentException
    {
        public RequiredProcessArgumentException(string message) : base(message)
        {
        }
    }
}
