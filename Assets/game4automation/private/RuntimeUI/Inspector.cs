
using System.Collections.Generic;
using UnityEngine;

namespace game4automation
{
    public class Inspector : MonoBehaviour
    {
        public bool ShowInInspector = true;
        public bool ShowOnlyMarkedAttributes = false;
        public bool ShowOnlyDefinedComponents=true;
        public bool HideDefinedElements=false;
    
        public string HierarchyName;
        public string ComponentName;

        public List<Component> Elements;
        private Game4AutomationController Game4AutomationController;
    
        // Start is called before the first frame update
        void Start()
        {
            var g4a = GameObject.Find("game4automation");
            if (g4a!=null)
                Game4AutomationController = g4a.GetComponent<Game4AutomationController>();

            if (ShowInInspector)
                InitInspector();
        }

        void InitInspector()
        {

            Game4AutomationController.InspectorController.Add(this);
        }
  
    }


}

