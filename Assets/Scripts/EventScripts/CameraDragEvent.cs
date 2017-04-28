using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In progress
public class CameraDragEvent : Event {
    public float dragTime = 1.0f;   //Time it takes to drag the camera to the desired angle
    public float stareTime = 5.0f;  //Time the camera is staring at the desired location
    public Vector3 location;    //Location to stare at
    bool restrictPlayerControl = true;  //Prevent the player from moving their character or camera?
    float maxRadiansPerSecond;
    float moveSpeed;
    Vector3 moveDir;
    Camera cam; //need to get camera
    public Transform moveLocation;
    public Transform target;    //used in preference of location, for moving targets (ie. helicopter)

    // Use this for initialization
    protected override void Start () {
        maxRadiansPerSecond = 0;
        //override from Event
        timeToComplete = dragTime + stareTime;

        moveDir = Vector3.zero;

        // Finds the Main Camera
        cam = Camera.main;
        if (cam == null) Debug.LogError("Must have main camera tagged 'MainCamera'");
        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {
        if (!IsPlaying) return;
        base.Update();
        if (IsFinished && restrictPlayerControl) gm.PauseInput = false;
        if (target)
            location = target.position;
        //if (timeToComplete <= stareTime) return;

        if (moveLocation)
        {
            cam.transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        //rotate to face the location -- should not rotate when facing the location
        Vector3 rotAngle = Vector3.RotateTowards(cam.transform.forward, location - cam.transform.position, maxRadiansPerSecond * Time.deltaTime, 1);
        cam.transform.rotation = Quaternion.LookRotation(rotAngle);

    }

    public override void PlayEvent()
    {
        if (target)
            location = target.position;
        //restrict input if wanted
        gm.PauseInput = restrictPlayerControl;
        //calculate the radians to turn in the specified time
        float angle = Vector3.Angle(cam.transform.forward, location - cam.transform.position);
        maxRadiansPerSecond = Mathf.Deg2Rad * angle / dragTime;

        if (moveLocation)
        {
            moveSpeed = Mathf.Abs(Vector3.Distance(moveLocation.position, cam.transform.position) / dragTime);
            moveDir = moveLocation.position - cam.transform.position;
            moveDir = Vector3.Normalize(moveDir);
        }
        base.PlayEvent();
    }
}
