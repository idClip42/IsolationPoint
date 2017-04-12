using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    public string UIText;   //text shown to the player
    public bool isTriggered;    //can be completed by trigger on collider (ie. go to location)
    public bool onEnter;    //trigger complete upon enter or exit?
    //public bool isInteractable; //can be completed by interaction
    public string[] triggerTags;    //tags that will trigger completion

    public Objective parent;   //main objective parent, if null find gm
                                //could be another objective -> subobjectives
    public Objective[] subObjectives;  //subobjectives -> act like objectives
    public Event[] events;      //events to play on completion

    bool isCompleted = false;
    public bool IsCompleted {
        get { return isCompleted; }
        set {
            if (!enabled) return;
            isCompleted = value;
            if (IsCompleted)
            {
                isPlaying = true;
                if (parent == null) gm.IsPlayingEvent = IsPlaying;
                StartEvents();
                NextObjective();
            }
        }
    }

    GameManager gm;
    bool allFinished = false;
    public bool AllFinished
    {
        set
        {
            allFinished = value;
            if (allFinished)
            {
                isPlaying = false;
                if (parent == null)
                {
                    gm.IsPlayingEvent = IsPlaying;
                }
                enabled = false;
            }
        }
    }
    bool isPlaying = false;
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    // Use this for initialization
    void Start()
    {
        gm = GameObject.Find("GM").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        //for events
        if (!IsPlaying) return;

        CheckArrayEvents();
    }

    void OnTriggerEnter(Collider c)
    {
        if (isTriggered && onEnter && isActiveAndEnabled)
        {
            bool tagged = false;    //correct tag to trigger?
            foreach (string tag in triggerTags)
            {
                if (tag == c.tag)
                {
                    tagged = true;
                    continue;
                }
            }

            //exit if not the correct tag
            if (!tagged)
            {
                return;
            }


            if (subObjectives.Length == 0)
            {
                IsCompleted = true;
                //NextObjective();
                return;
            }

            //check for completed subobjectives
            CheckSubobjectives(this);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (isTriggered && !onEnter && isActiveAndEnabled)
        {
            bool tagged = false;    //correct tag to trigger?
            foreach(string tag in triggerTags)
            {
                if(tag == c.tag)
                {
                    tagged = true;
                    continue;
                }
            }

            //exit if not the correct tag
            if (!tagged)
            {
                return;
            }


            if (subObjectives.Length == 0)
            {
                IsCompleted = true;
                //NextObjective();
                return;
            }

            //check for completed subobjectives
            CheckSubobjectives(this);
        }
    }

    /// <summary>
    /// Check for the completion of all subobjects in the parent or get the next objective.
    /// </summary>
    void NextObjective()
    {
        if (!isActiveAndEnabled) return;

        //next objective in gm list
        if (parent == null && IsCompleted)
        {
            gm.PlayEvent(); //used to fade out objective text
            return;
        }

        //for subobjectives
        CheckSubobjectives(parent);

    }

    /// <summary>
    /// Check if the parent of this has completed all its subobjectives. If so, get the next objective.
    /// </summary>
    void CheckSubobjectives(Objective parent)
    {
        if (!isActiveAndEnabled) return;

        if (parent.subObjectives.Length > 0)
        {
            int subsCompleted = 0;  //number of subobjectives completed
            foreach (Objective g in parent.subObjectives)
            {
                //check each sub in parent for completion
                if (g.IsCompleted) subsCompleted++;
            }

            //if all subs are completed
            if (subsCompleted == parent.subObjectives.Length)
            {
                parent.IsCompleted = true;
            }
        }
    }

    /// <summary>
    /// Check if the all the events associated with this are complete.
    /// </summary>
    void CheckArrayEvents()
    {
        if(events.Length == 0)
        {
            AllFinished = true;
            return;
        }

        int fin = 0;
        //if (isFinished) fin++;
        foreach (Event e in events)
        {
            if (e.IsFinished)
            {
                fin++;
            }
        }
        if (fin == events.Length) AllFinished = true;
    }

    /// <summary>
    /// Start all the events associated with this.
    /// </summary>
    void StartEvents()
    {
        foreach(Event e in events)
        {
            if (e != null) e.PlayEvent();
        }
    }
}
