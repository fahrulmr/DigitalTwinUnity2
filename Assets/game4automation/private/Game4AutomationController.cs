// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

#pragma warning disable 0168
#pragma warning disable 0649

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using game4automationtools;
using UnityEngine.Rendering;
using System.Collections.Generic;


#if (UNITY_POST_PROCESSING_STACK_V2)
using UnityEngine.Rendering.PostProcessing;
#endif


namespace game4automation
{
    [ExecuteAlways]
    //! This object needs to be in every game4automation scene. It controls main central data (like scale...) and manages main game4automation settings for the scene.
    [HelpURL("https://game4automation.com/documentation/current/game4automation.html")]
    public class Game4AutomationController : Game4AutomationBehavior
    {
        #region PublicVariables    
        [HideInInspector] public  ActiveOnly ActiveOn;
        [HideInInspector] public string Version;

        [Header("General options")] public bool Connected = true;
        public float Scale = 1000;
    
        [Range(0, 10)] public float SpeedOverride = 1;
        [ReorderableList] public List<string> HideGroups;
        private int _lastmuid = 0;
        private bool _stepwise = false;
        private GameObject _debugconsole;

        [OnValueChanged("ChangedTimeScale")] [Range(0, 20)]
        public float TimeScale = 1;

        [HideInInspector] public bool EnablePositionDebug = true;
        [HideInInspector] public int DebugLayer = 13;


        [Header("UI options Editor")] public float HierarchyUpdateCycle = 0.2f;
        [BoxGroup("Hierarchy Icons")] public bool ShowHierarchyIcons = true;

        [BoxGroup("Hierarchy Icons")] [ShowIf("ShowHierarchyIcons")]
        public bool ShowHide = true;

        [BoxGroup("Hierarchy Icons")] [ShowIf("ShowHierarchyIcons")]
        public bool ShowFilter;

        [BoxGroup("Hierarchy Icons")] [ShowIf("ShowHierarchyIcons")]
        public bool ShowComponents = true;


        [Slider(0f, 2f)] public float ScaleHandles = 1;
        public GameObject StandardSource;

        [BoxGroup("Hotkeys")] public bool EnableHotkeys = true;

        [ShowIf("EnableHotkeys")] [BoxGroup("Hotkeys")]
        public KeyCode HotkeySource;

        [ShowIf("EnableHotkeys")] [BoxGroup("Hotkeys")]
        public KeyCode HotkeyDelete;
        
        [ShowIf("EnableHotkeys")] [BoxGroup("Hotkeys")]
        public KeyCode HotkeyCreateOnSource;

        

        [Header("UI options Runtime")] public bool UIEnabledOnStart = true;
        [ShowIf("UIEnabledOnStart")] public bool RuntimeInspectorEnabled = true;

        [ShowIf("UIEnabledOnStart")] public bool HideInfoBox = false;
        public GameObject RuntimeApplicationUI;
        public GameObject RuntimeAutomationUI;
        

        [Header("Environment")] [OnValueChanged("ChangedVisual")]
        public bool EnableEnvironmentAndQuality = true;

        [OnValueChanged("ChangedVisual")] public Material VRSkybox;

        [OnValueChanged("ChangedVisual")] public bool StandardBasePlateAndLights = true;

        [EnableIf("StandardBasePlateAndLights")] [OnValueChanged("ChangedVisual")]
        public float BasePlateDimensions = 4f;

        [OnValueChanged("ChangedVisual")] public bool VREnvironment;

        [OnValueChanged("ChangedVisual")] public GameObject EnvironmentAsset;

        [OnValueChanged("ChangedVisual")] [Slider(0f, 5f)] [Header("Lights")]
        public float SunLightIntensity = 2f;

        [OnValueChanged("ChangedVisual")] [Slider(0f, 5f)]
        public float FirstLightIntensity = 3f;

        [OnValueChanged("ChangedVisual")] [Slider(0f, 5f)]
        public float SecondLightIntensity = 1.5f;

