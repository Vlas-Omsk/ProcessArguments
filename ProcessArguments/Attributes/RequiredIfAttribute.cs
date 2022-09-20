using System;
using System.ComponentModel.DataAnnotations;

namespace ProcessArguments
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        public RequiredIfAttribute(string conditionMethodName)
        {
            ConditionMethodName = conditionMethodName;
        }

        public string ConditionMethodName { get; set; }
    }
}
