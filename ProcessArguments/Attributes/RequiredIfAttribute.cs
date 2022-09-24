using System;

namespace ProcessArguments
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        public RequiredIfAttribute(string conditionMethodName)
        {
            ConditionMethodName = conditionMethodName;
        }

        public string ConditionMethodName { get; set; }
    }
}
