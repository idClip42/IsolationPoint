using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : Event {
    public Generator generator;//because there could be multiple but this only controls one

	// Use this for initialization
	public override void Start () {
        base.Start();
	}

    // Update is called once per frame
    public override void Update () {
        base.Update();
	}

    public override void PlayEvent()
    {
        base.PlayEvent();
    }
}
