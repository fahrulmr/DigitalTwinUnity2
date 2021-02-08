// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using System.IO.MemoryMappedFiles;
using System.Text;
    
namespace game4automation
{
    
    [System.Serializable] 
    //! Struct for an SHM Signal
    public struct SHMSignal
    {
        [ReadOnly] public Signal signal; //!< Connected Signal to the SHM signal
        [ReadOnly] public string name; //!< Name of the SHM signal
        [ReadOnly] public SIGNALTYPE type; //!< Type of the SHM signal
        [ReadOnly] public SIGNALDIRECTION direction; //!< Direction of the SHM signal
        [ReadOnly] public int mempos; //!< Memory position (byte position) in the Shared memory of the signal
        [ReadOnly] public byte bit;  //!< Bit position (0 if no bit) of the Signal in the shared memory
    }

    
    //! Base class for all types of shared memory interfaces (even with different structure as simit like the plcsimadvanced interface
    public class InterfaceSHMBaseClass : InterfaceBaseClass
    {
        
        
        protected string ReadString(MemoryMappedViewAccessor accessor, long pos, int size)
        {
            string res = "";

            byte[] buffer = new byte[size];
            int count = accessor.ReadArray<byte>(pos, buffer, 0, (byte) size);
            if (count == size)
            {
                res = Encoding.Default.GetString(buffer);
            }

            return res;
        }
        
    }
}