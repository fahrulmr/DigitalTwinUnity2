// Game4Automation (R) Parts4Cad
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using game4automationtools;
using UnityEditor;

namespace game4automation
{

    public class Parts4Cad : MonoBehaviour
    {

        [ReadOnly] public string Name;
        [ReadOnly] public string Description;
        [ReadOnly] public string OrderID;
        [ReadOnly] public string Catalog;
        [ReadOnly] public string Vendor;
        [ReadOnly] public string Supplier;
        public bool ShowAttributes = false;

        [ShowIf("ShowAttributes")] [ResizableTextArea]
        public string Attributes;

        [HideInInspector] public string Mident;

#if UNITY_EDITOR
        [Button("Update Part")]
        public void UpdatePart()
        {
            Parts4CadImport.UpdateParts4Cad(this);
        }
    
      

        [Button("Create New Part")]
        public void NewPart()
        {
            if (gameObject.transform.parent != null)
                Selection.activeObject = gameObject.transform.parent;
            Parts4CadImport.OpenParts4Cad(null);
        }
        #endif
        
    }
}