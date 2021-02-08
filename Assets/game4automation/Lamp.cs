﻿// Game4Automation (R) Framework for Automation Concept Design, Virtual Commissioning and 3D-HMI
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEngine;
using game4automation;

namespace game4automation
{
    //! Lamp color enum
    public enum LAMPCOLOR
    {
        White,
        Yellow,
        Green,
        Red,
    }

    [SelectionBase]
    [ExecuteInEditMode]
    //! Lamp for displaying a lamp in the scene
    public class Lamp : Display
    {

        [Header("Settings")]
        public LAMPCOLOR LampColor; //!< Lamp color
        public Material MaterialWhite; //!<  Material for color white
        public Material MaterialGreen; //!<  Material for color green
        public Material MaterialYellow; //!<  Material for color yellow
        public Material MaterialRed; //!< Material for color red
        public bool UseHalo; //!<  Use halo for lamp
        public float Diameter; //!<  Diameter of lamp in mm
        public float Height; //!< Height of lamp in mm
        public float LightRange; //!< Light range of lamp in mm
    
        [Header("Lamp IO's")]
        public int InColor = 0; //!<  Color of lamp,(0=White, 1 = Green, 2 = Yellow, 3 = Red)
        public bool Flashing = false; //!<  True if lamp should be flashing.
        public float Period = 1; //!<  Lamp fleshing period in seconds.
        public bool LampOn = false; //!  Lamp is on if true.

        private Material _coloron;
        private Material _coloroff;

        private MeshRenderer _meshrenderer;

        private LAMPCOLOR _colorbefore;
        private float _timeon;
        private int _incolorbefore;
        private bool _flashingbefore;
        private float _periodbefore;
        private bool _lamponbefore;
        private bool _lampon;
        private Light _lamp;
        private Behaviour _helo;
        private Color _color;
        private Material _material;
        private Transform _cylinder;

        // Change olor
        
        // Use this for initialization
        private void InitLight()
        {
            InColor = (int) LampColor;
            _meshrenderer = GetMeshRenderer();
            switch (LampColor)
            {
                case LAMPCOLOR.White:
                    _material = MaterialWhite;
                    _color = Color.white;
                    break;
                case LAMPCOLOR.Yellow:
                    _material = MaterialYellow;
                    _color = Color.yellow;
                    break;
                case LAMPCOLOR.Green:
                    _material = MaterialGreen;
                    _color = Color.green;
                    break;
                case LAMPCOLOR.Red:
                    _material = MaterialRed;
                    _color = Color.red;
                    break;
            }

            Material newMaterial = Material.Instantiate(_material);
            _meshrenderer.material = newMaterial;
            if (_material == null)
            {
                Error("No Material in Lamp selected",this);
            }
            if (_lamp != null)
            {
                _lamp.color = _color;
                if (Game4AutomationController!=null)
                   _lamp.range = LightRange / Game4AutomationController.Scale;
            }
            if (_cylinder != null)
            {
                if (Game4AutomationController!=null)
                      _cylinder.localScale = new Vector3(Diameter/Game4AutomationController.Scale,Height/(2*Game4AutomationController.Scale),Diameter/Game4AutomationController.Scale);   
            }
        
        }

        private void OnValidate()
        {
            InitLight();
        }

        public void Start()
        {
      
            _timeon = Time.time;
            _colorbefore = LampColor;
            _incolorbefore = InColor;
            _lamponbefore = LampOn;
            _lamp = GetComponentInChildren<Light>();
            _helo = (Behaviour)GetComponent("Halo");
            _cylinder = gameObject.transform.Find("Cylinder");
     
            InitLight();
            Off();
        
        }

        //! Turns the lamp on.
        public void On()
        {
            LampOn = true;
            _meshrenderer.sharedMaterial.EnableKeyword("_EMISSION");
            _meshrenderer.sharedMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            _meshrenderer.sharedMaterial.SetColor("_EmissionColor",_color);
            if (_lamp)
            {
                _lamp.enabled = true;
            }
            if (_helo && UseHalo)
            {
                _helo.enabled = true;
            }
       
        }

        //!  Turns the lamp off.
        public void Off()
        {
            LampOn = false;
            _meshrenderer.sharedMaterial.DisableKeyword("_EMISSION");
            _meshrenderer.sharedMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            _meshrenderer.sharedMaterial.SetColor("_EmissionColor",Color.black);
       
            if (_lamp)
            {
                _lamp.enabled = false;
            }
            if (_helo && UseHalo)
            {
                _helo.enabled = false;
            }
        }


        // Update is called once per frame
        void Update()
        {

            if (_colorbefore != LampColor || _incolorbefore != InColor)
            {
                if (_incolorbefore != InColor)
                {
                    LampColor = (LAMPCOLOR)InColor;
                }
                InitLight();
            }

            if (Flashing)
            {
                float delta = Time.time - _timeon;
                if (!_lampon && delta > Period)
                {
                    _lampon = true;
                }
                else
                {
                    if (_lampon && delta > Period / 2)
                    {
                        _lampon = false;
                    }
                }
            }

            if (!Flashing)
            {
                _lampon = LampOn;
            }

            if (_lampon && _lampon != _lamponbefore)
            {
                On();
                _timeon = Time.time;
            }

            if (!_lampon && _lampon != _lamponbefore)
            {
                Off();
            }

            _colorbefore = LampColor;
            _incolorbefore = InColor;
            _lamponbefore = _lampon;
        }
    }
}