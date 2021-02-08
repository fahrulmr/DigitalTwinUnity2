
namespace game4automation
{
    using UnityEngine;

    public class TestSignalChanged : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Register Event During Runtime
            var com = GetComponent<PLCInputBool>();
            com.SignalChanged += SignalChangedRuntime;
        }

 
        public void SignalChangedRuntime(Signal signal)
        {
            Debug.Log("Signal Changed Runtime " + signal.ToString());
        }
    
        public void SignalChangedEditor(Signal signal)
        {
            Debug.Log("Signal Changed Editor " + signal.ToString());
        }
    }
}
