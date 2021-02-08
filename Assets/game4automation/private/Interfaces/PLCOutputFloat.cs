﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using UnityEngine;

namespace game4automation
{
	[System.Serializable]

	//! PLC FLOAT Output Signal
	public class PLCOutputFloat : Signal
	{

		public StatusFloat Status;
		private float _value;

		public float Value
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

		public override string GetVisuText()
		{
			return Value.ToString("0.0");
		}

		public override void SetValue(string value)
		{
			if (value != "")
				Value = float.Parse(value);
			else
				Value = 0;
		}
		
		public override void SetValue(object  value)
		{
			if (value != null)
			{
				Type t = value.GetType();
				if (t == typeof(double))
				{

					Value = Convert.ToSingle(value);
				}
				else
				{
					Value = (float) value;
				}
			}
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
