// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections;


namespace game4automation
{
    public class EditorParts4Cad : EditorWindow
    {
        [MenuItem("game4automation/Cadenas parts4cad", false, 700)]
        static void NewParts4Cad()
        {
           
            Parts4CadImport.OpenParts4Cad(null);
        }

    }
}