using System;

namespace game4automationtools
{
    /// <summary>
    /// Make tags appear as tag popup fields 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TagAttribute : DrawerAttribute
    {
    }
}
