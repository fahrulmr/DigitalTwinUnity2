using System;

namespace game4automationtools
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : DrawerAttribute
    {
        public string Text { get; private set; }

        public ButtonAttribute(string text = null)
        {
            this.Text = text;
        }
    }
}
