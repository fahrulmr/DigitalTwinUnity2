// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  



  using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if GAME4AUTOMATION_PLAYMAKER
using PlayMaker;
#endif

namespace game4automation
{
    [InitializeOnLoad]
    public class Game4AutomationToolbar : EditorWindow
    {
        private bool groupEnabled;
       
        [MenuItem("game4automation/Create new game4automation Scene", false, 1)]
        static void CreateNewScene()
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            AddComponent("Assets/game4automation/game4automation.prefab");
        }
        
        
        [MenuItem("game4automation/Export full project as package", false, 51)]
        static void ExportWholeProjet()
        {
            string[] s = Application.dataPath.Split('/');
            string projectName = s[s.Length - 2];
            
            var path = EditorUtility.SaveFilePanel(
                "Export full project as package",
                "",
                projectName,
                "unitypackage");

            if (path.Length != 0)
            {
                AssetDatabase.ExportPackage ("Assets", path, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);
            }
        
        }
        
        [MenuItem("game4automation/Export current scene as package", false, 52)]
        static void ExportScene()
        {

            string projectName = SceneManager.GetActiveScene().name;
            string assetpath = SceneManager.GetActiveScene().path;
            var path = EditorUtility.SaveFilePanel(
                "Export current scene including dependencies as package",
                "",
                projectName,
                "unitypackage");

            if (path.Length != 0)
            {
                AssetDatabase.ExportPackage (assetpath, path, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            }
        
        }
        
        [MenuItem("game4automation/Export selected as package", false, 53)]
        static void ExportSelected()
        {
            var selected = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            var obj1 = selected[0];
            var projectName = obj1.name;
            string assetpath =  AssetDatabase.GetAssetPath (obj1);
            var path = EditorUtility.SaveFilePanel(
                "Export selected including dependencies as package",
                "",
                projectName,
                "unitypackage");

            if (path.Length != 0)
            {
                AssetDatabase.ExportPackage (assetpath, path, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            }
        
        }


        [MenuItem("game4automation/Add CAD Link (Pro)", false, 150)]
        static void AddCADLink()
        {
            var find = AssetDatabase.FindAssets(
                "CADLink t:prefab");
            if (find.Length > 0)
                AddComponent(AssetDatabase.GUIDToAssetPath(find[0]));
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "CADLink is only included in Game4Automation Professional", "OK");
            }
        }
        
        [MenuItem("game4automation/Add Component/Source", false, 160)]
        static void AddSource()
        {
            AddScript(typeof(Source));
        }
        
        [MenuItem("game4automation/Add Component/MU", false, 160)]
        static void AddMU()
        {
            AddScript(typeof(MU));
        }

        [MenuItem("game4automation/Add Component/Drive", false, 160)]
        static void AddDrive()
        {
            AddScript(typeof(Drive));
        }

        [MenuItem("game4automation/Add Component/Transport Surface", false, 160)]
        static void AddTransportSurface()
        {
            AddScript(typeof(TransportSurface));
        }

        [MenuItem("game4automation/Add Component/Drive Behaviour/Simple Drive", false, 160)]
        static void AddDriveBehaviourSimple()
        {
            AddScript(typeof(Drive_Simple));
        }

        [MenuItem("game4automation/Add Component/Drive Behaviour/Destination Drive", false, 160)]
        static void AddDriveBehaviourDestination()
        {
            AddScript(typeof(Drive_DestinationMotor));
        }

        [MenuItem("game4automation/Add Component/Drive Behaviour/Cylinder", false, 160)]
        static void AddDriveBehaviourCylinder()
        {
            AddScript(typeof(Drive_Cylinder));
        }

        [MenuItem("game4automation/Add Component/Drive Behaviour/Gear", false, 160)]
        static void AddDriveBehaviourGear()
        {
            AddScript(typeof(Drive_Gear));
        }

        [MenuItem("game4automation/Add Component/Sensor", false, 160)]
        static void AddSensor()
        {
            AddScript(typeof(Sensor));
        }

        [MenuItem("game4automation/Add Component/Sensor Behaviour/Standard", false, 160)]
        static void AddSensorBehaviourStandard()
        {
            AddScript(typeof(Sensor_Standard));
        }

        [MenuItem("game4automation/Add Component/Grip", false, 160)]
        static void AddGrip()
        {
            AddScript(typeof(Grip));
        }

        [MenuItem("game4automation/Add Component/Sink", false, 160)]
        static void AddSink()
        {
            AddScript(typeof(Sink));
        }
        
        
        [MenuItem("game4automation/Add Component/Group", false, 160)]
        static void AddGroup()
        {
            AddScript(typeof(Group));
        }
        
        [MenuItem("game4automation/Add Component/Kinematic", false, 160)]
        static void AddKinematicScript()
        {
            AddScript(typeof(Kinematic));
        }
        
