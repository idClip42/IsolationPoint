using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorOpenEvent : Event {

    public GameObject door1;
    public GameObject door2;
    DoorMovement door1Script;
    DoorMovement door2Script;

    // Use this for initialization
    public override void Start () {
        base.Start();
        door1Script = door1.GetComponent<DoorMovement>();
        door2Script = door2.GetComponent<DoorMovement>();
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
	}

    public override void PlayEvent()
    {
        if (door1Script != null) door1Script.SmashOpen();
        if (door2Script != null) door2Script.SmashOpen();
        base.PlayEvent();
    }
}
