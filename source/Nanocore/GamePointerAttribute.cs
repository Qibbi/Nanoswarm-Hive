using System;

namespace Nanocore
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GamePointerAttribute : Attribute
    {
        public int Origin { get; set; }
        public int Steam { get; set; }
    }
}
