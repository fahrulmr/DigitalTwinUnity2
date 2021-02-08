// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEditor;
using UnityEngine;


namespace game4automation
{
    [CustomEditor(typeof(S7Interface))]
    public class S7InterfaceEditor : UnityEditor.Editor {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            S7Interface myScript = (S7Interface)target;
            
            
            if(GUILayout.Button("Select symbol table"))
            {
                var File = ""; 
                File = EditorUtility.OpenFilePanel("Select file to import", File, "sdf");
                myScript.SymbolTable = File;
            }
            
            if(GUILayout.Button("Import symbol table"))
            {
                myScript.ReadSignalFile();
            }
            if(GUILayout.Button("Check Connection"))
            {
                myScript.CheckConnection();
            }
       
        }

       
    }
}