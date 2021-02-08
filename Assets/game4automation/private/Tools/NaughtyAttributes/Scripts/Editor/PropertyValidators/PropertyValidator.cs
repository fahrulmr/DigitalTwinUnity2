using UnityEditor;

namespace game4automationtools.Editor
{
    public abstract class PropertyValidator
    {
        public abstract void ValidateProperty(SerializedProperty property);
    }
}
