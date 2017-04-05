using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//In progress
public class CameraDragEvent : Event {
    public float dragTime = 1.0f;   //Time it takes to drag the camera to the desired angle
    public float stareTime = 5.0f;  //Time the camera is staring at the desired location
    public Vector3 location;    //Location to stare at
    public bool restrictPlayerControl = false;  //Prevent the player from moving their character or camera?
    //float maxRadiansPerSecond;
    float horiz = 0.0f;
    float vert = 0.0f;
    Camera cam; //need to get camera

    // Use this for initialization
    protected override void Start () {
        base.Start();
        //maxRadiansPerSecond = 0;
        //override from Event
        timeToComplete = dragTime + stareTime;

        // Finds the Main Camera
        cam = Camera.main;
        if (cam == null) Debug.LogError("Must have main camera tagged 'MainCamera'");
    }

    // Update is called once per frame
    protected override void Update () {
        if (!IsPlaying) return;
        base.Update();
        if (IsFinished && restrictPlayerControl) gm.PauseInput = false;
        if (timeToComplete <= stareTime) return;

        //rotate to face the location -- should not rotate when facing the location
        Debug.Log(cam.transform.forward);
        Vector3 rot = cam.transform.rotation.eulerAngles;
        rot += new Vector3(horiz*Time.deltaTime, vert*Time.deltaTime, 0);
        rot.z = 0;
        cam.transform.rotation = Quaternion.Euler(rot);
        //Vector3 rotAngle = Vector3.RotateTowards(cam.transform.forward, location - cam.transform.position, maxRadiansPerSecond * Time.deltaTime, 0);
        //cam.transform.rotation = Quaternion.Euler(rotAngle);
        //Debug.Log(rotAngle);
        //cam.transform.LookAt(rotAngle, transform.up);
        Debug.Log(cam.transform.forward);
    }

    public override void PlayEvent()
    {
        //restrict input if wanted
        gm.PauseInput = restrictPlayerControl;
        //calculate the radians to turn in the specified time
        //float angle;// = Vector3.Angle(cam.transform.forward, location - cam.transform.position);
        //maxRadiansPerSecond = angle * Mathf.PI / (180 * dragTime);
        //maxRadiansPerSecond = angle / dragTime;
        Vector3 loc = location - cam.transform.position;
        float angle = Vector3.Angle(cam.transform.forward, loc) / dragTime;
        Vector3 locX = loc;
        locX.y = locX.z = 0;
        Vector3 locY = loc;
        locY.x = locY.z = 0;
        Debug.Log(angle);
        horiz = Vector3.Angle(cam.transform.forward, locX) / dragTime;
        Debug.Log(horiz);
        vert = Vector3.Angle(cam.transform.forward, locY) / dragTime;
        Debug.Log(vert);
        base.PlayEvent();
    }
}
