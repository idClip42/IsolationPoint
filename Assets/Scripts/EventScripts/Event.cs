using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {
    GameManager gm;
    public Event[] simultaneousEvents;  //These will run at the same time as the main event
    public float timeToComplete = 10.0f;

    bool isPlaying = false;
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    bool isFinished = false;
    public bool IsFinished
    {
        get { return isFinished; }
        set { isFinished = value; }
    }

    bool allFinished = false;


	// Use this for initialization
	public virtual void Start () {
        gm = GameObject.Find("GM").GetComponent<GameManager>();
    }

    // Update is called once per frame
    public virtual void Update () {
        if (isPlaying)
        {
            timeToComplete -= Time.deltaTime;
            if(timeToComplete <= 0 && allFinished)
            {
                isPlaying = false;
                isFinished = true;
            }
            CheckArrayEvents();
            gm.IsPlayingEvent = isPlaying;
        }
	}

    public virtual void PlayEvent()
    {
        isPlaying = true;
        foreach(Event e in simultaneousEvents)
        {
            e.PlayEvent();
        }
    }

    /// <summary>
    /// Check if the all the events associated with this are complete.
    /// </summary>
    void CheckArrayEvents()
    {
        if (isFinished) return;
        int fin = 0;
        foreach (Event e in simultaneousEvents)
        {
            if (e.IsFinished)
            {
                fin++;
            }
        }
        if (fin == simultaneousEvents.Length) allFinished = true;
    }
}
