using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHelicopterEvent : Event {

    public Helicopter heli;

	// Use this for initialization
	protected override void Start () {
        if (!heli)
            Debug.LogError("Needs a helicopter to trigger");

        base.Start();
	}
	

    public override void PlayEvent()
    {
        heli.TriggerHelicopter();
        base.PlayEvent();
    }
}
