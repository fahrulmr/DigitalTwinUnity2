﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  


namespace game4automation
{
	[System.Serializable]

	//! PLC BOOL Output Signal
	public class PLCOutputBool : Signal
	{

		public StatusBool Status;
	
		public bool Value
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

		public override void OnToggleHierarchy()
		{
			if (Settings.Override == false)
				Settings.Override = true;
			Status.ValueOverride = !Status.ValueOverride;
			EventSignalChanged.Invoke(this);
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
			Status.Value  = false;
		}
	

		public override string GetVisuText()
		{
			return Value.ToString();
		}

		// Sets the value as a string
		public override void SetValue(string value)
		{
			if (value != "")
				Value = bool.Parse(value);
			else
				Value = false;
		}

		public override void SetValue(object value)
		{
			if (value != null)
				Value = (bool)value;
		}
		
		public override object GetValue()
		{
			return Value;
		}

		// Sets the value as a bool
		public void SetValue(bool value)
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