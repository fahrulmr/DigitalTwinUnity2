﻿﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using UnityEditor;
using UnityEngine;
  #if GAME4AUTOMATION_INTERACT
using XdeEngine.Core;
#endif


  namespace game4automation
{
    [CustomEditor(typeof(Drive))]
    //! Class for displaying the drive handle in Unity Editor in Scene View during edit and play mode
    //! With the drive handle the drive directions can be defined and the drive can moved manually during playmode
    public class DriveHandle : game4automationtools.Editor.InspectorEditor
    {
        private static float sizecones = 0.5f;
        private static float sizecubes = 0.3f;
        private static float distancecenter = 0.2f;
        private static float transparency = 0.8f;
        private static float sizearc = 1.5f;
        private static float fontsize = 25;
        private static Color colordir = new Color(1, 0, 1, transparency);
        private static Color colorinactive = new Color(0.3f, 0.3f, 0.3f, transparency);
        private static Color colorrunning = new Color(0, 1, 0, transparency);
        private static Color colorrunningstopped = new Color(1, 1, 0, transparency);
        private static Color colorcirclelimits = new Color(1, 1, 1, 0.3f);
        private static Color labelcolor = Color.yellow;
        
        private float distanceclick = 0.2f;
   
        private int idactive, idnonactive1, idnonactive2, idrevert, idposmin;
        private Drive drive;
        private Kinematic _kinematic;
        private float size;
        private Vector3 posactive, posinactive1, posinactive2, posrevert, posmin;
        private DIRECTION dirnotused1, dirnotused2;
        private bool _istranslation;
        private float _globalscale = 1000;
        #if GAME4AUTOMATION_INTERACT
        private XdeHingeJoint _xdehingejoint;
        #endif
        private bool _isinit = false;
        private Game4AutomationController _settings;
        private float _scalehandle = 1;
        private float _offset;
        private bool _isttransportsurface=false;
        private GUIStyle guiStyle = new GUIStyle();

        protected virtual void OnSceneGUI()
        {
         
            if (!_isinit)
            {
                drive = (Drive) target;
                idactive = GUIUtility.GetControlID(FocusType.Passive);
                idnonactive1 = GUIUtility.GetControlID(FocusType.Passive);
                idnonactive2 = GUIUtility.GetControlID(FocusType.Passive);
                idrevert = GUIUtility.GetControlID(FocusType.Passive);
                idposmin = GUIUtility.GetControlID(FocusType.Passive);
#if GAME4AUTOMATION_INTERACT
                _xdehingejoint = drive.GetComponent<XdeHingeJoint>();
#endif

                _kinematic = drive.GetComponent<Kinematic>();
                _isinit = true;
                _settings = UnityEngine.Object.FindObjectOfType<Game4AutomationController>();
                if (_settings != null)
                {
                    _globalscale = _settings.Scale;
                    _scalehandle = _settings.ScaleHandles;
                }

                if (!ReferenceEquals(drive.TransportSurfaces, null))
                {
                    if (drive.TransportSurfaces.Count > 0)
                    {
                        foreach (var drive in drive.TransportSurfaces)
                        {
                            if (!ReferenceEquals(drive, null))
                                EditorGUIUtility.PingObject(drive.gameObject);
                        }
                    }
                }

            }

            if (drive.Direction == DIRECTION.Virtual)
                return;

            // Switching Directions with Keys - only Editor mode
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space && !Application.isPlaying)
            {
                drive.ReverseDirection = !drive.ReverseDirection;
            }
            
          
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Tab && !Application.isPlaying)
            {
                if ((int)drive.Direction < Enum.GetNames(typeof(DIRECTION)).Length-2)
                {
                    drive.Direction = drive.Direction + 1;
                }
                else
                {
                    drive.Direction = (DIRECTION)0;
                }
              
            }

