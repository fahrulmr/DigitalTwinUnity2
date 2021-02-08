// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using game4automation;
using UnityEngine;

namespace game4automation
{
	[RequireComponent(typeof(Drive))]
	//! Behavior model of a drive which is connected to another drive with a gear.
	//! All positions of the master drive are directly transfered with offset and gear factor to this drive.
	//! The formula is CurrentPosition = MasterDrive.CurrentPosition * GearFactor+Offset;
	[HelpURL("https://game4automation.com/documentation/current/drivebehaviour.html")]
	public class Drive_Gear : BehaviorInterface
	{
		[Header("Settings")]
		public Drive MasterDrive; //!< Master drive which is defining the position of this drive
		public float GearFactor = 1; //!< Gear factor of the gear
		public float Offset = 0; //!< Offset of the gear in millimeter
		
		private Drive _thisdrive; 
		private Drive Drive;
		
		// Use this for initialization
		void Start()
		{
			_thisdrive = GetComponent<Drive>();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			_thisdrive.CurrentPosition = MasterDrive.CurrentPosition * GearFactor+Offset;

		}
	}
}
