using System.Collections;
using System.Collections.Generic;
using game4automation;
using UnityEngine;
using game4automationtools;

namespace game4automation
{
    public class FirstPersonController : MonoBehaviour
    {
        public GameObject MainCamera;
        public GameObject Head;
        [OnValueChanged("Setup")] public float Size = 1.6f;
        public float JumpHeight = 2.5f;

        public float SensitivityX = 100f;
        public float SensitivityY = 100f;
        public float WalkSpeed = 5f;
        public float RunMultiplier = 2;
        public float Smoothing = 2.0f;
        public float HeadRotationLimits = 190f;
        public GenericButton Button;


        private SceneMouseNavigation _sceneMouseNavigation;
        private Camera _camera;
        private bool _cameranotnull = false;
        private bool _isinknee = false;
        private Rigidbody _rigidbody;
        private CharacterController _characterController;

        private float xRotation;

        public void SetActive(bool active)
        {
            if (Button != null)
                Button.SetStatus(active);
            this.enabled = active;
        }

        void Setup()
        {
            _characterController.height = Size;
            Head.transform.localPosition = new Vector3(0, Size / 2 - 0.1f, 0);
        }

        void Awake()
        {
            if (MainCamera != null)
            {
                _sceneMouseNavigation = MainCamera.GetComponent<SceneMouseNavigation>();
                _camera = MainCamera.GetComponent<Camera>();
                _cameranotnull = true;
            }

            _rigidbody = GetComponent<Rigidbody>();
            _characterController = GetComponent<CharacterController>();
            Setup();
        }

        // Update is called once per frame
        void Update()
        {
            if (!_cameranotnull)
                return;


            float mouseX = Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime;

            float movex = Input.GetAxis("Horizontal");
            float movez = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.Y))
            {
                Head.transform.localPosition = new Vector3(0, (Size / 2 - 0.1f) / 2, 0);
                _isinknee = true;
            }
            else
            {
                if (_isinknee)
                {
                    Head.transform.localPosition = new Vector3(0, (Size / 2 - 0.1f), 0);
                    _isinknee = false;
                }
            }

            var run = 1f;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                run = RunMultiplier;
            }

            if (movex > 0 || movez > 0)
                Debug.Log(movex + " / " + movez);
            Vector3 move = transform.right * movex + transform.forward * movez;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -HeadRotationLimits, HeadRotationLimits);
            Head.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            _characterController.Move(move * WalkSpeed * Time.deltaTime * run);

            if (_sceneMouseNavigation.FirstPersonControllerActive)
            {
                MainCamera.transform.position = Head.transform.position;
                MainCamera.transform.rotation = Head.transform.rotation;
            }
        }
    }
}