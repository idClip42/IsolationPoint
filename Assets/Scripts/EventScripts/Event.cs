using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {
    protected GameManager gm;
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
	protected virtual void Start () {
        gm = GameObject.Find("GM").GetComponent<GameManager>();
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (!isPlaying) return;

        timeToComplete -= Time.deltaTime;
        if (timeToComplete <= 0)
        {
            isFinished = true;
        }
        CheckArrayEvents();
        if (allFinished) isPlaying = false;
        gm.IsPlayingEvent = isPlaying;
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
        int fin = 0;
        if (isFinished) fin++;
        foreach (Event e in simultaneousEvents)
        {
            if (e.IsFinished)
            {
                fin++;
            }
        }
        if (fin == simultaneousEvents.Length + 1) allFinished = true;
    }
}
