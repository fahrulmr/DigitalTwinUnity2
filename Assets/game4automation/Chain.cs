using UnityEngine;
using Pixelplacement;


//! game4automation namespace
namespace game4automation
{
    [HelpURL("https://game4automation.com/documentation/current/chainelement.html")]
    [RequireComponent(typeof(Spline))]
    //! Chain for moving objects and MUs on spline curves
    public class Chain : Game4AutomationBehavior
    {
        public GameObject ChainElement; //!< Chainelements which needs to be created along the chain
        public Drive ConnectedDrive; //!< The drive which is moving ths chain
        public int NumberOfElements; //!< The number of elements which needs to be created along the chain
        public float StartPosition; //!< The start position on the chain (offset) for the first element

        public bool
            CalculatedDeltaPosition =
                true; //!< true if the distance (DeltaPositoin) between the chain elements should be calculated based on number and chain length

        public float DeltaPosition; //! the calculated or set distance between two chain elements
        [ReadOnly] public float Length; //!< the calculated lenth of the spline

        private Spline spline;

        private void Init()
        {
            spline = GetComponent<Spline>();
        }

        private void Reset()
        {
            Init();
        }


        protected override void AfterAwake()
        {
            Init();

            var position = StartPosition;

            if (spline != null)
            {
                spline.CalculateLength();
                Length = spline.Length * Game4AutomationController.Scale;
            }

            if (CalculatedDeltaPosition)
                DeltaPosition = Length / NumberOfElements;
            for (int i = 0; i < NumberOfElements; i++)
            {
                var newelement = Instantiate(ChainElement, ChainElement.transform.parent);
                newelement.transform.parent = this.transform;
                var chainelement = newelement.GetComponent<ChainElement>();
                chainelement.StartPosition = position;
                chainelement.Position = position;
                chainelement.ConnectedDrive = ConnectedDrive;
                chainelement.Chain = this;
                position = position + DeltaPosition;
            }
        }

        public Vector3 GetPosition(float normalizedposition)
        {
            if (spline != null)
                return spline.GetPosition(normalizedposition);
            else
            {
                return Vector3.zero;
            }
        }

        public Vector3 GetTangent(float normalizedposition)
        {
            return spline.GetDirection(normalizedposition);
        }


        void Start()
        {
        }
    }
}