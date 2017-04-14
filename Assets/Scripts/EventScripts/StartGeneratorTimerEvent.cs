using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGeneratorTimerEvent : Event{

    public Generator generator;//because there could be multiple but this only controls one

    public override void PlayEvent()
    {
        generator.startTimer = true;
        base.PlayEvent();
    }
}