        [Header("Visual quality")] [OnValueChanged("ChangedVisual")] [Dropdown("qualityvalues")]
        public int QualityLevel = 5;

        [OnValueChanged("ChangedVisual")] [Slider(0f, 2f)]
        public float GlobalLight = 0.8f;

        [Header("Visual quality")]
#if !(UNITY_POST_PROCESSING_STACK_V2 && !UNITY_ANDROID && !UNITY_IOS)
        [InfoBox(
            "You need to integrate Unity's post processing stack (v2) via the Package manager to use the HQ Visual settings. Please also add Unity.Postprocessing.Runtime to the game4automation.base Assembly Defintion.",
            InfoBoxType.Warning)]
#endif
        [OnValueChanged("ChangedVisual")]
        public bool EnableHQ = true;

        [HideInInspector] public List<GameObject> ObjectsWithHiddenSubobjects;
        [HideInInspector] public List<GameObject> HiddenSubobjects;
        [HideInInspector] public List<GameObject> InObjectWithHiddenSubobjects;
        [HideInInspector] public List<GameObject> LockedObjects;
        [HideInInspector] public List<string> HiddenGroups;
        [HideInInspector] public List<GameObject> ConnectionsActive = new List<GameObject>();

#if (UNITY_POST_PROCESSING_STACK_V2 && !UNITY_ANDROID && !UNITY_IOS)
        [ShowIf("EnableHQ")] [OnValueChanged("ChangedVisual")]
        public bool Postprocessing = true;

        [ShowIf("EnableHQ")] [Slider(0f, 0.1f)] [OnValueChanged("ChangedVisual")]
        public float Bloom = 0.01f;

        [ShowIf("EnableHQ")] [Slider(0f, 4f)] [OnValueChanged("ChangedVisual")]
        public float AmbientOcclusion = 0.64f;

        [ShowIf("EnableHQ")] [OnValueChanged("ChangedVisual")]
        public bool Reflections = true;

        [ShowIf("EnableHQ")] [Slider(0f, 1)] [OnValueChanged("ChangedVisual")]
        public float ReflectionDistanceFade = 0.4f;

        [ShowIf("EnableHQ")] [Slider(-100f, 100f)] [OnValueChanged("ChangedVisual")]
        public float Brightness = 0f;

        [ShowIf("EnableHQ")] [Slider(-100f, 100f)] [OnValueChanged("ChangedVisual")]
        public float Saturation = 0f;

        [ShowIf("EnableHQ")] [Slider(-100f, 100f)] [OnValueChanged("ChangedVisual")]
        public float Contrast = 0f;

        [ShowIf("EnableHQ")] [Slider(-100f, 100f)] [OnValueChanged("ChangedVisual")]
        public float Temperature = 0f;
#endif

        #endregion

        #region Private Variables    

        private float[] scalevalues = new float[] {1, 10, 100, 1000};
        private float[] speedvalues = new float[] {0.1f, 0.5f, 1, 1.5f, 2, 5, 10, 20};
        private float[] timescalevalues = new float[] {0.1f, 0.5f, 1, 1.5f, 2, 5, 10, 20};

        private DropdownList<int> qualityvalues = new DropdownList<int>()
        {
            {"Very Low", 0},
            {"Low", 1},
            {"Medium", 2},
            {"Hight", 3},
            {"VeryHigh", 4},
            {"Ultra", 5}
        };

        [HideInInspector] public InspectorController InspectorController;

        private Camera _maincamera;
        private GameObject _uimessages;
        private bool _vrenvironmenton;
        private int _updatesignal;

       
        private static int undoIndex;
        private GameObject _buttonconnection;
        private GenericButton _buttonconnectiongb;

        #endregion

        #region PublicMethods

        public new void ChangeConnectionMode(bool isconnected)
        {
            
        }

