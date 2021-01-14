using System;
using System.Linq;
using System.Reflection;

namespace Nanocore.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetName(this Enum value)
        {
            MemberInfo memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (memberInfo is null)
            {
                return null;
            }
            DisplayAttribute attribute = (DisplayAttribute)memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (attribute is null)
            {
                return value.ToString();
            }
            return attribute.Name;
        }
    }
}
