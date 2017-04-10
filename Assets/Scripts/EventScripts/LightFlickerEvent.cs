using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerEvent : Event {
    public Generator generator;//because there could be multiple but this only controls one
    float flickerTime;
    public bool stayOn = false; //Will the lights still be on when completed? Assumed starts on.

    // Use this for initialization
    protected override void Start () {
        base.Start();
        flickerTime = 4.0f;
        timeToComplete = flickerTime;
	}

    // Update is called once per frame
    protected override void Update () {
        if (!IsPlaying) return;
        base.Update();

        //y=-x/3 + 0.5sin(.2(x+12.55)^2)+1  y=light intensity x=time 
        float intensity = (-(flickerTime - timeToComplete) / 3) + 0.5f * Mathf.Sin(0.2f * ((flickerTime - timeToComplete + 12.55f) * (flickerTime - timeToComplete + 12.55f))) + 1;
        if (IsFinished)
        {
            intensity = 2.0f;
            if (!stayOn)
            {
                generator.SwitchLights(false);
                //return;
            }
        }

        //change light intensity
        foreach (GameObject l in generator.AllLights)
        {
            l.GetComponentInChildren<Light>().intensity = intensity;
        }
    }

    public override void PlayEvent()
    {
        base.PlayEvent();
    }
}
