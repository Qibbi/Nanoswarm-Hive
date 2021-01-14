using System;

namespace Nanocore.Core
{
    public class DisplayAttribute : Attribute
    {
        public string Name { get; }

        public DisplayAttribute(string name)
        {
            Name = name;
        }
    }
}
