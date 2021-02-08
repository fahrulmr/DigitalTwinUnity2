
using UnityEngine;

namespace game4automation
{
    [RequireComponent(typeof(Grip))]
    public class Grip_SingleIO : BehaviorInterface
    {
        public PLCOutputInt GripInteger;
        public PLCOutputBool GripBoolean;
        public Drive_Cylinder ConnectedCylinder;
        public bool OnPickCylinderOut = true;
        private bool _isoutint;
        private bool _isoutbool;

        private Grip grip;

        private bool _cylinder;

        // Start is called before the first frame update
        void Start()
        {
            _isoutint = GripInteger != null;
            _isoutbool = GripBoolean != null;
            grip = GetComponent<Grip>();
            _cylinder = ConnectedCylinder != null;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (_isoutint)
            {
                if (GripInteger.Value > 0)
                {
                    grip.PickObjects = true;
                    grip.PlaceObjects = false;
                    if (_cylinder)
                    {
                        ConnectedCylinder._in = !OnPickCylinderOut;
                        ConnectedCylinder._out = OnPickCylinderOut;
                    }
                }
                else
                {
                    grip.PickObjects = false;
                    grip.PlaceObjects = true;
                    if (_cylinder)
                    {
                        ConnectedCylinder._in = OnPickCylinderOut;
                        ConnectedCylinder._out = !OnPickCylinderOut;
                    }
                }
            }

            if (_isoutbool)
            {
                if (GripBoolean.Value)
                {
                    grip.PickObjects = true;
                    grip.PlaceObjects = false;
                    if (_cylinder)
                    {
                        ConnectedCylinder._in = !OnPickCylinderOut;
                        ConnectedCylinder._out = OnPickCylinderOut;
                    }
                }
                else
                {
                    grip.PickObjects = false;
                    grip.PlaceObjects = true;
                    if (_cylinder)
                    {
                        ConnectedCylinder._in = OnPickCylinderOut;
                        ConnectedCylinder._out = !OnPickCylinderOut;
                    }
                }
            }
        }
    }
}