        public void AddHideGroup(string group)
        {
            #if UNITY_EDITOR
            if (!HiddenGroups.Contains(group))
                HiddenGroups.Add(group);
            EditorUtility.SetDirty(this);
            #endif
        }
        
        public void RemoveHideGroup(string group)
        {
#if UNITY_EDITOR
            if (HiddenGroups.Contains(group))
                HiddenGroups.Remove(group);
            EditorUtility.SetDirty(this);
#endif
        }
        
        public bool GroupIsHidden(string group)
        {
            return HiddenGroups.Contains(group);
        }
        
        public void ChangedVisual()
        {
            GameObject[] sunlights = { };
            GameObject[] firstlights = { };
            GameObject[] secondlights = { };

#if !UNITY_POST_PROCESSING_STACK_V2
            EnableHQ = false;
#endif

            if (EnableEnvironmentAndQuality == false)
                return;

            // General
            QualitySettings.SetQualityLevel(QualityLevel, true);


            var bottom = GetChildByNameAlsoHidden("Bottom");
            var mainlight = GetChildByNameAlsoHidden("Main Light");
            var secondlight = GetChildByNameAlsoHidden("SecondLight");

            // Lights with Tag
            try
            {
                sunlights = GameObject.FindGameObjectsWithTag("g4a sun");
                firstlights = GameObject.FindGameObjectsWithTag("g4a firstlight");
                secondlights = GameObject.FindGameObjectsWithTag("g4a secondlight");
            }
            catch (UnityException e)
            {
            }

            foreach (var light in sunlights)
            {
                var thelight = light.GetComponent<Light>();
                if (thelight != null)
                    thelight.intensity = SunLightIntensity;
            }

            foreach (var light in firstlights)
            {
                var thelight = light.GetComponent<Light>();
                if (thelight != null)
                    thelight.intensity = FirstLightIntensity;
            }

            foreach (var light in secondlights)
            {
                var thelight = light.GetComponent<Light>();
                if (thelight != null)
                    thelight.intensity = SecondLightIntensity;
            }

            // Lights with LightGroup
            var lightgroups = GameObject.FindObjectsOfType<LightGroup>();
            foreach (var lightgroup in lightgroups)
            {
                lightgroup.SetIntensity(LightGroupEnum.Sun, SunLightIntensity);
                lightgroup.SetIntensity(LightGroupEnum.FirstLight, FirstLightIntensity);
                lightgroup.SetIntensity(LightGroupEnum.SecondLight, SecondLightIntensity);
            }

            // Environment
            if (StandardBasePlateAndLights && !VREnvironment)
            {
                if (bottom != null)
                {
                    bottom.SetActive(true);
                    bottom.transform.localScale = new Vector3(BasePlateDimensions, -0.01f, BasePlateDimensions);
                }

                if (mainlight != null)
                    mainlight.SetActive(true);
                if (secondlight != null)
                    secondlight.SetActive(true);
            }
            else
            {
                if (bottom != null)
                    bottom.SetActive(false);
                if (mainlight != null)
                    mainlight.SetActive(false);
                if (secondlight != null)
                    secondlight.SetActive(false);
            }

            // find all cameras
            Camera[] cameras = new Camera[Camera.allCamerasCount];
#if (UNITY_POST_PROCESSING_STACK_V2 && UNITY_EDITOR && !UNITY_ANDROID && !UNITY_IOS)
            Bloom bloomlayer = null;
            AmbientOcclusion ambientlayer = null;
            ScreenSpaceReflections reflections = null;

            ColorGrading color = null;
            Camera.GetAllCameras(cameras);
            foreach (var camera in cameras)
            {
                var postvolume = camera.GetComponent<PostProcessVolume>();
                var postlayer = camera.GetComponent<PostProcessLayer>();
                if (StandardBasePlateAndLights)
                    camera.clearFlags = CameraClearFlags.Color;
                if (VREnvironment)
                    camera.clearFlags = CameraClearFlags.Skybox;

                if (EnableHQ == true)
                {
                    camera.renderingPath = RenderingPath.DeferredShading;

                    // Create Layer and Volume 
                    postlayer = camera.GetComponent<PostProcessLayer>();
                    postvolume = camera.GetComponent<PostProcessVolume>();

                    DestroyImmediate(postlayer);
                    DestroyImmediate(postvolume);

                    postlayer = camera.gameObject.AddComponent<PostProcessLayer>();
                    postvolume = camera.gameObject.AddComponent<PostProcessVolume>();
                    postvolume.isGlobal = true;
                    LayerMask volumeLayer = (1 << LayerMask.NameToLayer("g4a Postprocessing"));
                    postlayer.volumeLayer = volumeLayer;
                    postlayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;

                    var assets = AssetDatabase.FindAssets("game4automationVRPostProcessing");
                    string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                    postvolume.profile =
                        (PostProcessProfile) AssetDatabase.LoadAssetAtPath(path, typeof(PostProcessProfile));

                    if (postvolume != null)
                    {
                        postvolume.enabled = Postprocessing;
                        postvolume.profile.TryGetSettings(out bloomlayer);
                        if (Bloom == 0)
                            bloomlayer.enabled.value = false;
                        else
                        {
                            bloomlayer.enabled.value = true;
                            bloomlayer.intensity.value = Bloom;
                        }

                        postvolume.profile.TryGetSettings(out ambientlayer);
                        if (AmbientOcclusion == 0)
                            ambientlayer.enabled.value = false;
                        else
                        {
                            ambientlayer.enabled.value = true;
                            ambientlayer.intensity.value = AmbientOcclusion;
                        }

                        postvolume.profile.TryGetSettings(out reflections);
                        reflections.enabled.value = Reflections;
                        reflections.distanceFade.value = ReflectionDistanceFade;

                        postvolume.profile.TryGetSettings(out color);
                        color.temperature.value = Temperature;
                        color.contrast.value = Contrast;
                        color.gain.value = new Vector4(0, 0, 0, Brightness);
                        color.saturation.value = Saturation;
                    }

                    if (postlayer != null)
                        postlayer.enabled = Postprocessing;
                }
                else
                {
                    camera.renderingPath = RenderingPath.Forward;
                    // Destroy Layer and Volume 
                    postlayer = camera.GetComponent<PostProcessLayer>();
                    postvolume = camera.GetComponent<PostProcessVolume>();

                    if (postlayer != null)
                        DestroyImmediate(postlayer);
                    if (postvolume != null)
                        DestroyImmediate(postvolume);
                }
            }
#endif

#if UNITY_EDITOR
            var vr = GameObject.Find("VREnvironment");
            if (vr != null)
            {
                if (VREnvironment)
                {
                    RenderSettings.ambientIntensity = GlobalLight;
                    RenderSettings.skybox = VRSkybox;
                    RenderSettings.ambientMode = AmbientMode.Skybox;
                    var vrasset = EnvironmentAsset;
                    if (vrasset == null)
                    {
                        EditorUtility.DisplayDialog("Warning",
                            "The VR Environment is only included in Game4Automation Professional", "OK");
                        return;
                    }
                    else
                    {
                        if (vr.transform.childCount == 0)
                        {
                            var env = Instantiate(EnvironmentAsset);
                            env.name = EnvironmentAsset.name;
                            env.transform.parent = vr.transform;
                        }
                    }
                }
                else
                {
                    foreach (Transform child in vr.transform)
                    {
                        DestroyImmediate(child.gameObject);
                    }

                    RenderSettings.ambientMode = AmbientMode.Flat;
                    RenderSettings.skybox = null;
                }
            }
#endif
        }

