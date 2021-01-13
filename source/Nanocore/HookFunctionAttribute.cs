using System;

namespace Nanocore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HookFunctionAttribute : Attribute
    {
        public int Origin { get; set; }
        public int Steam { get; set; }
        public string FunctionName { get; }

        public HookFunctionAttribute(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