        [MenuItem("game4automation/Add Component/Chain", false, 160)]
        static void AddChainScript()
        {
            AddScript(typeof(Chain));
        }
        
        [MenuItem("game4automation/Add Component/Chain element", false, 160)]
        static void AddChainElementScript()
        {
            AddScript(typeof(ChainElement));
        }

        [MenuItem("game4automation/Add Component/Playmaker FSM (Pro)", false, 160)]
        static void AddPlaymakerFSM()
        {
#if GAME4AUTOMATION_PLAYMAKER
            AddScript(typeof(PlayMakerFSM));
#else
            string sym = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (sym.Contains("GAME4AUTOMATION_PROFESSIONAL"))
            {
                EditorUtility.DisplayDialog("Info",
                    "You need to purchase and download Playmaker on the Unity Asset Store before using it. GAME4AUTOMATION_PLAYMAKER needs to be set in Scripting Define Symbols.",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info",
                    "The Playmaker Actions are only included in Game4Automatoin Professional.",
                    "OK");
            }
#endif
        }

        [MenuItem("game4automation/Add Object/Sensor Beam", false, 151)]
        static void AddSensorBeamn()
        {
            AddComponent("Assets/game4automation/SensorBeam.prefab");
        }

        [MenuItem("game4automation/Add Object/Lamp", false, 151)]
        static void AddLamp()
        {
            AddComponent("Assets/game4automation/Lamp.prefab");
        }

        [MenuItem("game4automation/Add Object/UI/Button", false, 151)]
        static void AddPushButton()
        {
            AddComponent("Assets/game4automation/UIButton.prefab");
        }

        [MenuItem("game4automation/Add Object/UI/Lamp", false, 151)]
        static void AddUILamp()
        {
            AddComponent("Assets/game4automation/UILamp.prefab");
        }

        [MenuItem("game4automation/Add Object/UI/Empty space", false, 151)]
        static void AddUIEmpty()
        {
            AddComponent("Assets/game4automation/UILamp.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Input Bool", false, 155)]
        static void AddPLCInputBool()
        {
            AddComponent("Assets/game4automation/PLCInputBool.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Input Float", false, 155)]
        static void AddPLCInputFloat()
        {
            AddComponent("Assets/game4automation/PLCInputFloat.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Input Int", false, 155)]
        static void AddPLCInpuInt()
        {
            AddComponent("Assets/game4automation/PLCInputInt.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Output Bool", false, 155)]
        static void AddPLCOutputBool()
        {
            AddComponent("Assets/game4automation/PLCOutputBool.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Output Float", false, 155)]
        static void AddPLCOutputFloat()
        {
            AddComponent("Assets/game4automation/PLCOutputFloat.prefab");
        }

        [MenuItem("game4automation/Add Object/Signal/PLC Output Int", false, 155)]
        static void AddPLCOutputInt()
        {
            AddComponent("Assets/game4automation/PLCOutputInt.prefab");
        }


        [MenuItem("game4automation/Add Object/Interface/S7", false, 155)]
        static void AddS7Interface()
        {
            AddComponent("Assets/game4automation/S7Interface.prefab");
        }
        
