// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz    


using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using System.Collections.Generic;
using System.Linq;


namespace game4automation
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class Global
    {
        // Global Variables
        public static bool RuntimeInspectorEnabled = true;

        public static string Version = "";
        public static string Release = "";
        public static string Build = "";
        public static Game4AutomationController g4acontroller; // Game4Automation Controller of last Scene playing
        public static bool g4acontrollernotnull = false;
        #region GLOBALTOOLS

        public static void SetG4AController(Game4AutomationController controller)
        {
            if (controller != null)
            {
                g4acontroller = controller;
                g4acontrollernotnull = true;
            }
            else
            {
                g4acontroller = null;
                g4acontrollernotnull = false;
            }
        }
        
        public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
        {
            var result = new List<System.Type>();
            var assemblies = aAppDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(aType))
                        result.Add(type);
                }
            }

            return result.ToArray();
        }

        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject))
                .Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null)
                throw new System.MissingMemberException(
                    "Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException(
                    "Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = UnityEngine.Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int) showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect) positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            throw new System.NotSupportedException(
                "Can't find internal main window. Maybe something has changed inside Unity");
        }

#if UNITY_EDITOR
        public static void CenterOnMainWin(this UnityEditor.EditorWindow aWin)
        {
            var main = GetEditorMainWindowPos();
            var pos = aWin.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            aWin.position = pos;
        }
