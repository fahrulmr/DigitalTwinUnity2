// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  



using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using game4automationtools;
namespace game4automation
{
    public enum DIRECTION
    {
        LinearX,
        LinearY,
        LinearZ,
        RotationX,
        RotationY,
        RotationZ,
        Virtual
    }

    //! This is the base class for all Game4Automation objects. This base clase is providing some additional scripts and properties for all components.
    public class Game4AutomationBehavior : MonoBehaviour
    {
        [HideIf("hidename")]
        public string Name; //!< The name of the component if it should be different from the GameObject name
        
  
        public enum ActiveOnly { Always,Connected, Disconnected, Never, DontChange }
        [HideIf("hideactiveonly")]
        public  ActiveOnly Active;
        [HideInInspector] public GameObject FromTemplate;
        [HideInInspector]
        public Game4AutomationController Game4AutomationController;
        [HideInInspector][SerializeField] public bool HideNonG44Components;
            
        private static ILogger _logger = Debug.unityLogger;

        protected  bool hidename() { return true; }
        protected  bool hideactiveonly() { return false; }
        
        //! Transfers the direction enumeration to a vector
        public  Vector3 DirectionToVector(DIRECTION direction)
        {
            Vector3 result = Vector3.up;
            switch (direction)
            {
                case DIRECTION.LinearX:
                    result = Vector3.right;
                    break;
                case DIRECTION.LinearY:
                    result = Vector3.up;
                    break;
                case DIRECTION.LinearZ:
                    result = Vector3.forward;
                    break;
                case DIRECTION.RotationX:
                    result = Vector3.right;
                    break;
                case DIRECTION.RotationY:
                    result = Vector3.up;
                    break;
                case DIRECTION.RotationZ:
                    result = Vector3.forward;
                    break;
                case DIRECTION.Virtual:
                    result = Vector3.zero;
                    break;
            }

            return result;
        }
        