        public void ChangeUIEnable()
        {
            if (UIEnabledOnStart)
            {
                var info = GetChildByName("Info");
                if (RuntimeAutomationUI != null)
                    RuntimeAutomationUI.SetActive(true);
                if (RuntimeApplicationUI != null)
                    RuntimeApplicationUI.SetActive(true);
                if (HideInfoBox)
                    if (info != null)
                        info.SetActive(false);
            }
            else
            {
                if (RuntimeAutomationUI != null)
                    RuntimeAutomationUI.SetActive(false);
                if (RuntimeApplicationUI != null)
                    RuntimeApplicationUI.SetActive(false);
            }
        }

        public void MessageBox(string message, bool autoclose, float closeafterseconds)
        {
            var uimessage = (GameObject) Instantiate(UnityEngine.Resources.Load<GameObject>("UIMessageBox"));
            uimessage.name = "MessageBox";
            uimessage.transform.localScale = new Vector3(1, 1, 1);
            uimessage.transform.SetParent(_uimessages.transform);
            UIMessageBox messageobj = uimessage.GetComponent<UIMessageBox>();
            messageobj.DisplayMessage(message, autoclose, closeafterseconds);
        }

        public int GetMUID(GameObject caller)
        {
            _lastmuid++;
            return _lastmuid;
        }

#if UNITY_EDITOR

