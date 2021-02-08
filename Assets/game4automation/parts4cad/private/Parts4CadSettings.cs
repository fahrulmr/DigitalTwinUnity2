
using UnityEngine;

[CreateAssetMenu(fileName = "Parts4CadSettings", menuName = "game4automation/Add Parts4CadSettings", order = 1)]
public class Parts4CadSettings : ScriptableObject
{
   public bool UnpackWhenImported = true;

   public bool RotateToUnityStandard = true;

   public float ImportScale=0.001f;

   public bool ShortNamingInScene = true;
}
