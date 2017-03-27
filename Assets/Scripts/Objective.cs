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

    bool isCompleted = false;
    public bool IsCompleted {
        get { return isCompleted; }
        set {
            isCompleted = value;
            if (IsCompleted)
            {
                NextObjective();
            }
        }
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
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
                isCompleted = true;
                NextObjective();
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
                isCompleted = true;
                NextObjective();
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
            GameObject gm = GameObject.Find("GM");
            gm.GetComponent<GameManager>().NextObjective();
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
                parent.NextObjective();
            }
        }
    }
}
