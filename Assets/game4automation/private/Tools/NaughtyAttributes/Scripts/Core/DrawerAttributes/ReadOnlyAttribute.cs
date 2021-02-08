using System;

namespace game4automationtools
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : DrawerAttribute
    {
    }
}
