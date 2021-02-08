using UnityEditor;

namespace game4automationtools.Editor
{
    [PropertyDrawCondition(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawCondition : ShowIfPropertyDrawCondition
    {
    }
}
