// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEditor;
using UnityEngine;


namespace game4automation
{
  
    [CustomEditor(typeof(Signal),true)]
    public class SignalInspectorWindow : UnityEditor.Editor {

        bool show = true;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Signal signal = (Signal)target;
            float win = Screen.width;
            float w1=win*0.5f; 
            float w2=win*0.5f;
        
            show = EditorGUILayout.Foldout(show ,"Signal Connection Info");
            if (show)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label("Behavior", GUILayout.Width(w1));
                GUILayout.Label("Connection", GUILayout.Width(w2));
                GUILayout.EndHorizontal();
                foreach (var signalinfo in signal.ConnectionInfo)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(15);
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(signalinfo.Behavior, typeof(Object), false,GUILayout.Width(w1));
                    EditorGUI.EndDisabledGroup();
                    GUILayout.Label(signalinfo.ConnectionName, GUILayout.Width(w2));
                    GUILayout.EndHorizontal();
                }
          
       
             
            }
        }

       
    }
}
