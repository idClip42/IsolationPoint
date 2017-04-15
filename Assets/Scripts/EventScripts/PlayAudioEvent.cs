using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioEvent : Event {

    AudioSource src;
    public AudioClip clip;  //clip to play
    public float delay = 0;
    public bool playOnStart = true;

	// Use this for initialization
	protected override void Start () {
        src = GetComponent<AudioSource>();
        if (playOnStart) PlayEvent();

        base.Start();
	}

    public override void PlayEvent()
    {
        base.PlayEvent();
        src.clip = clip;
        src.PlayDelayed(delay);
    }
}