            // Jogging with Keys - only Runmode           
            if (Event.current.type == EventType.KeyDown && Application.isPlaying)
            {
                if (Event.current.keyCode == KeyCode.Alpha3 || Event.current.keyCode == KeyCode.Keypad3)
                {
                    drive.JogForward = true;
                    drive.JogBackward = false;
                }
                
                if ( Event.current.keyCode == KeyCode.Alpha1  || Event.current.keyCode == KeyCode.Keypad1)
                {
        
                    drive.JogForward = false;
                    drive.JogBackward = true;
                }
            }           
            if (Event.current.type == EventType.KeyUp && Application.isPlaying)
            {
                if (Event.current.keyCode == KeyCode.Alpha3 || Event.current.keyCode == KeyCode.Alpha1  || Event.current.keyCode == KeyCode.Keypad1 || Event.current.keyCode == KeyCode.Keypad3)
                {
                    drive.JogForward = false;
                    drive.JogBackward = false;
                }
            }
     


            if (drive.Direction == DIRECTION.LinearX || drive.Direction == DIRECTION.LinearY ||
                drive.Direction == DIRECTION.LinearZ)
                _istranslation = true;
            else
                _istranslation = false;

            switch (Event.current.GetTypeForControl(idactive))
            {
                case EventType.Layout:
                    HandleUtility.AddControl(idactive, HandleUtility.DistanceToCircle(posactive, size * distanceclick));
                    break;
            }

            switch (Event.current.GetTypeForControl(idnonactive1))
            {
                case EventType.Layout:
                    HandleUtility.AddControl(idnonactive1,
                        HandleUtility.DistanceToCircle(posinactive1, size * distanceclick));
                    break;
            }

            switch (Event.current.GetTypeForControl(idnonactive2))
            {
                case EventType.Layout:
                    HandleUtility.AddControl(idnonactive2,
                        HandleUtility.DistanceToCircle(posinactive2, size * distanceclick));
                    break;
            }

            switch (Event.current.GetTypeForControl(idrevert))
            {
                case EventType.Layout:
                    HandleUtility.AddControl(idrevert, HandleUtility.DistanceToCircle(posrevert, size * distanceclick));
                    break;
            }

            switch (Event.current.GetTypeForControl(idposmin))
            {
                case EventType.Layout:
                    HandleUtility.AddControl(idrevert, HandleUtility.DistanceToCircle(posmin, size * distanceclick));
                    break;
            }

          
            
