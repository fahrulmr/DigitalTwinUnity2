
using UnityEngine;
using game4automation;


public class DemoRDKControl : MonoBehaviour
{
    public Sensor SensorEntry;
    public Drive DriveEntry;
    public Grip RobotGripper;
    public Sensor SensorExit;
    public Drive DriveExit;

    public PLCInputInt MoveToPick;
    public PLCInputInt MoveToPlace;
    public PLCInputInt BoxIsPlaced;
    public PLCOutputInt IsAtPick;
    public PLCOutputInt IsAtPlace;

    private int Step = 0;
    private float timeplace;
  
    // Start is called before the first frame update
    void Start()
    {
        MoveToPlace.Value = 0;
        MoveToPick.Value = 0;
        BoxIsPlaced.Value = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SensorEntry.Occupied)
            DriveEntry.JogForward = false;
        else
            DriveEntry.JogForward = true;

        if (Step == 0 && SensorEntry.Occupied)
        {
            MoveToPick.Value = 1;
            Step++;
        }

        if (Step == 1 && IsAtPick.Value==1)
        {
            RobotGripper.PlaceObjects = false;
            RobotGripper.PickObjects = true;
            MoveToPlace.Value = 1;
            MoveToPick.Value = 0;
            BoxIsPlaced.Value = 0;
            Step++;
            DriveExit.JogForward = false;
        }

        if (Step == 2 && IsAtPlace.Value == 1)
        { 
      
            MoveToPlace.Value = 0;
            RobotGripper.PickObjects = false;
            RobotGripper.PlaceObjects = true;
            timeplace = Time.time;
            Step++;
        }

        // Wait a little bit for releasing
        if (Step == 3 && Time.time > timeplace + 1)
        {
            Step++;
        }
        
        if (Step ==4 && IsAtPlace.Value == 1 && RobotGripper.PickedMUs.Count == 0)
        {
            DriveExit.JogForward = true;
            BoxIsPlaced.Value = 1;
            Step = 0;
        }
            
    }
}
