
using UnityEngine;

namespace game4automation
{
	[ExecuteInEditMode]
	public class TextTimer : MonoBehaviour
	{
		public float CloseAfterSeconds;
		public GameObject HideObject;
		private bool _hide = false;

		private bool _hide2 = false;
		// Use this for initialization
		void Start ()
		{
			Invoke("Hide", CloseAfterSeconds);
		}

		public void Hide()
		{
			_hide = true;
		}
		
		void Show()
		{
			_hide = false;
			if (HideObject!=null)
				HideObject.SetActive(true);
		}

		void LateUpdate()
		{
			if (_hide2 == true)
			{	
				HideObject.SetActive(false);
				_hide2 = false;
			}

			if (_hide == true)
			{
				_hide2 = true; // Wait one cycle due to rebuild error message
				_hide = false;
			}
		}
	}
}
