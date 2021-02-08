using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game4automation
{
    public class DemoGripRobot : MonoBehaviour
    {
        // Start is called before the first frame update
        public void OnGrip(MU mu, bool grip)
        {
            mu.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            foreach (var rb in mu.GetComponentsInChildren<Rigidbody>())
            {
                rb.interpolation = RigidbodyInterpolation.None;
            }
        }

  
    }
}

