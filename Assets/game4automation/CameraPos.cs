using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game4automation
{
    [CreateAssetMenu(fileName = "CameraPos", menuName = "game4automation/Add Camera Position", order = 1)]
    public class CameraPos : ScriptableObject
    {
        public string Description;
        public Vector3 CameraRot;
        public Vector3 TargetPos; 
        public float CameraDistance; 
        
  
        public void SaveCameraPosition(SceneMouseNavigation mousenav)
        {
            CameraRot = mousenav.currentRotation.eulerAngles;
            CameraDistance = mousenav.currentDistance; 
            TargetPos = mousenav.target.position;
        }
    }

}

