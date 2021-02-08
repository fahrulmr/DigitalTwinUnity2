using UnityEditor;

namespace game4automationtools.Editor
{
    public abstract class PropertyDrawer
    {
        public abstract void DrawProperty(SerializedProperty property);

        public virtual void ClearCache()
        {

        }
    }
}
