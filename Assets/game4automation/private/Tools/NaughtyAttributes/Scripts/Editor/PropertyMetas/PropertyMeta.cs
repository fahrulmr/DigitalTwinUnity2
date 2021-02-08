using UnityEditor;

namespace game4automationtools.Editor
{
    public abstract class PropertyMeta
    {
        public abstract void ApplyPropertyMeta(SerializedProperty property, MetaAttribute metaAttribute);
    }
}