        public void SetVisible(GameObject target, bool isActive)
        {
            Global.SetVisible(target, isActive);
        }

        public void UpdateAllLockedAndHidden()
        {
            foreach (var obj in LockedObjects.ToArray())
            {
                SetLockObject(obj, true);
            }

            foreach (var obj in ObjectsWithHiddenSubobjects.ToArray())
            {
                HideSubObjects(obj, true);
            }
        }

        public void SetLockObject(GameObject target, bool isLocked)
        {
            Global.SetLockObject(target, isLocked);
            Undo.IncrementCurrentGroup();

            if (Selection.objects.Length > 1)
                foreach (var obj in Selection.objects)
                {
                    if (obj.GetType() == typeof(GameObject))
                    {
                        if (obj != target)
                            SetLockObject((GameObject) obj, isLocked);
                    }
                }
        }
        
        public bool AreSubObjectsHidden(GameObject target)
        {
            return ObjectsWithHiddenSubobjects.Contains(target);
        }

        public bool IsHiddenSubObject(GameObject target)
        {
            return HiddenSubobjects.Contains(target);
        }

        public bool IsInObjectWithHiddenSubobjects(GameObject target)
        {
            return InObjectWithHiddenSubobjects.Contains(target);
        }

        public void AlterHideObjects(GameObject target)
        {
            if (ObjectsWithHiddenSubobjects.Contains(target))
                HideSubObjects(target, false);
            else
                HideSubObjects(target, true);
        }

        public void ResetView()
        {
            var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var obj in objs)
            {
                HideSubObjects(obj, false);
                Global.SetExpandedRecursive(obj, false);
                Global.SetLockObject(obj,false);
            }
            
            ObjectsWithHiddenSubobjects = new List<GameObject>();
            LockedObjects = new List<GameObject>();
            HiddenSubobjects = new List<GameObject>();
            InObjectWithHiddenSubobjects = new List<GameObject>();
            
        }
        
        public void SetSimpleView(bool simple, bool expanded)
        {
            var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            
            if (simple)
            {
                foreach (var obj in objs)
                {
                    HideSubObjects(obj, true);
        
                }
            }
            else
            {
                foreach (var obj in ObjectsWithHiddenSubobjects.ToArray())
                {
                    HideSubObjects(obj, false);
                }
            }

            if (!expanded)
            {
                foreach (var obj in objs)
                {
                    if (obj != this)
                        Global.SetExpandedRecursive(obj, false);
                }
            }
        }

        public void HideSubObjects(GameObject target, bool hide)
        {
            Global.HideSubObjects(target, hide);
        }
        
#endif

        public void Quit()
        {
            Application.Quit();
        }

        public void Pause()
        {
            Time.timeScale = 0;
            if (_stepwise)
            {
                BroadcastMessage("SetToggleOn", "Pause");
            }
        }

