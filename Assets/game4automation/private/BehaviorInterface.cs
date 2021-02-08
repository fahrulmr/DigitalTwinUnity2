// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace game4automation
{
	public class BehaviorInterfaceConnection
	{
		public Signal Signal;
		public string Name;
	}

	public class BehaviorInterface : Game4AutomationBehavior
	{

		public List<BehaviorInterfaceConnection> ConnectionInfo = new List<BehaviorInterfaceConnection>(); 
		
		public void UpdateConnectionInfo()
		{
			ConnectionInfo.Clear();
			Type mytype = this.GetType();
				FieldInfo[] fields = mytype.GetFields();

				foreach (FieldInfo field in fields)
				{
					if (field != null)
					{
						var type = field.FieldType;
						if (type.IsSubclassOf(typeof(game4automation.Signal)))
						{
							var info = new BehaviorInterfaceConnection();
							info.Name = field.Name;
							info.Signal = (Signal)field.GetValue(this);		
							ConnectionInfo.Add(info);
						}
					}
				}
		}
		
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}
	}
}