        [MenuItem("game4automation/Add Object/Game4Automation", false, 160)]
        static void AddGame4Automatoin()
        {
            AddComponent("Assets/game4automation/game4automation.prefab");
        }

        
#if UNITY_STANDALONE_WIN
        [MenuItem("game4automation/Add Object/Interface/TwinCAT ADS (Pro)", false, 156)]
        static void AddTwinCATInterface()
        {
            var find = AssetDatabase.FindAssets(
                "TwinCATInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/twinCAT/TwinCATInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }
#endif


#if UNITY_STANDALONE_WIN
        [MenuItem("game4automation/Add Object/Interface/Shared Memory (Pro)", false, 156)]
        static void AddSharedMemoryInterface()
        {
            var find = AssetDatabase.FindAssets(
                "SharedMemoryInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/SharedMemory/SharedMemoryInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }
#endif

#if UNITY_STANDALONE_WIN
        [MenuItem("game4automation/Add Object/Interface/PLCSIMAdvanced (Pro)", false, 157)]
        static void AddPLCSimAdvancedInterface()
        {
            var find = AssetDatabase.FindAssets(
                "PLCSIMAdvancedInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/PLCSimAdvanced/PLCSIMAdvancedInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }
#endif


        [MenuItem("game4automation/Add Object/Interface/OPCUA (Pro)", false, 155)]
        static void AddOPCUAInterface()
        {
            var find = AssetDatabase.FindAssets(
                "OPCUAInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/OPCUA4UNITY/OPCUAInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }
        
#if UNITY_STANDALONE_WIN
        [MenuItem("game4automation/Add Object/Interface/RoboDK (Pro)", false, 157)]
        static void AddRoboDKInterface()
        {
            var find = AssetDatabase.FindAssets(
                "RoboDKInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/RoboDK/RoboDKInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }
#endif
        

        [MenuItem("game4automation/Add Object/Interface/Modbus (Pro)", false, 157)]
        static void AddPLCConnectInterface()
        {
            var find = AssetDatabase.FindAssets(
                "ModbusInterface t:prefab");
            if (find.Length > 0)
                AddComponent("Assets/game4automation/private/Interfaces/Modbus/ModbusInterface.prefab");
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This interface is only included in Game4Automation Professional", "OK");
            }
        }



        [MenuItem("game4automation/Add Object/Kinematic", false, 156)]
        static void AddKinematic()
        {
            AddComponent("Assets/game4automation/Kinematic.prefab");
        }
        
        [MenuItem("game4automation/Add Object/CAD Pixyz Update (Pro)", false, 156)]
        static void AddCAD()
        {
            var find = AssetDatabase.FindAssets(
                "CAD t:prefab");
            if (find.Length > 0)
            {
                
               var com = AddComponent("Assets/game4automation/private/ProductiveTools/CADUpdater/CAD.prefab");
               PrefabUtility.UnpackPrefabInstance(com,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            }

            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "This CAD update function is only included in Game4Automation Professional", "OK");
            }
        }
   
        [MenuItem("game4automation/Apply standard settings", false, 500)]
        private static void SetStandardSettingsMenu()
        {
            SetStandardSettings(true);
        }

        
            
        [MenuItem("game4automation/Simple Hierarchy Unfolded", false, 601)]
        static void SimpleHierarchyViewUnfolded()
        {
            if (Global.g4acontrollernotnull)
                Global.g4acontroller.SetSimpleView(true,true);
         
        }
        
        [MenuItem("game4automation/Simple Hierarchy Folded", false, 601)]
        static void SimpleHierarchyViewFolded()
        {
            if (Global.g4acontrollernotnull)
                Global.g4acontroller.SetSimpleView(true,false);
         
        }
        
        [MenuItem("game4automation/Reset Hierarchy View", false, 601)]
        static void NormalHierarchyView()
        {
            if (Global.g4acontrollernotnull)
                Global.g4acontroller.ResetView();
         
        }
        
  
        
   
        public static void SetDefine(string mydefine)
        {
            var currtarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currtarget);
            if (!symbols.Contains(mydefine))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currtarget, symbols + ";" + mydefine);
            }
        
        }

        public static void DeleteDefine(string mydefine)
        {
            var currtarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currtarget);
            if (symbols.Contains(";"+ mydefine))
            {
                symbols = symbols.Replace(";" + mydefine, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currtarget, symbols);
            }
            if (symbols.Contains(mydefine))
            {
                symbols = symbols.Replace( mydefine, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currtarget, symbols);
            }
        }

        public static void SetStandardSettings(bool message)
        {
            List<string> layerstoconsider = new List<string>
            {
                "g4a Debug", "g4a Controller", "g4a Lamps", "g4a SensorMU", "g4a TransportMU", "g4a Sensor",
                "g4a Transport", "g4a MU", "g4a Postprocessing"
            };
            List<string> collission = new List<string>
            {
                "g4a SensorMU-g4a Sensor",
                "g4a TransportMU/g4a Transport",
                "g4a TransportMU/g4a TransportMU",
                "g4a SensorMU/g4a Sensor",
                "g4a Sensor/g4a MU",
                "g4a Transport/g4a TransportMU",
                "g4a Transport/g4a MU",
                "g4a Transport/g4a Transport",
                "g4a MU/g4a MU",
            };

            var layernumber = 13;
            foreach (var layer in layerstoconsider)
            {
                CreateLayer(layer, layernumber);
                layernumber++;
            }

            for (int x = 0; x < 31; x++)
            {
                for (int y = 0; y < 31; y++)
                {
                    string layerx = LayerMask.LayerToName(x);
                    string layery = LayerMask.LayerToName(y);
                    string index1 = layerx + "/" + layery;
                    string index2 = layery + "/" + layerx;
                    if (layerstoconsider.Contains(layerx) || layerstoconsider.Contains(layery))
                    {
                        if (collission.Contains(index1) || collission.Contains(index2))
                        {
                            UnityEngine.Physics.IgnoreLayerCollision(x, y, false);
                        }
                        else
                        {
                            UnityEngine.Physics.IgnoreLayerCollision(x, y, true);
                            UnityEngine.Physics.IgnoreLayerCollision(x, y, true);
                        }
                    }
                }
            }

            ToggleGizmos(false);
            PlayerSettings.colorSpace = ColorSpace.Linear;
            QuickToggle.ShowQuickToggle(true);
            SetDefine("GAME4AUTOMATION");

            // Check if Professional Version and set define
            if (AssetDatabase.IsValidFolder("Assets/Playmaker"))
            {
                SetDefine("GAME4AUTOMATION_PLAYMAKER");
            }

            var alllayers = ~0;
            Tools.visibleLayers = alllayers & ~(1 << LayerMask.NameToLayer("UI"));

            // Check if Playmaker  and set define
            if (AssetDatabase.IsValidFolder("Assets/game4automation/private/Interfaces/SharedMemory"))
            {
                SetDefine("GAME4AUTOMATION_PROFESSIONAL");
            }

            if (!message)
                EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoGame4Automation.unity");
            if (message)
                EditorUtility.DisplayDialog("Game4Automatoin Standard Settings applied",
                    "Game4Automatoin standard settings are applied (Layers are created, UI Layer hide and all Gizmos off, linear color space",
                    "OK");
        }

        [MenuItem("game4automation/Open demo scene", false, 700)]
        static void OpenDemoScene()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoGame4Automation.unity");
        }
        
          
        [MenuItem("game4automation/Additional demos/Radial conveyor", false, 700)]
        static void OpenDemoRadial()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/RadialConveyordemo.unity");
        }
        
        [MenuItem("game4automation/Additional demos/Moving drives with CAM profiles", false, 700)]
        static void OpenDemoCAM()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/CAMDemo.unity");
        }
        
        [MenuItem("game4automation/Additional demos/Changing MU appearance", false, 700)]
        static void OpenDemoChangeMU()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoChangeMU.unity");
        }
          