        public void Play()
        {
            Time.timeScale = TimeScale;
        }

        public void ChangedTimeScale()
        {
            Time.timeScale = TimeScale;
        }

        public void OnConnectionButtonPresed(GenericButton button)
        {
            Connected = button.IsOn;
            var objs = UnityEngine.Resources.FindObjectsOfTypeAll<Game4AutomationBehavior>();
            foreach (var obj in objs)
            {
                obj.ChangeConnectionMode(Connected);
            }
        }

        public void OnConnectionOpened(GameObject Interface)
        {
            if (!ConnectionsActive.Contains(Interface))
                ConnectionsActive.Add(Interface);
            UpdateInterfaceButtonStatus();
        }

        public void OnConnectionClosed(GameObject Interface)
        {
            if (ConnectionsActive.Contains(Interface))
                ConnectionsActive.Remove(Interface);
            UpdateInterfaceButtonStatus();
        }

        private void UpdateInterfaceButtonStatus()
        {
            if (ConnectionsActive.Count > 0)
            {
                if (_buttonconnectiongb != null)
                    _buttonconnectiongb.SetColor(Color.green);
            }
            else
            {
                if (_buttonconnectiongb != null)
                    _buttonconnectiongb.SetColor(Color.white);
            }
        } 
        
        public void OnUIButtonPressed(GameObject Button)
        {
            var buttonname = Button.name;
            var buttonpressed = false;
            if (Button.GetComponent<Toggle>() != null)
            {
                buttonpressed = Button.GetComponent<Toggle>().isOn;
            }

            switch (buttonname)
            {
                case "Play":
                    if (buttonpressed)
                    {
                        Play();
                    }
                    else
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        Pause();
                    }

                    break;
                case "Pause":
                    if (buttonpressed)
                    {
                        Pause();
                    }
                    else
                    {
                        if (_stepwise)
                        {
                            _stepwise = false;
                        }

                        Play();
                    }

                    break;
                case "Step":
                    if (_stepwise == true)
                    {
                        Play();
                    }

                    _stepwise = true;
                    Invoke("Pause", 0.1F);
                    break;
                case "Perspective":
                    if (buttonpressed)
                    {
                        _maincamera.orthographic = true;
                        _maincamera.farClipPlane = 100;
                        _maincamera.nearClipPlane = -100;
                    }
                    else
                    {
                        _maincamera.orthographic = false;
                        _maincamera.nearClipPlane = 0.01f;
                        _maincamera.farClipPlane = 1000;
                    }

                    break;
            }
        }

        public void OnPlayModeFinished()
        {
#if UNITY_EDITOR
            UpdateAllLockedAndHidden();
#endif
        }

        public void ChangeVREnvironment()
        {
            VREnvironment = !VREnvironment;
            SetVREnvironment(VREnvironment);
        }

        public bool GetVREnvironment()
        {
            return VREnvironment;
        }

        public void SetVREnvironment(bool on)
        {
            VREnvironment = on;
            StandardBasePlateAndLights = !on;
            ChangedVisual();
        }