#endif
        public static Bounds GetTotalBounds(GameObject root)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            ;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                bounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return bounds;
        }

        public static void MovePositionKeepChildren(GameObject root, Vector3 deltalocalposition)
        {
            List<GameObject> childs = new List<GameObject>();
            // save all children
            foreach (Transform child in root.transform)
            {
                childs.Add(child.gameObject);
            }

            // temp unparent children
            root.transform.DetachChildren();
            root.transform.localPosition = root.transform.localPosition + deltalocalposition;
            foreach (var child in childs)
            {
                child.transform.parent = root.transform;
            }
        }

        public static void MoveRotationKeepChildren(GameObject root, Quaternion deltarotation)
        {
            List<GameObject> childs = new List<GameObject>();
            // save all children
            foreach (Transform child in root.transform)
            {
                childs.Add(child.gameObject);
            }

            // temp unparent children
            root.transform.DetachChildren();
            root.transform.localRotation = root.transform.localRotation * deltarotation;
            foreach (var child in childs)
            {
                child.transform.parent = root.transform;
            }
        }

        public static void SetPositionKeepChildren(GameObject root, Vector3 globalposition)
        {
            List<GameObject> childs = new List<GameObject>();
            // save all children
            foreach (Transform child in root.transform)
            {
                childs.Add(child.gameObject);
            }

            // temp unparent children
            root.transform.DetachChildren();
            root.transform.position = globalposition;
            foreach (var child in childs)
            {
                child.transform.parent = root.transform;
            }
        }

        public static void SetRotationKeepChildren(GameObject root, Quaternion rotation)
        {
            List<GameObject> childs = new List<GameObject>();
            // save all children
            foreach (Transform child in root.transform)
            {
                childs.Add(child.gameObject);
            }

            // temp unparent children
            root.transform.DetachChildren();
            root.transform.rotation = rotation;
            foreach (var child in childs)
            {
                child.transform.parent = root.transform;
            }
        }

        public static Vector3 GetTotalCenter(GameObject root)
        {
            var bounds = GetTotalBounds(root);
            return bounds.center;
        }

        public static Object[] GatherObjects(GameObject root)
        {
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            Stack<GameObject> recurseStack = new Stack<GameObject>(new GameObject[] {root});

            while (recurseStack.Count > 0)
            {
                GameObject obj = recurseStack.Pop();
                objects.Add(obj);

                foreach (Transform childT in obj.transform)
                    recurseStack.Push(childT.gameObject);
            }

            return objects.ToArray();
        }

        public static bool IsGame4AutomationTypeIncluded(GameObject target)
        {
            Game4AutomationBehavior[] behavior = target.GetComponentsInChildren<Game4AutomationBehavior>();
            var length = behavior.Length;
            if (length == 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsGame4AutomationViewTypeIncluded(GameObject target)
        {
            Game4AutomationBehavior[] behavior = target.GetComponentsInChildren<Game4AutomationBehavior>();

            var found = 0;
            foreach (var behav in behavior)
            {
#if GAME4AUTOMATION_PROFESSIONAL
                if ((behav.GetType() == typeof(Group))
                    || (behav.GetType() == typeof(CAD)))
#else
                if ((behav.GetType() == typeof(Group)))
#endif
                {
                    found++;
                }
            }

            var length = behavior.Length - found;
            if (length == 0)
            {
                return false;
            }

            return true;
        }


#if UNITY_EDITOR
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
            if (symbols.Contains(";" + mydefine))
            {
                symbols = symbols.Replace(";" + mydefine, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currtarget, symbols);
            }

            if (symbols.Contains(mydefine))
            {
                symbols = symbols.Replace(mydefine, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currtarget, symbols);
            }
        }

        public static void SetAssemblyDefReference(string assemblydef, string reference, bool set)
        {
            var path = Path.Combine(Application.dataPath, assemblydef);
            string assydef = File.ReadAllText(path);
            if (set)
            {
                // already there
                if (assydef.Contains(reference))
                    return;
                var search = "\"references\": [";
                var pos = assydef.IndexOf(search) + search.Length;

                var insertvalue = "\n        \"" + reference + "\",";
                assydef = assydef.Insert(pos, insertvalue);
            }

            if (!set)
            {
                if (!assydef.Contains(reference))
                    return;
                var start = assydef.IndexOf(reference);
                var posend = assydef.IndexOf(",", start);
                var posstart = assydef.LastIndexOf("\n", posend);
                assydef = assydef.Remove(posstart, posend - posstart + 1);
            }

            File.WriteAllText(path, assydef);
        }

        public static void AddComponent(string assetpath)
        {
            GameObject component = Selection.activeGameObject;
            Object prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject));
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = new Vector3(0, 0, 0);
            if (component != null)
            {
                go.transform.parent = component.transform;
            }

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }

        public static GameObject AddComponentTo(Transform transform, string assetpath)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject));
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = new Vector3(0, 0, 0);
            if (transform != null)
            {
                go.transform.parent = transform;
            }

            return go;
        }


        public static void SetVisible(GameObject target, bool isActive)
        {
            if (target.activeSelf == isActive) return;

            target.SetActive(isActive);
            EditorUtility.SetDirty(target);

            Object[] objects = GatherObjects(target);
            foreach (Object obj in objects)
            {
                GameObject go = (GameObject) obj;
                go.SetActive(isActive);
                EditorUtility.SetDirty(go);
            }

            if (Selection.objects.Length > 1)
                foreach (var obj in Selection.objects)
                {
                    if (obj.GetType() == typeof(GameObject))
                    {
                        if (obj != target)
                            SetVisible((GameObject) obj, isActive);
                    }
                }
        }

        public static void HideSubObjects(GameObject target, bool hide)
        {
            if (ReferenceEquals(g4acontroller, null))
                return;

            EditorUtility.SetDirty(g4acontroller);
            if (!hide)
            {
                if (g4acontroller.ObjectsWithHiddenSubobjects.Contains(target))
                    g4acontroller.ObjectsWithHiddenSubobjects.Remove(target);

                Object[] objects = GatherObjects(target);

                foreach (Object obj in objects)
                {
                    if (g4acontroller.ObjectsWithHiddenSubobjects.Contains((GameObject) obj))
                        g4acontroller.ObjectsWithHiddenSubobjects.Remove((GameObject) obj);
                    obj.hideFlags = HideFlags.None;
                    if (g4acontroller.HiddenSubobjects.Contains((GameObject) obj))
                        g4acontroller.HiddenSubobjects.Remove((GameObject) obj);
                    if (g4acontroller.InObjectWithHiddenSubobjects.Contains((GameObject) obj))
                        g4acontroller.InObjectWithHiddenSubobjects.Remove((GameObject) obj);
                }

                SetExpandedRecursive(target, true);
                EditorApplication.DirtyHierarchyWindowSorting();
            }
            else
            {
                g4acontroller.ObjectsWithHiddenSubobjects.Add(target);
                SetExpandedRecursive(target, true);
                Object[] objects = GatherObjects(target);
                foreach (Object obj in objects)
                {
                    if (IsGame4AutomationViewTypeIncluded((GameObject) obj) == false && obj != target)
                    {
                        obj.hideFlags = HideFlags.HideInHierarchy;
                        if (!g4acontroller.HiddenSubobjects.Contains((GameObject) obj))
                            g4acontroller.HiddenSubobjects.Add((GameObject) obj);
                    }
                    else
                    {
                        if (obj != target)
                        {
                            if (!g4acontroller.InObjectWithHiddenSubobjects.Contains((GameObject) obj))
                                g4acontroller.InObjectWithHiddenSubobjects.Add((GameObject) obj);
                        }
                    }
                }

                EditorApplication.DirtyHierarchyWindowSorting();
            }
        }

        public static void SetLockObject(GameObject target, bool isLocked)
        {
            bool objectLockState = (target.hideFlags & HideFlags.NotEditable) > 0;
            if (objectLockState == isLocked)
                return;

            Object[] objects = GatherObjects(target);

            if (isLocked && g4acontroller != null)
            {
                if (!g4acontroller.LockedObjects.Contains(target))
                    g4acontroller.LockedObjects.Add(target);
            }
            else
                g4acontroller.LockedObjects.Remove(target);

            foreach (Object obj in objects)
            {
                GameObject go = (GameObject) obj;
                string undoString = string.Format("{0} {1}", isLocked ? "Lock" : "Unlock", go.name);
                Undo.RecordObject(go, undoString);

                // Set state according to isLocked
                if (isLocked)
                {
                    go.hideFlags |= HideFlags.NotEditable;
                }
                else
                {
                    if (Global.g4acontrollernotnull)
                        g4acontroller.LockedObjects.Remove(go);
                    go.hideFlags &= ~HideFlags.NotEditable;
                }

                // Set hideflags of components
                foreach (Component comp in go.GetComponents<Component>())
                {
                    if (comp is Transform)
                        continue;

                    Undo.RecordObject(comp, undoString);

                    if (isLocked)
                    {
                        comp.hideFlags |= HideFlags.NotEditable;
                        comp.hideFlags |= HideFlags.HideInHierarchy;
                    }
                    else
                    {
                        comp.hideFlags &= ~HideFlags.NotEditable;
                        comp.hideFlags &= ~HideFlags.HideInHierarchy;
                    }

                    EditorUtility.SetDirty(comp);
                }

                EditorUtility.SetDirty(go);
                if (g4acontroller != null)
                    EditorUtility.SetDirty(g4acontroller);
            }
        }

        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            System.Reflection.MethodInfo methodInfo = type.GetMethod("SetExpandedRecursive");
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            EditorWindow editorWindow = EditorWindow.focusedWindow;
            methodInfo.Invoke(editorWindow, new object[] {go.GetInstanceID(), expand});
        }
