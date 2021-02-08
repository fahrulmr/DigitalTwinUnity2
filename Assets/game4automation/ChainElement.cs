using UnityEngine;

using game4automation;
using game4automationtools;


namespace game4automation
{
    [HelpURL("https://game4automation.com/documentation/current/chainelement.html")]
    [SelectionBase]
//! An element which is transported by a chain (moving along the spline on the chain)
    public class ChainElement : Game4AutomationBehavior
    {
        [Header(("Settings"))]
        public bool
            AlignWithChain = true; //!< true if the chainelement needs to align with the chain tangent while moving

        public bool
            MoveRigidBody = true; //!< needs to be set to true if chainelements has colliders which should make parts move physically
        
        [ShowIf("AlignWithChain")]
        public Vector3 AlignVector = new Vector3(1, 0, 0); //!< additinal rotation for the alignment

        [game4automationtools.ReadOnly] public Drive ConnectedDrive; //!< Drive where the chain is connected to
        [game4automationtools.ReadOnly] public float StartPosition; //!< Start position of this chain element
        [game4automationtools.ReadOnly] public Chain Chain; //!< Chain where this chainelement belongs to
        [game4automationtools.ReadOnly] public float Position; //!< Current position of this chain element
        [game4automationtools.ReadOnly] public float RelativePosition; //!< Relative position of this chain element

        private Vector3 _targetpos;
        private Quaternion targetrotation;
        private Vector3 tangentforward;
        private Game4AutomationController game4Automation;
        private bool chainnotnull = false;

        private Rigidbody _rigidbody;
        
        public void SetPosition()
        {
            RelativePosition = Position / Chain.Length;
            var positon  = Chain.GetPosition(RelativePosition);

            if (MoveRigidBody)
                _rigidbody.MovePosition(positon);
            else
                transform.position = positon;
            _targetpos = transform.position;
            if (AlignWithChain)
            {
                var rotation =  Quaternion.LookRotation(Chain.GetTangent(RelativePosition), AlignVector);
                if (MoveRigidBody)
                    _rigidbody.MoveRotation(rotation);
                else
                    transform.rotation = rotation;
            }
        }

        public void UpdatePosition(float deltaTime)
        {
            Position = ConnectedDrive.CurrentPosition + StartPosition;

            if (Position > Chain.Length)
            {
                var rounds = Position / Chain.Length;
                Position = (Position % Chain.Length);
            }

            SetPosition();
        }


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (Chain != null)
            {
                chainnotnull = true;
                SetPosition();
            }
            else
                chainnotnull = false;

        }

        private void Update()
        {
            if (chainnotnull)
                UpdatePosition(Time.deltaTime);
        }
    }
}