// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz    

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if GAME4AUTOMATION_INTERACT
using XdeEngine.Core;
using XdeEngine.Core.Monitoring;
using xde_types.core;


namespace game4automation
{

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class Game4AutomationPhysics
    {
        public static void Kinematize(GameObject obj)
        {
#if UNITY_EDITOR
            
            XdeUnitJoint joint;
        

            Drive drive = obj.GetComponent<Drive>();
            drive.UseInteract = true;
            // Check if Phyiscs Manager is available - if not create one
            if (!UnityEngine.Object.FindObjectOfType<XdeScene>())
            {
                var com = Global.AddComponentTo(null,"Assets/game4automation/private/[PhysicsManager].prefab");
                EditorUtility.DisplayDialog("Game4Automation Physics manager added",
                    "For Interact one Physics Manager needs to be in the scene. Physics Manager was automatically added!",
                    "OK");
            }

            var xderb = obj.GetComponent<XdeRigidBody>();
            if (xderb == null)
                xderb = obj.AddComponent<XdeRigidBody>();
            xderb.mass = 1;
            xderb.WeightEnabled = true;
            // Rotation
            if (drive.Direction == DIRECTION.RotationX || drive.Direction ==
                DIRECTION.RotationY || drive.Direction == DIRECTION.RotationZ)
            {
                joint = obj.GetComponent<XdeHingeJoint>();
                if (joint==null)
                      joint = obj.AddComponent<XdeHingeJoint>();
                // Delete other joint if available
                var joint2 = obj.GetComponent<XdePrismaticJoint>();
                if (joint2!=null)
                    EditorApplication.delayCall += () =>Object.DestroyImmediate(joint2);
                XdeHingeJoint xdehhj = (XdeHingeJoint) joint;
                xdehhj.axis = drive.GetLocalDirection();
            }
            else
                // Linear
            {
                joint = obj.GetComponent<XdePrismaticJoint>();
                if (joint==null)
                    joint = obj.AddComponent<XdePrismaticJoint>();
                // Delete other joint if available
                var joint2 = obj.GetComponent<XdeHingeJoint>();
                if (joint2!=null)
                    EditorApplication.delayCall += () =>Object.DestroyImmediate(joint2);
                XdePrismaticJoint xdepris = (XdePrismaticJoint) joint;
                xdepris.axis = drive.GetLocalDirection();

            }


            var monitor = obj.GetComponent<XdeUnitJointMonitor>();
            if (monitor == null)
                monitor = obj.AddComponent<XdeUnitJointMonitor>();
            monitor.targetJoint = joint;
            monitor.synchronizeJointState = true;
            
            var controller = obj.GetComponent<XdeUnitJointPDController>();
            if (controller == null)
                controller = obj.AddComponent<XdeUnitJointPDController>();
            controller.mode = coupling_mode.POSITION_VELOCITY;
            controller.targetJoint = joint;
            controller.proportionalGain = 100000;
            controller.derivedGain = 0;
            controller.enableOnStart = true;
            
          
            
        }

   
        public static void UnKinematize(GameObject obj)
        {
            var drive = obj.GetComponent<Drive>();
            drive.UseInteract = false;
            drive.jointcontroller = null;
            drive.jointmonitor = null;
            
                     
            var controller = obj.GetComponent<XdeUnitJointPDController>();
            if (controller != null)
                EditorApplication.delayCall += () =>Object.DestroyImmediate(controller);

            var monitor = obj.GetComponent<XdeUnitJointMonitor>();
            if (monitor != null)
                EditorApplication.delayCall += () =>Object.DestroyImmediate(monitor);

            var joint = obj.GetComponent<XdeUnitJoint>();
            if (joint != null)
                EditorApplication.delayCall += () =>Object.DestroyImmediate(joint);
            
            var xderb = obj.GetComponent<XdeRigidBody>();
            if (xderb != null)
                EditorApplication.delayCall += () =>Object.DestroyImmediate(xderb);

        }
    #endif

        public static void InitDrive(Drive drive)
        {
            // Check if allready Interact component there
            var hingejoint = drive.gameObject.GetComponent<XdeHingeJoint>();
            var prismaticjoint = drive.gameObject.GetComponent<XdePrismaticJoint>();
            if (hingejoint == null && prismaticjoint == null) // Do nothing
                return;
            
            // Interact is there also use it
            drive.UseInteract = true;
            
            // Rotation
            if (hingejoint != null)
            {
                drive.Direction = drive.VectorToDirection(true, hingejoint.axis);
                if (hingejoint.useMax == true && hingejoint.useMin == true)
                {
                    drive.UseLimits = true;
                    drive.LowerLimit = hingejoint.minValue;
                    drive.UpperLimit = hingejoint.maxValue;
                }
            }
            // Linear
            else
                drive.Direction = drive.VectorToDirection(false, hingejoint.axis);
            drive.CalculateVectors();
            Kinematize(drive.gameObject);
        }
        
        public static void EnableDrive(Drive drive, bool enable)
        {
            var jointcontroller = drive.gameObject.GetComponent<XdeUnitJointPDController>();
            if (jointcontroller != null)
                jointcontroller.enableOnStart = enable;
        }
        
        public static void SetPosition(Drive drive, float position)
        {
            float currpos = 0;
            if (drive.IsRotation)
                currpos = drive.CurrentPosition;
            else
                currpos = drive.CurrentPosition / drive.Game4AutomationController.Scale;
  
            var deltatophyisicspos = currpos - drive.jointmonitor.currentPosition;
            var speed = deltatophyisicspos / Time.deltaTime;
            drive.jointcontroller.Write(currpos,speed);

        }
    }
}
#endif
