// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using game4automationtools;


namespace game4automation
{
	
	public class GenericButton : Game4AutomationUI
	{
	
		[Header("Status")] 
		public bool IsOn;
		public bool IsPressed;
		
		
		[Header("Display Settings")] 
		[OnValueChanged("UpdateStatus")]
		public Sprite ImageOn;
		[OnValueChanged("UpdateStatus")]
		public Sprite ImageOff;
		public Image ActiveImageOnOn;
		
		
		[Header("Button Actions")] 
		[ReorderableList] public List<GameObject> ActivateOnOn;
		[ReorderableList] public List<GameObject> ActivateOnOff;
		public ButtonEventOnClick EventOnClick;
		private Button _button;

		private Image _image;
		// Use this for initialization
		
		public void SetColor(Color color)
		{
			var colors = GetComponent<Button> ().colors;
			colors.normalColor = color;
			colors.selectedColor = color;
			GetComponent<Button> ().colors = colors;
		}

	
		void Start () {
		
			_button = GetComponent<Button>();
			_image = GetComponent<Image>();
			
			if (_button != null)
			{
				_button.onClick.AddListener(delegate { ButtonClicked(_button); });
			}
			UpdateButton();
		}


		public void SetStatus(bool ison)
		{
			IsOn = ison;
			UpdateButton();
		}
		
		void UpdateButton()
		{
			_image = GetComponent<Image>();
			if (IsOn)
			{
				if (ImageOn!=null)
					_image.overrideSprite = ImageOn;
				if (ActiveImageOnOn != null)
					ActiveImageOnOn.enabled = true;
				if (ActivateOnOn != null)
				{
					foreach (var go in ActivateOnOn)
					{
						if (go!=null)
							go.SetActive(true);
					}
				}
				if (ActivateOnOff != null)
				{
					foreach (var go in ActivateOnOff)
					{
						if (go!=null)
							go.SetActive(false);
					}
				}
					
			}
			else
			{
				if (ImageOff!=null)
					_image.overrideSprite = ImageOff;
				if (ActiveImageOnOn != null)
					ActiveImageOnOn.enabled = true;
				if (ActivateOnOn != null)
				{
					foreach (var go in ActivateOnOn)
					{
						if (go!=null)
							go.SetActive(false);
					}
				}
				if (ActivateOnOff != null)
				{
					foreach (var go in ActivateOnOff)
					{
						if (go!=null)
							go.SetActive(true);
					}
				}

			}

		}
		//Output the new state of the Toggle into Text
		void ButtonClicked(Button button)
		{

			IsOn = !IsOn;
			UpdateButton();
			EventOnClick.Invoke(this);

		}
		
		
	}
}