        //! Transfers a vector to the direction enumeration
        public  DIRECTION VectorToDirection(bool torotatoin,Vector3 vector)
        {
            if (!torotatoin)
            {
                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.LinearX)) == 1)
                {
                    return DIRECTION.LinearX;
                }

                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.LinearY)) == 1)
                {
                    return DIRECTION.LinearY;
                }

                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.LinearZ)) == 1)
                {
                    return DIRECTION.LinearZ;
                }
            }
            else
            {
                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.RotationX)) == 1)
                {
                    return DIRECTION.RotationX;
                }
                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.RotationY)) == 1)
                {
                    return DIRECTION.RotationY;
                }
                if (Vector3.Dot(vector, DirectionToVector(DIRECTION.RotationZ)) == 1)
                {
                    return DIRECTION.RotationZ;
                }
            }
            // if nothing return virtual
            return DIRECTION.Virtual;
        }

        public  float GetLocalScale(Transform thetransform, DIRECTION direction)
        {
            float result = 1;
            switch (direction)
            {
                case DIRECTION.LinearX:
                    result = thetransform.lossyScale.x;
                    break;
                case DIRECTION.LinearY:
                    result = thetransform.lossyScale.y;
                    break;
                case DIRECTION.LinearZ:
                    result = thetransform.lossyScale.z;
                    break;
            }

            return result;
        }

        //! Gets back if the direction is linear or a rotation
        public static bool DirectionIsLinear(DIRECTION direction)
        {
            bool result = false;
            switch (direction)
            {
                case DIRECTION.LinearX:
                    result = true;
                    break;
                case DIRECTION.LinearY:
                    result = true;
                    break;
                case DIRECTION.LinearZ:
                    result = true;
                    break;
                case DIRECTION.RotationX:
                    result = false;
                    break;
                case DIRECTION.RotationY:
                    result = false;
                    break;
                case DIRECTION.RotationZ:
                    result = false;
                    break;
                case DIRECTION.Virtual:
                    result = true;
                    break;
            }

            return result;
        }


        //! Sets the visibility of this object including all subobjects 
        public void SetVisibility(bool visibility)
        {
            Renderer[] components = gameObject.gameObject.GetComponentsInChildren<Renderer>();
            if (components != null)
            {
                foreach (Renderer component in components)
                    component.enabled = visibility;
            }

            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers != null)
            {
                foreach (MeshRenderer meshrenderer in meshRenderers)
                    meshrenderer.enabled = visibility;
            }
        }

        //! Gets a child by name 
        public GameObject GetChildByName(string name)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        
        //! Gets all child by name 
        public List<GameObject> GetChildsByName(string name)
        {
            List<GameObject> childs = new List<GameObject>();
            Transform[] children = transform.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == name)
                {
                   childs.Add(child.gameObject);
                }
            }

            return childs;
        }
        

        public GameObject GetChildByNameAlsoHidden(string name)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
        
        public List<GameObject> GetAllMeshesWithGroup(string group)
        {
            List<GameObject> list = new List<GameObject>();
            var groupcomps = Object.FindObjectsOfType<Group>();
            foreach (var groupcomp in groupcomps)
            {
                if (groupcomp.GetGroupName() == group)
                {
                    // Check if one parent has the same group
                    var mesh = groupcomp.gameObject.GetComponent<MeshFilter>();
                 
                    if (!ReferenceEquals(mesh, null))
                    {
                        list.Add(groupcomp.gameObject);
                    }
                }
            }
            return list;
        }

        public List<GameObject> GetAllWithGroup(string group)
        {
            List<GameObject> list = new List<GameObject>();
            var groupcomps = Object.FindObjectsOfType<Group>();
            foreach (var groupcomp in groupcomps)
            {
                if (groupcomp.GetGroupName() == group)
                {
                    // Check if one parent has the same group
                    var parent = groupcomp.transform.parent;
                    bool add = true;
                    if (!ReferenceEquals(parent, null))
                    {
                        // search upwards
                        var uppergroups = parent.gameObject.GetComponentsInParent<Group>();
                        // is the group in one of the upper parents?
                        foreach (var uppergroup in uppergroups)
                        {
                            if (uppergroup.GetGroupName() == group)
                            {
                                add = false;
                            }
                        }
                    }

                    if (add)
                        list.Add(groupcomp.gameObject);
                }
            }
            return list;
        }

        public List<GameObject> GetAllWithGroups(List<string> groups)
        {
            List<GameObject> first;
            first = GetAllWithGroup(groups[0]);

            for (int i = 1; i < groups.Count; i++)
            {
                var newobjs = GetAllWithGroup(groups[i]);
                IEnumerable<GameObject> res = first.AsQueryable().Intersect(newobjs);
                first = res.ToList();
            }

            return first;
        }
        
        public List<GameObject> GetAllMeshesWithGroups(List<string> groups)
        {
            List<GameObject> first;
            first = GetAllMeshesWithGroup(groups[0]);

            for (int i = 1; i < groups.Count; i++)
            {
                var newobjs = GetAllWithGroup(groups[i]);
                IEnumerable<GameObject> res = first.AsQueryable().Intersect(newobjs);
                first = res.ToList();
            }
            return first;
        }
        
        public List<string> GetMyGroups()
        {
            List<string> list = new List<string>();
            var groups = GetComponents<Group>();
            foreach (var group in groups)
            {
                list.Add(group.GroupName);
            }

            return list;
        }
        
        public List<GameObject> GetMeshesWithSameGroups()
        {
            var list = GetMyGroups();
            var list2 = GetAllMeshesWithGroups(list);
            list2.Remove(this.gameObject);
            return list2;
        }


        public List<GameObject> GetAllWithSameGroups()
        {
            var list = GetMyGroups();
            var list2 = GetAllWithGroups(list);
            list2.Remove(this.gameObject);
            return list2;
        }

        //! Gets the top of an MU component (the first MU script going up in the hierarchy)
        protected MU GetTopOfMu(GameObject obj)
        {
            return obj.GetComponentInParent<MU>();
        }

        //!     Gets the mesh renderers in the childrens
        public MeshRenderer GetMeshRenderer()
        {
            MeshRenderer renderers = gameObject.GetComponentInChildren<MeshRenderer>();
            return renderers;
        }

        //! sets the collider in all child objects
        public void SetCollider(bool enabled)
        {
            Collider[] components = gameObject.GetComponentsInChildren<Collider>();
            if (components != null)
            {
                foreach (Collider component in components)
                    component.enabled = enabled;
            }
        }

        //! Displays an error message
        public void ErrorMessage(string message)
        {
#if (UNITY_EDITOR)
            EditorUtility.DisplayDialog("Game4Automation Error for [" + this.gameObject.name + "]", message, "OK", "");
#endif
            Error(message, this);
        }

        public void ChangeConnectionMode(bool isconnected)
        {
            if (Active == ActiveOnly.DontChange)
                return;
            
            if (Active == ActiveOnly.Always)
            {
                if (this.enabled == false)
                    this.enabled = true;
            }
            
            if (Active == ActiveOnly.Connected)
            {
                if (isconnected)
                    this.enabled = true;
                else
                    this.enabled = false;
            }

            if (Active == ActiveOnly.Disconnected)
            {
                if (!isconnected)
                    this.enabled = true;
                else
                    this.enabled = false;
            }
            
            if (Active == ActiveOnly.Never)
            {
                this.enabled = false;
            }
        }

        //! Logs a message
        public void Log(string message)
        {
            _logger.Log("game4automation: " + message);
        }

        //! Logs a message with a relation to an object
        public void Log(string message, object obj)
        {
            _logger.Log("game4automation: Object [" + this.gameObject.name + "] " + message, obj);
        }

        //! Logs a warinng with a relation to an object
        public void Warning(string message, object obj)
        {
            _logger.LogWarning("game4automation: Object [" + this.gameObject.name + "] " + message, obj);
        }

        //! Logs an error with a relation to an object
        public void Error(string message, object obj)
        {
            _logger.LogError("game4automation: Object [" + this.gameObject.name + "] " + message, obj);
        }

        //! Logs an error
        public void Error(string message)
        {
            _logger.LogError("game4automation: " + message, this);
        }

        //! Displays a gizmo for debugging positions
        public GameObject DebugPosition(string debugname, Vector3 position, Quaternion quaternation, float scale)
        {
            GameObject debuggizmo = null;

            if (Game4AutomationController.EnablePositionDebug)
            {
                debuggizmo = GameObject.Find("debugname");
                if (debuggizmo == null)
                {
                    var gizmo = UnityEngine.Resources.Load("Gizmo/Gizmo", typeof(GameObject));
                    debuggizmo = Instantiate((GameObject) gizmo, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    debuggizmo.layer = Game4AutomationController.DebugLayer;
                }

                debuggizmo.transform.position = position;
                debuggizmo.transform.rotation = quaternation;
                debuggizmo.transform.localScale = Vector3.one * scale;
                debuggizmo.name = debugname;
            }

            return debuggizmo;
        }

        //! Freezes all child components to the current poosition
        public void SetFreezePosition(bool enabled)
        {
            Rigidbody[] components = gameObject.GetComponentsInChildren<Rigidbody>();
            if (components != null)
            {
                foreach (Rigidbody rigid in components)
                    if (enabled)
                    {
                        rigid.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else
                    {
                        rigid.constraints = RigidbodyConstraints.None;
                    }
            }
        }

        //! Initialiates the components and gets the reference to the Game4AutomationController in the scene
        protected void InitGame4Automation()
        {
            Game4AutomationController = UnityEngine.Object.FindObjectOfType<Game4AutomationController>();
            if (Game4AutomationController == null)
            {
                Error(
                    "No Game4AutomationController found - Game4AutomationController Script needs to be once inside every Game4Automation Scene");
                Debug.Break();
            }

            if (Name == "")
            {
                Name = gameObject.name;
            }
            
            ChangeConnectionMode(Game4AutomationController.Connected);
            
            
        }
        
        protected virtual void AfterAwake()
        {
            
        }



        public virtual void AwakeAlsoDeactivated()
        {
            
        }
        

        protected void Awake()
        {
 
            if (Application.isPlaying)
                InitGame4Automation();
            AfterAwake();
        }
    }
}