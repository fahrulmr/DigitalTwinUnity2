// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using System;
using UnityEngine;

namespace game4automation
{
	public class Sound : MonoBehaviour
	{

		public enum Soundmode {Drive,Pick}
		public enum Soundtype {Robot1,Robot2,SmallElectric1,SmallElectric2,AirPressure}
	
		[Header("Settings")]
		public Soundmode Mode = Soundmode.Drive;
		public Soundtype SoundType;
		public float SpeedMax = 200;
		public float PitchMin = 0.9f;
		public float PitchMax = 1.1f;

		[Header("Sound IOs")]
		public bool PlayAudio;
		public float Volume = 1;
		public float Pitch = 1;

		public bool IsPlaying;
	
		private bool _isplayingbefore = false;
		private AudioSource AudioSource;
		private AudioClip AudioClip;
		private Drive _drive;
		private Soundtype _soundtypebefore;


		void SetSound()
		{
			AudioSource = gameObject.GetComponent<AudioSource>();
			if (AudioSource == null)
			{
				AudioSource = gameObject.AddComponent<AudioSource>();
			}
			IsPlaying = false;
			switch (SoundType)
			{
				case Soundtype.Robot1:
					AudioClip = UnityEngine.Resources.Load("Sounds/robot1", typeof(AudioClip)) as AudioClip;
					break;
				case Soundtype.Robot2:
					AudioClip =  UnityEngine.Resources.Load("Sounds/robot2", typeof(AudioClip)) as AudioClip;
					break;
				case Soundtype.SmallElectric1:
					AudioClip =  UnityEngine.Resources.Load("Sounds/smallelectrical1", typeof(AudioClip)) as AudioClip;
					break;
				case Soundtype.SmallElectric2:
					AudioClip =  UnityEngine.Resources.Load("Sounds/smallelectrical2", typeof(AudioClip)) as AudioClip;
					break;
				case Soundtype.AirPressure:
					AudioClip =  UnityEngine.Resources.Load("Sounds/airpressure", typeof(AudioClip)) as AudioClip;
					break;
			}

			switch (Mode)
			{
				case Soundmode.Drive :
					AudioSource.loop = true;
					_drive = GetComponent<Drive>();
					break;
				case Soundmode.Pick :
					AudioSource.loop = false;
					break;
			}
			AudioSource.clip = AudioClip;	
		}

		// Use this for initialization
		void Start ()
		{
			SetSound();
		}

		// Update is called once per frame
		void Update ()
		{
			if (_soundtypebefore != SoundType)
			{
				SetSound();
				_soundtypebefore = SoundType;
			}
			switch (Mode)
			{
				case Soundmode.Drive :
					var speed = Math.Abs(_drive.CurrentSpeed);
					if (speed > SpeedMax)
					{
						Pitch = PitchMax;
					}
					else
					{
						Pitch = PitchMin + (PitchMax - PitchMin) * speed/ SpeedMax;
					}
					if (speed > 0)
					{
						PlayAudio = true;
					}
					else
					{
						PlayAudio = false;
					}
					break;
				case Soundmode.Pick :
					break;
			}
		
			AudioSource.volume = Volume;
			AudioSource.pitch = Pitch;

		
			if (PlayAudio == true && _isplayingbefore == false)
			{
				AudioSource.Play();
				IsPlaying = true;
				_isplayingbefore = true;
			}

			if (PlayAudio == false)
			{
				IsPlaying = false;
				_isplayingbefore = false;
				AudioSource.Stop();
			}
		}
	}
}
