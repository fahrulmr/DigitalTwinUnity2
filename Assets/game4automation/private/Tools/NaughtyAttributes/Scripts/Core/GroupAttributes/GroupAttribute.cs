using System;

namespace game4automationtools
{
    public abstract class GroupAttribute : NaughtyAttribute
    {
        public string Name { get; private set; }

        public GroupAttribute(string name)
        {
            this.Name = name;
        }
    }
}
