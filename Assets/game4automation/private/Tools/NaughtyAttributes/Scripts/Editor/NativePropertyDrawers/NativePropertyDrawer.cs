using System.Reflection;

namespace game4automationtools.Editor
{
    public abstract class NativePropertyDrawer
    {
        public abstract void DrawNativeProperty(UnityEngine.Object target, PropertyInfo property);
    }
}
