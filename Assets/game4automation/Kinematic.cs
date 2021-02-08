// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using System.Collections.Generic;
using game4automationtools;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace game4automation
{
    [ExecuteAlways]
    //! Kinematic helper for changing the kinematic during simulation starts
    //! This helps to use existing non changed CAD date in Game4Automation
    [HelpURL("https://game4automation.com/documentation/current/kinematic.html")]
    public class Kinematic : Game4AutomationBehavior
    {
        [BoxGroup("Reposition (including children)")] [Label("Enable")] 	[OnValueChanged("UpdateValues")]
        public bool RepositionEnable = false; //!<  Enables Repositioning
        [BoxGroup("Reposition (including children)")] [ShowIf("RepositionEnable")] 	[OnValueChanged("UpdateValues")]
        public bool UpdateInEditorMode; //!<  Also Update in Editor Mode
        [BoxGroup("Reposition (including children)")] [ShowIf("RepositionEnable")] 	[OnValueChanged("UpdateValues")]
        public GameObject MoveTo; //!<  Reposition and Move the Pivot of this object to the defined Pivot
    
        [BoxGroup("Reposition (including children)")] [ShowIf("RepositionEnable")] 	[OnValueChanged("UpdateValues")]
        public Vector3 AdditionalRotation; //!<  Gives an additional rotation when repositioning
        
        [BoxGroup("Move Center (keep children)")] [Label("Enable")]
        public bool MoveCenterEnable = false; //!<  Enables to move the Pivot Point without moving the part itself

        [BoxGroup("Move Center (keep children)")] [ShowIf("MoveCenterEnable")]
        public Vector3 DeltaPosOrigin; //!<  Vector to move the Pivot Point in x,y,z

        [BoxGroup("Move Center (keep children)")] [ShowIf("MoveCenterEnable")]
        public Vector3 DeltaRotOrigin; //!<  Rotation to move the Pivot Point

        [BoxGroup("Integrate Group")] [Label("Enable")]
        public bool IntegrateGroupEnable = false;  //!<  Integrate a Group as children of this component

        [BoxGroup("Integrate Group")] [ShowIf("IntegrateGroupEnable")]
        public string GroupName = "";  //!<  The name of the group to integrate
        
        [BoxGroup("Integrate Group")] [ShowIf("IntegrateGroupEnable")]
        public GameObject GroupNamePrefix; //!<  Optional reference to a Part which name is defining a Prefix for the Groupname. Needs to be used with Prefabs which are using Group function

        [BoxGroup("Integrate Group")] [ShowIf("IntegrateGroupEnable")]
        public Boolean SimplifyHierarchy; //!< Simplify the Hierarchy for the integrated parts

        [BoxGroup("New Kinematic Parent")] [Label("Enable")]
        public bool KinematicParentEnable = false;   //!< Defines a new kinematic parent for this component (moves it and all children during simulation start to a new parent)

        [BoxGroup("New Kinematic Parent")] [ShowIf("KinematicParentEnable")]
        public GameObject Parent; //!< The new kinematic parent

        // The information text in the hierarchy view
        public string GetVisuText()
        {
            var text = "";
            if (IntegrateGroupEnable)
                text = text + "<" + GroupName;

            if (Parent == null)
                return text;

            if (KinematicParentEnable)
                if (text != "")
                    text = text + " ";
                else
                    text = text + "^" + Parent.name;

            return text;
        }
        
        public void UpdateValues()
        {
            if (UpdateInEditorMode)
                   MoveAndRotate(true);
        }
        public void MoveAndRotate()
        {
            MoveAndRotate(true);
        }

        private void MoveAndRotate(bool silent)
        {


            if (MoveTo != null)
            {
                var newrot =  MoveTo.transform.rotation * Quaternion.Euler(AdditionalRotation);
                if ((transform.position != MoveTo.transform.position) || (transform.rotation != newrot))
                {
                    bool ok = true;
                    if (!silent)
                    {
#if UNITY_EDITOR
                        ok = EditorUtility.DisplayDialog("Warning", "Repositioning Object " + name + "because " + MoveTo.name +" changed position", "OK", "CANCEL");
#endif
                    }

                    if (ok)
                    {
                        transform.position = MoveTo.transform.position;
                        transform.rotation = MoveTo.transform.rotation * Quaternion.Euler(AdditionalRotation);
                    }
                } 
            }
        }

        new void Awake()
        {
            if (Application.IsPlaying(gameObject))
            {
                List<GameObject> objs;
                if (IntegrateGroupEnable)
                {
                    var groupname = GroupName;
                    if (GroupNamePrefix != null)
                        groupname = GroupNamePrefix.name + GroupName;
                    if (!SimplifyHierarchy)
                        objs = GetAllWithGroup(groupname);
                    else
                        objs = GetAllMeshesWithGroup(groupname);

                    foreach (var obj in objs)
                    {
                        obj.transform.parent = transform;
                    }
                }

                if (KinematicParentEnable)
                {
                    gameObject.transform.parent = Parent.transform;
                }

                if (RepositionEnable)
                {
                    if (MoveTo != null)
                    {
                        MoveAndRotate(true);
                    }
                }
                
                if (MoveCenterEnable)
                {
                    var deltapos = new Vector3(DeltaPosOrigin.x / Global.g4acontroller.Scale / transform.lossyScale.x,
                        DeltaPosOrigin.y / Global.g4acontroller.Scale / transform.lossyScale.y,
                        DeltaPosOrigin.z / Global.g4acontroller.Scale / transform.lossyScale.z);

                    Global.MovePositionKeepChildren(gameObject, deltapos);
                    Global.MoveRotationKeepChildren(gameObject, Quaternion.Euler(DeltaRotOrigin));
                }
            }
            else
            {
                if (UpdateInEditorMode)
                 MoveAndRotate(false);
            }
        }
    }
}