        [MenuItem("game4automation/Additional demos/Modelling chain systems", false, 700)]
        static void OpenDemoChain()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoChain.unity");
        }

        [MenuItem("game4automation/Additional demos/Simulate industrial robots with RoboDK (Pro)", false, 700)]
        static void OpenDemoRoboDK()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoRoboDK.unity");
        }
        
        [MenuItem("game4automation/Additional demos/Starting Drives with Conditions", false, 700)]
        static void OpenDemoStartingDrives()
        {
            EditorSceneManager.OpenScene("Assets/game4automation/Scenes/DemoStartDriveOnCondition.unity");
        }

        [MenuItem("game4automation/Documentation ", false, 701)]
        static void OpenDocumentation()
        {
            Application.OpenURL("https://game4automation.com/documentation/current/index.html");
        }

        [MenuItem("game4automation/About", false, 702)]
        static void Info()
        {
            Application.OpenURL("https://game4automation.com");
        }

        public static void ToggleGizmos(bool gizmosOn)
        {
#if UNITY_2018
            int val = gizmosOn ? 1 : 0;
            Assembly asm = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            System.Type type = asm.GetType("UnityEditor.AnnotationUtility");
            if (type != null)
            {
                MethodInfo getAnnotations =
                    type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo setGizmoEnabled =
                    type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo setIconEnabled =
                    type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                var annotations = getAnnotations.Invoke(null, null);
                foreach (object annotation in (IEnumerable) annotations)
                {
                    System.Type annotationType = annotation.GetType();
                    FieldInfo classIdField =
                        annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                    FieldInfo scriptClassField =
                        annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                    if (classIdField != null && scriptClassField != null)
                    {
                        int classId = (int) classIdField.GetValue(annotation);
                        string scriptClass = (string) scriptClassField.GetValue(annotation);
                        setGizmoEnabled.Invoke(null, new object[] {classId, scriptClass, val});
                        setIconEnabled.Invoke(null, new object[] {classId, scriptClass, val});
                    }
                }
            }
#endif
        }


        private static void CreateLayer(string name, int number)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            var tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var propCount = layerProps.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < propCount; i++)
            {
                var layerProp = layerProps.GetArrayElementAtIndex(i);

                var stringValue = layerProp.stringValue;

                if (stringValue == name && i != number)
                {
                    layerProp.stringValue = string.Empty;
                }

                //if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null && i == number)
                    firstEmptyProp = layerProp;
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name +
                                           "\" not created.");
                return;
            }

            firstEmptyProp.stringValue = name;
            tagManager.ApplyModifiedProperties();
        }

        static void AddScript(System.Type type)
        {
            GameObject component = Selection.activeGameObject;
            
            if (component != null)
            {
                Undo.AddComponent(component, type);
            }
            else
            {
                EditorUtility.DisplayDialog("Please select an Object",
                    "Please select first an Object where the script should be added to!",
                    "OK");
            }
        }

        static GameObject AddComponent(string assetpath)
              {
                  GameObject component = Selection.activeGameObject;
                  Object prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject));
                  GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
      
                  if (go != null)
                  {
                      go.transform.position = new Vector3(0, 0, 0);
                      if (component != null)
                      {
                          go.transform.parent = component.transform;
                      }
      
                      Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
      
                      
                  }
                  return go;
              }
    }
}