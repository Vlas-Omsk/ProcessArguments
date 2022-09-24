using System;

namespace ProcessArguments
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfAttribute : Attribute
    {
        public RequiredIfAttribute(string conditionMethodName)
        {
            ConditionMethodName = conditionMethodName;
        }

        public string ConditionMethodName { get; set; }
        public string Message { get; set; }
    }
}
