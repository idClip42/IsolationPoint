using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Meant to be attached to a fullscreen image in the UI
public class ScreenFadeEvent : Event {

    Image img;
    public float alpha = 1.0f;

	// Use this for initialization
	protected override void Start () {
        //black screen
        Color c = Color.black;
        c.a = 1.0f;
        //get image
        img = GetComponent<Image>();
        img.color = c;
        //fade out img on start of game
        Fade(alpha, timeToComplete);

        base.Start();
	}

    void Fade(float alpha, float time)
    {
        img.CrossFadeAlpha(alpha, time, false);
    }

    public override void PlayEvent()
    {
        base.PlayEvent();
        Fade(alpha, timeToComplete);
    }
}