            if (Event.current.type == EventType.Repaint)
            {
                Transform transform = ((Drive) target).transform;
            
                var driveposition = transform.position;

                var dirdrive = DirectionToVector(drive,true);
                var dirdrivelocal = DirectionToVector(drive, false);
                var direction = dirdrive * distancecenter;
                var directionmin = new Vector3(0, 0, 0);
                var rotationquat = Quaternion.LookRotation(dirdrive);
                var rotationquatrevert = Quaternion.LookRotation(dirdrive * -1);
                size = HandleUtility.GetHandleSize(posactive) * _scalehandle;

                _isttransportsurface = false;
                // Set Position to transport Surface if Drive is controlling Transport surface
                if (!ReferenceEquals(drive.TransportSurfaces, null))
                {
                    if (drive.TransportSurfaces.Count > 0)
                    {
                        // get first transportsurface
                        var trans = drive.TransportSurfaces[0];
                        if (!ReferenceEquals(trans, null))
                            driveposition = trans.GetMiddleTopPoint();
                        // get middle top position of transportsurface
                        _isttransportsurface = true;

                        if (trans.Radial)
                        {
                            driveposition = trans.gameObject.transform.position;
                        }
                    }
                }
             

                if (Application.isPlaying)
                    _offset = 0;
                else
                    _offset = drive.Offset;
                

                if (_istranslation)
                {
                    /// Linear Movement /////////////////////////////////////
                    ///
                   
                    if (drive.UseLimits)
                    {
                        if (!_isttransportsurface)
                        {
                            direction = dirdrive * (((drive.UpperLimit + _offset - drive.CurrentPosition) / 1000) +
                                                    size * sizecones * 0.5f);
                            directionmin = -dirdrive * (((drive.LowerLimit + _offset - drive.CurrentPosition) / 1000) -
                                                        size * sizecones * 0.5f);
                        }
                        else
                        {
                            direction = dirdrive * (((drive.UpperLimit ) / 1000) +
                                                    size * sizecones * 0.5f);
                            directionmin = -dirdrive * (((drive.LowerLimit ) / 1000) -
                                                        size * sizecones * 0.5f);
                        }

                        if (!Application.isPlaying)
                            if (!_isttransportsurface)
                                  directionmin = -dirdrive * (((drive.LowerLimit + _offset - drive.CurrentPosition) / 1000) -
                                                        size * sizecubes * 0.5f);
                            else
                                directionmin = -dirdrive * (((drive.LowerLimit) / 1000) -
                                                            size * sizecubes * 0.5f);
                    }


                    if (drive.ReverseDirection)
                    {
                        direction = direction * -1;
                        directionmin = directionmin * -1;
                        rotationquat = Quaternion.LookRotation(dirdrive * -1);
                        rotationquatrevert = Quaternion.LookRotation(dirdrive);
                    }

                    posactive = driveposition + direction;
                    posrevert = driveposition - direction;
                    posmin = driveposition - directionmin;


                    // Draw Direction Handle
                    Handles.color = colordir;
                    if (Application.isPlaying)
                    {
                        Handles.color = colorrunningstopped;
                        if (drive.CurrentSpeed > 0)
                            Handles.color = colorrunning;
                    }

                    Handles.ConeHandleCap(idactive, posactive,  rotationquat, size * sizecones,
                        EventType.Repaint);

                    // Limit Handles
                    if (drive.UseLimits)
                    {
                        if (!Application.isPlaying)
                            Handles.CubeHandleCap(idactive, posmin, rotationquatrevert,
                                size * sizecubes, EventType.Repaint);
                        else
                        {
                            if (drive.CurrentSpeed < 0)
                                Handles.color = colorrunning;
                            else
                                Handles.color = colorrunningstopped;
                            Handles.ConeHandleCap(idrevert, posmin,  rotationquatrevert,
                                size * sizecones,
                                EventType.Repaint);
                        }

                        Handles.DrawLine(posmin, posactive);
                    }

                    if (Application.isPlaying)
                    {
                        Handles.color = Color.white;
                       
                        /// Label for position
                        guiStyle.fontSize = (int)(fontsize * _scalehandle);
                        guiStyle.fontStyle = FontStyle.Bold;
                        guiStyle.normal.textColor = labelcolor;
                       
                        Handles.BeginGUI();
                        Vector3 pos = driveposition;
                        Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
                        GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), drive.CurrentPosition.ToString("0.0")+"mm", guiStyle);
                        Handles.EndGUI();
                      
                       // Handles.Label(driveposition, drive.CurrentPosition.ToString(),guiStyle);
                        
                        
                        Handles.color = colorrunningstopped;
                        if (drive.CurrentSpeed < 0)
                            Handles.color = colorrunning;
                        if (!drive.UseLimits)
                            Handles.ConeHandleCap(idrevert, posrevert,  rotationquatrevert,
                                size * sizecones,
                                EventType.Repaint);
                    }

