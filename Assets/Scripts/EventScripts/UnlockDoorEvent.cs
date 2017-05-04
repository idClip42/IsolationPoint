using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorEvent : Event {

    public GameObject[] doors;
    public bool unlock = true;

    public override void PlayEvent()
    {
        foreach(GameObject g in doors)
        {
            DoorMovement d = g.GetComponentInChildren<DoorMovement>();
            d.IsLocked = !unlock;
        }
        base.PlayEvent();
    }
}
