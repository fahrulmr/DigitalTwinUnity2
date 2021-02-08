// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;

namespace game4automation
{
	public class Game4AutomationUI : BehaviorInterface {
		
	
	
		public void SetColor(GameObject obj, Color color)
		{
			var renderers =  obj.GetComponentsInChildren<MeshRenderer>();
			foreach (Renderer render in renderers)
			{
				MaterialPropertyBlock props = new MaterialPropertyBlock();
				props.SetColor("_Color",color);
				props.SetColor("_Emission",color);
				render.SetPropertyBlock(props);
			}
		}
	
		public void ResetColor(GameObject obj)
		{
			var renderers =  obj.GetComponentsInChildren<MeshRenderer>();
			foreach (Renderer render in renderers)
			{
				render.SetPropertyBlock(null);
			}
		}
		
	}
}
