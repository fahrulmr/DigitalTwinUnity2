// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System.Configuration;
using UnityEditor;
using UnityEngine;

namespace game4automation
{
    [InitializeOnLoad]
    public static class Hotkeys
    {
        static GameObject active;
        private static Game4AutomationController g4a;

        public static void KeyEvent(KeyCode key)
        {
            active = Selection.activeGameObject;
            g4a = UnityEngine.Object.FindObjectOfType<Game4AutomationController>();
            if (g4a == null)
                return;
            if (key == g4a.HotkeySource)
            {
                if (g4a.EnableHotkeys)
                {
                    var source = g4a.StandardSource;
                    Debug.Log("Source created");
                    var newsource = (GameObject) PrefabUtility.InstantiatePrefab(source);
                    // check if drive or transportsurface is selected
                    if (active != null)
                    {
                        var drive = active.GetComponentInChildren<Drive>();
                        var surface = active.GetComponentInChildren<TransportSurface>();
                        if (!ReferenceEquals(surface, null))
                        {
                            var surfacemiddle = surface.GetMiddleTopPoint();
                            if (!ReferenceEquals(newsource, null))
                            {
                                var MU = newsource.GetComponent<MU>();
                                if (!ReferenceEquals(MU, null))
                                    MU.PlaceMUOnTopOfPosition(surfacemiddle);
                            }
                        }
                    }
                }
                return;
            }
        }


        static Hotkeys()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }


        private static void OnSceneGUI(SceneView sceneView)
        {
            Event current = Event.current;
            if (current.type != EventType.KeyDown)
                return;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
            {
                KeyEvent(Event.current.keyCode);
            }
        }
    }


    [InitializeOnLoad]
    public class CustomHierarchyView
    {
        private static bool keydown = false;

        static CustomHierarchyView()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            if (keydown == false)
            {
                if (Event.current.GetTypeForControl(instanceID) == EventType.KeyDown)
                {
                    Hotkeys.KeyEvent(Event.current.keyCode);
                    keydown = true;
                }
            }

            if (keydown == true)
            {
                if (Event.current.GetTypeForControl(instanceID) == EventType.KeyUp)
                {
                    keydown = false;
                }
            }
        }
    }
}