using System;

namespace ProcessArguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
        public string Message { get; set; }
    }
}
