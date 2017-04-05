﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoorOpenEvent : Event {

    public GameObject door1;
    public GameObject door2;
    DoorMovement door1Script;
    DoorMovement door2Script;
    public bool toOpen = true;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        door1Script = door1.GetComponent<DoorMovement>();
        door2Script = door2.GetComponent<DoorMovement>();
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public override void PlayEvent()
    {
        if (door1Script != null) {
            if (toOpen)
            {
                door1Script.SmashOpen();
            } else {
                door1Script.SmashClose();
            }
        }
        if (door2Script != null)
        {
            if (toOpen)
            {
                door2Script.SmashOpen();
            }
            else
            {
                door2Script.SmashClose();
            }
        }
        base.PlayEvent();
    }
}
