using UnityEngine;

namespace game4automation.Resources.Gizmo
{
	public class Gizmo : MonoBehaviour {


		void Display(string name, Vector3 position, Quaternion rotation, float Scale)
		{
			transform.position = position;
			transform.rotation = rotation;
			Vector3 scale = Vector3.one * Scale;
			transform.localScale = scale;
			gameObject.name = name;
		}
	
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}
	}
}
