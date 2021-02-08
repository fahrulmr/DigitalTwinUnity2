// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace game4automation
{
    public class InterfaceBaseClass : Game4AutomationBehavior
    {
        [ReadOnly] public bool IsConnected=false;
        
        public List<InterfaceSignal> InterfaceSignals = new List<InterfaceSignal>();
        

        //! Creates a new List of InterfaceSignals based on the Components under this Interface GameObject
        public void UpdateInterfaceSignals(ref int inputs, ref int outputs)
        {
            InterfaceSignals.Clear();
            inputs = 0;
            outputs = 0;
            var signals =  gameObject.GetComponentsInChildren(typeof(Signal), true);
            foreach (Signal signal in signals)
            {
                var newsignal = signal.GetInterfaceSignal();
                InterfaceSignals.Add(newsignal);
                if (newsignal.Direction == InterfaceSignal.DIRECTION.INPUT)
                {
                    inputs++;
                }
                if (newsignal.Direction == InterfaceSignal.DIRECTION.OUTPUT)
                {
                    outputs++;
                }
            }
        }
           //! Create a signal object as sub gameobject
        public Signal CreateSignalObject(string name, SIGNALTYPE type, SIGNALDIRECTION direction)
        {
            GameObject signal;
            Signal signalscript = null;

            signal = GetChildByName(name);
            if (signal == null)
            {
                signal = new GameObject("name");
                signal.transform.parent = this.transform;
                signal.name = name;
            }

            if (direction == SIGNALDIRECTION.INPUT)
            {
                // Byte and  Input
                switch (type)
                {
                    case SIGNALTYPE.BOOL:
                        if (signal.GetComponent<PLCInputBool>() == null)
                        {
                            signal.AddComponent<PLCInputBool>();
                        }

                        break;
                    case SIGNALTYPE.INT:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputInt>();
                        }

                        break;
                    case SIGNALTYPE.DINT:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputInt>();
                        }

                        break;
                    case SIGNALTYPE.BYTE:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputInt>();
                        }

                        break;
                    case SIGNALTYPE.WORD:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputInt>();
                        }

                        break;
                    case SIGNALTYPE.DWORD:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputInt>();
                        }

                        break;
                    case SIGNALTYPE.REAL:
                        if (signal.GetComponent<PLCInputInt>() == null)
                        {
                            signal.AddComponent<PLCInputFloat>();
                        }

                        break;
                }
            }

            if (direction == SIGNALDIRECTION.OUTPUT)
            {
                switch (type)
                {
                    case SIGNALTYPE.BOOL:
                        if (signal.GetComponent<PLCOutputBool>() == null)
                        {
                            signal.AddComponent<PLCOutputBool>();
                        }

                        break;
                    case SIGNALTYPE.INT:
                        if (signal.GetComponent<PLCOutputInt>() == null)
                        {
                            signal.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case SIGNALTYPE.DINT:
                        if (signal.GetComponent<PLCOutputInt>() == null)
                        {
                            signal.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case SIGNALTYPE.BYTE:
                        if (signal.GetComponent<PLCOutputInt>() == null)
                        {
                            signal.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case SIGNALTYPE.WORD:
                        if (signal.GetComponent<PLCOutputInt>() == null)
                        {
                            signal.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case SIGNALTYPE.DWORD:
                        if (signal.GetComponent<PLCOutputInt>() == null)
                        {
                            signal.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case SIGNALTYPE.REAL:
                        if (signal.GetComponent<PLCOutputFloat>() == null)
                        {
                            signal.AddComponent<PLCOutputFloat>();
                        }

                        break;
                }
            }
            signalscript = signal.gameObject.GetComponent<Signal>();
            if (signalscript != null)
            {
                signalscript.Settings.Active = true;
                signalscript.SetStatusConnected(true);
            }

            return signalscript;
        }
        
        
        public Signal AddSignal(InterfaceSignal interfacesignal)
        {
            GameObject signalobject = null;
            Signal newsignal = null;
            InterfaceSignals.Add(interfacesignal);
            signalobject = GetSignal(interfacesignal.Name);
            if (signalobject == null)
            {
                signalobject = new GameObject("name");
                signalobject.transform.parent = this.transform;
                if (interfacesignal.SymbolName != "")
                {
                    signalobject.name = interfacesignal.SymbolName;
                }
                else
                {
                    signalobject.name = interfacesignal.Name;
                }
            }

            Signal oldsignal = signalobject.GetComponent<Signal>();
            if (interfacesignal.Direction == InterfaceSignal.DIRECTION.INPUT)
            {
                // Byte and  Input
                switch (interfacesignal.Type)
                {
                    case InterfaceSignal.TYPE.BOOL:
                        if (signalobject.GetComponent<PLCInputBool>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCInputBool>();
                        }

                        break;
                    case InterfaceSignal.TYPE.INT:
                        if (signalobject.GetComponent<PLCInputInt>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCInputInt>();
                        }

                        break;

                    case InterfaceSignal.TYPE.REAL:
                        if (signalobject.GetComponent<PLCInputFloat>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCInputFloat>();
                        }

                        break;
                }
            }

            if (interfacesignal.Direction == InterfaceSignal.DIRECTION.OUTPUT)
            {
                switch (interfacesignal.Type)
                {
                    case InterfaceSignal.TYPE.BOOL:
                        if (signalobject.GetComponent<PLCOutputBool>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCOutputBool>();
                        }

                        break;
                    case InterfaceSignal.TYPE.INT:
                        if (signalobject.GetComponent<PLCOutputInt>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCOutputInt>();
                        }

                        break;
                    case InterfaceSignal.TYPE.REAL:
                        if (signalobject.GetComponent<PLCOutputFloat>() == null)
                        {
                            newsignal = signalobject.AddComponent<PLCOutputFloat>();
                        }

                        break;
                }
            }

            if (oldsignal != newsignal && newsignal != null)
            {
                DestroyImmediate(oldsignal);
            }

            interfacesignal.Signal = signalobject.gameObject.GetComponent<Signal>();

            if (interfacesignal.Signal != null)
            {
                //if (newsignal)
                //{
                interfacesignal.Signal.Comment = interfacesignal.Comment;
                interfacesignal.Signal.OriginDataType = interfacesignal.OriginDataType;
                if (interfacesignal.SymbolName != "")
                {
                    interfacesignal.Signal.Name = interfacesignal.Name;
                }
                //}

                interfacesignal.Signal.Settings.Active = true;
                interfacesignal.Signal.SetStatusConnected(true);
            }

            return interfacesignal.Signal;
        }

        public void RemoveSignal(InterfaceSignal interfacesignal)
        {
            if (interfacesignal.Signal != null)
            {
                Destroy(interfacesignal.Signal.gameObject);
            }

            InterfaceSignals.Remove(interfacesignal);
        }
        

        //! Gets a signal with a name
        public virtual GameObject GetSignal(string name)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>();
            // First check names of signals
            foreach (var child in children)
            {
                var signal = child.GetComponent<Signal>();
                if (signal != null && child != gameObject.transform)
                {
                    if (signal.Name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            // Second check names of components
            foreach (var child in children)
            {
                if (child != gameObject.transform)
                {
                    if (child.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }

        protected  void OnConnected()
        {
            SetAllSignalStatus(true);
            IsConnected = true;
            if (Game4AutomationController!=null)
                  Game4AutomationController.OnConnectionOpened(gameObject);
        }

        protected  void OnDisconnected()
        {
            SetAllSignalStatus(false);
 
            IsConnected = false;
            if (Game4AutomationController!=null)
                Game4AutomationController.OnConnectionClosed(gameObject);
        }
        
      
        public virtual void OpenInterface()
        {
        }

        public virtual void CloseInterface()
        {
        }
        
        public void SetAllSignalStatus(bool connected)
        {
            var signals = GetComponentsInChildren<Signal>();
            foreach (var signal in signals)
            {
                signal.SetStatusConnected(connected);
            }
        }
        
        public void DestroyAllSignals()
        {
            var signals = GetComponentsInChildren<Signal>();
            foreach (var signal in signals.ToArray())
            {
                if (signal != null)
                    DestroyImmediate(signal.gameObject);
            }
        }

        public void DeleteSignals()
        {
            InterfaceSignals.Clear();
        }

        void OnEnable()
        {
            Debug.Log("OnEnable " + name);
            OpenInterface();
        }

        void OnDisable()
        {
            CloseInterface();
        }
        
        
        
    }
}