#endif

        #endregion

        #region VERSION

        // Get Version
        public static void IncrementVersion()
        {
#if UNITY_EDITOR
            Game4AutomationVersion scriptableversion =
                UnityEngine.Resources.Load<Game4AutomationVersion>("Game4AutomationVersion");
            scriptableversion.Build = scriptableversion.Build + 1;
#endif
        }

        public static void SetVersion()
        {
            Game4AutomationVersion scriptableversion =
                UnityEngine.Resources.Load<Game4AutomationVersion>("Game4AutomationVersion");
            if (scriptableversion != null)
            {
                Build = scriptableversion.Build.ToString();
                Build = Build.Replace("\n", "");


                Release = scriptableversion.Release;
                scriptableversion.Build = int.Parse(Build);
                Version = Release + "." + Build + " (Unity " + Application.unityVersion + ")";
            }
        }

        static Global()
        {
            Initialize();
        }

        #endregion


        #region EVENTS

#if UNITY_EDITOR
        // Global Events
        public static void OnSceneLoaded(Scene scene, OpenSceneMode mode)
        {
            Debug.Log("Game4Automation scene " + scene.name + " loaded");
            try
            {
                var rootobjs = scene.GetRootGameObjects();
                foreach (var rootobj in rootobjs)
                {
                    if (rootobj.GetComponent<SceneInfo>() != null)
                        rootobj.GetComponent<SceneInfo>().OnSceneLoad();
                }
            }
            catch (Exception e)
            {
                var error = e;
            }
         
        }

        // Global Events
        public static void OnSceneClosing(Scene scene, bool removing)
        {
            QuickToggle.SetGame4Automation(null);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Debug.Log(state);
            if (g4acontrollernotnull)
                if (state == PlayModeStateChange.EnteredEditMode)
                    g4acontroller.OnPlayModeFinished();
        }

#endif

        // When Unity Is Loaded
#if !UNITY_EDITOR
      [RuntimeInitializeOnLoadMethod]
#endif
        public static void Initialize()
        {
            SetVersion();

#if UNITY_EDITOR
            EditorSceneManager.sceneOpened += OnSceneLoaded;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        #endregion
    }
}

// Started Before Build
#if UNITY_EDITOR
class Game4AutomationBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPreprocessBuild(BuildReport target)
    {
        game4automation.Global.Initialize();
    }
}

#endif