#if UNITY_EDITOR
        public void OnEnable()
        {
            Global.SetG4AController(this);
            if (ObjectsWithHiddenSubobjects == null)
                ObjectsWithHiddenSubobjects = new List<GameObject>();
            if (LockedObjects == null)
                LockedObjects = new List<GameObject>();
            if (HiddenSubobjects == null)
                HiddenSubobjects = new List<GameObject>();
            if (InObjectWithHiddenSubobjects == null)
                InObjectWithHiddenSubobjects = new List<GameObject>();


            if (Application.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                // After End of Play
                QuickToggle.SetGame4Automation(this);
                UpdateAllLockedAndHidden();
            }

            if (Application.isPlaying == true && EditorApplication.isPlayingOrWillChangePlaymode == true)
            {
                // When Play Started
                QuickToggle.SetGame4Automation(this);
                UpdateAllLockedAndHidden();
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene Loaded: " + scene.name);
      
        }

        public void OnDisable()
        {
            QuickToggle.SetGame4Automation(null);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Global.SetG4AController(null);
        }
#endif

        public void OnPausePressed(bool pressed)
        {
            if (pressed)
            {
                Time.timeScale = TimeScale;
            }
            else
            {
                Time.timeScale = 0;
            }
        }

        #endregion

        #region PrivateMethods
        protected  new bool hideactiveonly() { return true; }
        
        
        private void HideGroupsOnStart()
        {
            foreach (var group in HideGroups)
            {
                var elements = GetAllWithGroup(group);

                foreach (var element in elements)
                {
                    element.gameObject.SetActive(false);
                }
            }

            foreach (var group in HiddenGroups)
            {
                Debug.Log("Group hidden " + group);
            }
        
        }
        
        void Start()
        {
            InvokeRepeating("UpdateHierarchy", 1.0F, HierarchyUpdateCycle);
            var obj = GameObject.Find("__MonoContext__");

            Object.DestroyImmediate(obj);
        }

        private void UpdateHierarchy()
        {
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

        private void UpdateSignals()
        {
            /// Clear Info on all Signals
            var signals = FindObjectsOfType<Signal>();
            foreach (var signal in signals)
            {
                signal.DeleteSignalConnectionInfos();
            }

            /// get all Behavior models
            var behaviors = FindObjectsOfType<BehaviorInterface>();
            foreach (var behavior in behaviors)
            {
                // now get all signals in behaviors
                behavior.UpdateConnectionInfo();
                foreach (var info in behavior.ConnectionInfo)
                {
                    if (info.Signal != null)
                    {
                        info.Signal.AddSignalConnectionInfo(behavior, info.Name);
                    }
                }
            }
        }

        void Reset()
        {
            Debug.Log("Reset");
            ObjectsWithHiddenSubobjects = new List<GameObject>();
            LockedObjects = new List<GameObject>();
            HiddenSubobjects = new List<GameObject>();
            InObjectWithHiddenSubobjects = new List<GameObject>();
            HiddenGroups = new List<string>();
        }

        new void Awake()
        {
            _maincamera = GetComponentInChildren<Camera>();
            _uimessages = GetChildByName("MessageBoxes");
            _buttonconnection = GetChildByName("Connected");

            if (_buttonconnection != null)
            {
                _buttonconnectiongb = _buttonconnection.GetComponent<GenericButton>();
                _buttonconnectiongb.SetStatus(Connected);
            }

            Global.g4acontroller = this;
            ChangeUIEnable();

            if (GetChildByName("Inspector") != null)
            {
                InspectorController = GetChildByName("Inspector").GetComponent<InspectorController>();
                InspectorController.gameObject.SetActive(RuntimeInspectorEnabled);
            }

            ChangedVisual();
            HideGroupsOnStart();

            // Call all G4AAwake
            var behaviors = Object.FindObjectsOfType<Game4AutomationBehavior>();
            foreach (var behavior in behaviors)
            {
                behavior.AwakeAlsoDeactivated();
            }
        }

        void Update()
        {
            //   QuickToggle.SetGame4Automation(this);
            if (Application.isPlaying)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    Quit();
                }

                if (Input.GetKeyDown(KeyCode.F12))
                {
                    if (_debugconsole != null)
                    {
                        _debugconsole.SetActive(!_debugconsole.activeSelf);
                    }
                }
            }

            if (!Application.isPlaying)
            {
                _updatesignal++;

                if (_updatesignal > 10)
                {
                    UpdateSignals();
                    _updatesignal = 0;
                }
            }
        }

        static void AddComponent(string assetpath)
        {
#if UNITY_EDITOR
            GameObject component = Selection.activeGameObject;
            Object prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject));
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = new Vector3(0, 0, 0);
            if (component != null)
            {
                go.transform.parent = component.transform;
            }

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
#endif
        }

        #endregion
    }
}