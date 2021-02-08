﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;

namespace game4automation
{
    [System.Serializable]

    //! PLC INT Output Signal
    public class PLCOutputInt : Signal
    {
        public StatusInt Status;
        private float _value;
        public int Value
        {
            get
            {
                if (Settings.Override)
                {
                    return Status.ValueOverride;
                }
                else
                {
                    return Status.Value;
                }
            }
            set
            { 
                var oldvalue = Status.Value;
                Status.Value = value;
                if (oldvalue != value)
                    SignalChangedEvent(this);
            }
        }

        public override void SetStatusConnected(bool status)
        {
            Status.Connected = status;
        }

        public override bool GetStatusConnected()
        {
            return Status.Connected;
        }

        // When Script is added or reset ist pushed
        private void Reset()
        {
            Settings.Active = true;
            Settings.Override = false;
            Status.Value = 0;
        }

        public override void SetValue(string value)
        {
            if (value != "")
                Status.Value = int.Parse(value);
            else
                Status.Value = 0;

     
        }

        //! Sets the value as an int
        public void SetValue(int value)
        {
            Value = value;
         
        }

        //! Sets the value as an int
        public override void SetValue(object value)
        {
            if (value != null)
            {
                Type t = value.GetType();
                try
                {
                    Value = Convert.ToInt32(value);
                } catch
                {}
            }

        }

        public override object GetValue()
        {
            return Value;
        }

        public override string GetVisuText()
        {
            return Value.ToString("0");
        }
        
        public void Update()
        {
            if (Status.OldValue != Status.Value)
            {
                EventSignalChanged.Invoke(this);
                Status.OldValue = Status.Value;
            }		
        }
    }
}