                    // Draw Handles in the Directions not used
                    if (!Application.isPlaying)
                    {
                        var directionnotused1 = DirectionNotUsed1(drive,false);
                        var dir1 = transform.rotation*directionnotused1 * distancecenter;
                        posinactive1 = driveposition + dir1;
                        var rotationquat1 = Quaternion.LookRotation(dir1);

                        var directionnotused2 = DirectionNotUsed2(drive,false);
                        var dir2 = transform.rotation*directionnotused2 * distancecenter;
                        posinactive2 = driveposition + dir2;
                        var rotationquat2 = Quaternion.LookRotation(dir2);

                        if (drive.ReverseDirection)
                        {
                            posinactive1 = driveposition - dir1;
                            rotationquat1 = Quaternion.LookRotation(dir1 * -1);
                            posinactive2 = driveposition - dir2;
                            rotationquat2 = Quaternion.LookRotation(dir2 * -1);
                        }


                        Handles.color = colorinactive;
                        Handles.ConeHandleCap(idnonactive1, posinactive1, rotationquat1,
                            size * sizecubes,
                            EventType.Repaint);


                        Handles.color = colorinactive;
                        Handles.ConeHandleCap(idnonactive2, posinactive2,  rotationquat2,
                            size * sizecubes,
                            EventType.Repaint);

                        Handles.color = colordir;
                        Handles.DrawLine(driveposition, posactive);
                        
                        Handles.color = colorinactive;
                        Handles.DrawLine(driveposition, posinactive1);
                        Handles.DrawLine(driveposition, posinactive2);
                    }
                }
                else
                {
                    /// Rotation
                    if (drive.UseInteract) // for interact check delta position of center
                    {
                        #if GAME4AUTOMATION_INTERACT
                        if (!ReferenceEquals(_xdehingejoint, null))
                        {
                            var center = _xdehingejoint.center;
                            var centerpos = new Vector3(_xdehingejoint.center.x/Global.g4acontroller.Scale,_xdehingejoint.center.y/Global.g4acontroller.Scale,_xdehingejoint.center.z/Global.g4acontroller.Scale);
                            driveposition = transform.position + drive.gameObject.transform.TransformDirection(centerpos);
                        }
                    #endif
                    }
                    
                    if (!ReferenceEquals(_kinematic, null))
                    {
                        if (_kinematic.MoveCenterEnable && !Application.isPlaying)
                        {
                            var center = _kinematic.DeltaPosOrigin;
                            var scale = transform.lossyScale;
                            var centerpos = new Vector3(center.x/Global.g4acontroller.Scale,center.y/Global.g4acontroller.Scale,center.z/Global.g4acontroller.Scale);
                            driveposition = transform.position +
                                            drive.gameObject.transform.TransformDirection(centerpos);
                        }
                    }
                    
                    var dirarc = DirectionNotUsed1(drive, true);
                    var dir1 = dirarc;
                    var dirrot = DirectionNotUsed2(drive, true);

                    var radius = size * sizearc;
                    var startpos = driveposition;
                    var drivreverse = 1;
                    if (drive.ReverseDirection)
                    {
                        drivreverse = -1;
                    }

                    var dirrotquat = Quaternion.LookRotation(dirrot * drivreverse);
                    var dirrotrevert = Quaternion.LookRotation(-dirrot * drivreverse);

                    posactive = startpos + (dirarc) * size * sizearc + (drivreverse * dirrot * radius * 0.5f);
                    posrevert = startpos + (dirarc) * size * sizearc + (-drivreverse * dirrot * radius * 0.5f);
                    var poszeroline = startpos + (dirarc) * size * sizearc;
                    var poslable = startpos + (dirarc) * size * sizearc;
                    var angle = 360f;
                    if (drive.UseLimits)
                    {
                        angle = drive.UpperLimit - drive.LowerLimit;
                        Vector3 rotatedVector =
                            Quaternion.AngleAxis(drive.LowerLimit + _offset - drive.CurrentPosition, drivreverse * dirdrive) *
                            dirarc;
                        dirarc = rotatedVector;
                    }

                    // Lines for 0 and Axis Direction
                    Handles.color = colordir;
                    Handles.DrawLine(driveposition,startpos+drivreverse*dirdrive*size*sizearc);
                    Handles.DrawLine(driveposition, poszeroline);

                    Handles.color = colorcirclelimits;
                    Handles.DrawSolidArc(driveposition, drivreverse * dirdrive, dirarc, angle, size * sizearc);

                    Handles.color = colordir;
                    if (Application.isPlaying)
                    {
                        if (drive.CurrentSpeed > 0)
                            Handles.color = colorrunning;
                        else
                            Handles.color = colorrunningstopped;
                    }

                    Handles.ConeHandleCap(idactive, posactive, dirrotquat, size * sizecones,
                        EventType.Repaint);
                    if (Application.isPlaying)
                    {

                        /// Label for position
                        guiStyle.fontSize = (int)(fontsize * _scalehandle);
                        guiStyle.fontStyle = FontStyle.Bold;
                        guiStyle.normal.textColor = labelcolor;
                       
                        Handles.BeginGUI();
                        Vector3 pos = poslable;
                        Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
                        GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), drive.CurrentPosition.ToString("0.0")+"°", guiStyle);
                        Handles.EndGUI();
                        
                        
                        Handles.color = colorrunningstopped;
                        if (drive.CurrentSpeed < 0)
                            Handles.color = colorrunning;
                        Handles.ConeHandleCap(idrevert, posrevert, dirrotrevert, size * sizecones,
                            EventType.Repaint);
                    }

                    // Draw Handles in the Directions not used
                    if (!Application.isPlaying)
                    {
                        var dirr1 = transform.rotation * dirrot;
                        var dirr2 = transform.rotation * dirdrive;
                        posinactive1 = startpos + (dirr1) * size * sizearc + (dirdrive * radius * 0.5f);
                        posinactive2 = startpos + (dirr2) * size * sizearc + (dir1 * radius * 0.5f);
                        Handles.color = colorinactive;
                        dirrotquat = Quaternion.LookRotation(dirdrive);
                        Handles.ConeHandleCap(idnonactive1, posinactive1, dirrotquat, size * sizecones,
                            EventType.Repaint);

                        dirrotquat = Quaternion.Inverse(Quaternion.LookRotation(-dir1));
                        Handles.ConeHandleCap(idnonactive2, posinactive2, dirrotquat, size * sizecones,
                            EventType.Repaint);
                    }
                }
            }

                
            if (Event.current.type == EventType.MouseDown)
            {
                int id = HandleUtility.nearestControl;
                if (!Application.isPlaying)
                {
                    if (id == idactive)
                    {
                        drive.ReverseDirection = !drive.ReverseDirection;
                    }

                    if (id == idnonactive1)
                    {
                        drive.Direction = dirnotused1;
                    }

                    if (id == idnonactive2)
                    {
                        drive.Direction = dirnotused2;
                    }
                }
                else
                {
                    {
                        if (id == idactive)
                        {
                            if (!drive.JogForward)
                            {
                                drive.JogForward = true;
                                drive.JogBackward = false;
                            }
                            else
                            {
                                drive.JogForward = false;
                                drive.JogBackward = false;
                            }
                        }

                        if (id == idrevert || id == idposmin)
                        {
                            if (!drive.JogBackward)
                            {
                                drive.JogBackward = true;
                                drive.JogForward = false;
                            }
                            else
                            {
                                drive.JogForward = false;
                                drive.JogBackward = false;
                            }
                        }
                    }
                }
            }
        }


        private Vector3 DirectionToVector(Drive drive, bool global)
        {
            Vector3 result = Vector3.up;
            switch (drive.Direction)
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
            }

            if (global)
                return drive.transform.TransformDirection(result);
            else
                return result;
        }

        private Vector3 DirectionToVector(Drive drive, DIRECTION dir, bool global)
        {
            Vector3 result = Vector3.up;
            switch (dir)
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
            }

            if (global)
                return drive.transform.TransformDirection(result);
            else
                return result;
        }

        private Vector3 DirectionNotUsed1(Drive drive, bool global)
        {
            DIRECTION result = DIRECTION.LinearX;
            switch (drive.Direction)
            {
                case DIRECTION.LinearX:
                    result = DIRECTION.LinearY;
                    break;
                case DIRECTION.LinearY:
                    result = DIRECTION.LinearZ;
                    break;
                case DIRECTION.LinearZ:
                    result = DIRECTION.LinearX;
                    break;
                case DIRECTION.RotationX:
                    result = DIRECTION.RotationY;
                    break;
                case DIRECTION.RotationY:
                    result = DIRECTION.RotationZ;
                    break;
                case DIRECTION.RotationZ:
                    result = DIRECTION.RotationX;
                    break;
            }

            dirnotused1 = result;
            return DirectionToVector(drive, result,global);
        }

        private Vector3 DirectionNotUsed2(Drive drive, bool global)
        {
            DIRECTION result = DIRECTION.LinearX;
            switch (drive.Direction)
            {
                case DIRECTION.LinearX:
                    result = DIRECTION.LinearZ;
                    break;
                case DIRECTION.LinearY:
                    result = DIRECTION.LinearX;
                    break;
                case DIRECTION.LinearZ:
                    result = DIRECTION.LinearY;
                    break;
                case DIRECTION.RotationX:
                    result = DIRECTION.RotationZ;
                    break;
                case DIRECTION.RotationY:
                    result = DIRECTION.RotationX;
                    break;
                case DIRECTION.RotationZ:
                    result = DIRECTION.RotationY;
                    break;
            }

            dirnotused2 = result;
            return DirectionToVector(drive, result,global);
        }
    }
}