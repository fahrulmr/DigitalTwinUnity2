// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using UnityEngine;
using System.Collections.Generic;
using game4automationtools;

namespace game4automation
{

	[Serializable]
	//! Struct for Settings of Signals
	public struct SettingsSignal
	{
		public bool Active;
		public bool Override;
	}
	
	[Serializable]
	//! Struct for current status of a bool signal
	public struct StatusBool
	{
		public bool Connected;
		public bool ValueOverride;
		public bool Value;
		[HideInInspector] public bool OldValue;
	}

	[Serializable]
	//! Struct for current status of a float signal
	public struct StatusFloat
	{
		public bool Connected;
		public float ValueOverride;
		public float Value;
		[HideInInspector] public float OldValue;
	}
 
 
	[Serializable]
	//! Struct for current status of a omt signal
	public struct StatusInt
	{
		public bool Connected;
		public int ValueOverride;
		public int Value;
		[HideInInspector] public int OldValue;
	}
	
	[Serializable]
	public class Connection
	{
		public BehaviorInterface Behavior;
		public string ConnectionName;
	}
	
	//! The base class for all Signals
	public class Signal : Game4AutomationBehavior
	{

		public string Comment;
		public string OriginDataType;
		public SettingsSignal Settings;
		protected string Visutext;
		public SignalEvent EventSignalChanged;


		[HideInInspector]
		public List<Connection> ConnectionInfo = new List<Connection>();


		protected new bool hidename() { return false; }
		
		//!  Virtual for getting the text for the Hierarchy View
		public virtual string GetVisuText()
		{
			return "not implemented";
		}

		//! Virtual for getting information if the signal is an Input
		public virtual bool IsInput()
		{
			return false;
		}

		//! Virtual for setting the value
		public virtual void SetValue(string value)
		{

		}
		
		
		//! Virtual for toogle in hierarhy view
		public virtual void OnToggleHierarchy()
		{

		}
		
		//! Virtual for setting the Status to connected
		public virtual void SetStatusConnected(bool status)
		{
	
		}
		
		//! Sets the value of the signal
		public virtual void SetValue(object value)
		{
			
		}

		//! Unforces the signal
		public void Unforce()
		{
			Settings.Override = false;
			EventSignalChanged.Invoke(this);
			SignalChangedEvent(this);
		}
		
		//! Gets the value of the signal
		public virtual object GetValue()
		{
			return null;
		}
	
		//! Virtual for getting the connected Status
		public virtual bool GetStatusConnected()
		{
			return false;
		}

		public void DeleteSignalConnectionInfos()
		{
			ConnectionInfo.Clear();	
		}
		
		public void AddSignalConnectionInfo(BehaviorInterface behavior, string connectionname)
		{
			var element = new Connection();
			element.Behavior = behavior;
			element.ConnectionName = connectionname;
			ConnectionInfo.Add(element);
			if (IsInput())
			{
				if (ConnectionInfo.Count > 1)
				{
					Error("PLCInput Signal is connected to more than one behavior model, this is not allowed",this);
				}
			}
		}
		
		//! Returns true if InterfaceSignal is connected to any Behavior Script
		public bool IsConnectedToBehavior()
		{
			if (ConnectionInfo.Count > 0)
				return true;
			else
				return false;
		}
		
		//! Returns an InterfaceSignal Object based on the Signal Component
		public InterfaceSignal GetInterfaceSignal()
		{
			var newsignal = new InterfaceSignal();
			newsignal.OriginDataType = OriginDataType;
			newsignal.Name = name;
			newsignal.SymbolName = Name;
			newsignal.Signal = this;
			var type = this.GetType().ToString();
			switch (type)
			{
				case "game4automation.PLCInputBool" :
					newsignal.Type = InterfaceSignal.TYPE.BOOL;
					newsignal.Direction = InterfaceSignal.DIRECTION.INPUT;
					break;
				case "game4automation.PLCOutputBool" :
					newsignal.Type = InterfaceSignal.TYPE.BOOL;
					newsignal.Direction = InterfaceSignal.DIRECTION.OUTPUT;
					break;
				case "game4automation.PLCInputFloat" :
					newsignal.Type = InterfaceSignal.TYPE.REAL;
					newsignal.Direction = InterfaceSignal.DIRECTION.INPUT;
					break;
				case "game4automation.PLCOutputFloat" :
					newsignal.Type = InterfaceSignal.TYPE.REAL;
					newsignal.Direction = InterfaceSignal.DIRECTION.OUTPUT;
					break;
				case "game4automation.PLCInputInt" :
					newsignal.Type = InterfaceSignal.TYPE.INT;
					newsignal.Direction = InterfaceSignal.DIRECTION.INPUT;
					break;
				case "game4automation.PLCOutputInt" :
					newsignal.Type = InterfaceSignal.TYPE.INT;
					newsignal.Direction = InterfaceSignal.DIRECTION.OUTPUT;
					break;
				case "game4automation.PLCInputTransform" :
					newsignal.Type = InterfaceSignal.TYPE.TRANSFORM;
					newsignal.Direction = InterfaceSignal.DIRECTION.INPUT;
					break;
				case "game4automation.PLCOutputTransform" :
					newsignal.Type = InterfaceSignal.TYPE.TRANSFORM;
					newsignal.Direction = InterfaceSignal.DIRECTION.OUTPUT;
					break;
			}

			return newsignal;
		}
		
		//! Is called when value is changed in inspector
		private void OnValidate()
		{
			SignalChangedEvent(this);		
		}
		
		private void Start()
		{
			SignalChangedEvent(this);
			if (EventSignalChanged != null)
			{
				if (EventSignalChanged.GetPersistentEventCount() == 0)
				{
					enabled = false;
				}
				else
				{
					enabled = true;
				}
			}
			else
			{
				enabled = false;
			}
		}
		
		public delegate void OnSignalChangedDelegate(Signal obj);  
		public event OnSignalChangedDelegate SignalChanged;

		protected void SignalChangedEvent(Signal signal)
		{
			if (SignalChanged != null)
				SignalChanged(signal);
		}
	
	}
}