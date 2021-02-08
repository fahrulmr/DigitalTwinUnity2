﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

namespace game4automation
{
	//! PLC FLOAT INPUT Signal
	public class PLCInputFloat : Signal
	{
		public StatusFloat Status;
	
		public float Value
		{
			get
			{
				if (Settings.Override)
				{
					return Status.ValueOverride;
				} else
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
	
		// When Script is added or reset ist pushed
		private void Reset()
		{
			Settings.Active= true;
			Settings.Override = false;
			Status.Value = 0;
			
		}
	
		public override void SetStatusConnected(bool status)
		{
			Status.Connected = status;
		}

		public override bool GetStatusConnected()
		{
			return Status.Connected;
		}


		public override string GetVisuText()
		{
			return Value.ToString("0.0");
		}
	
		public override bool IsInput()
		{
			return true;
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
		
		public override void SetValue(object value)
		{
			Value = (float)value;
		}
		
		public override object GetValue()
		{
			return Value;
		}
		
		//! Sets the value as a float
		public void SetValue(float value)
		{
			Value = value;
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
