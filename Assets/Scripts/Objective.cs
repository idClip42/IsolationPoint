using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {

    public string UIText;   //text shown to the player
    public bool isTriggered;    //can be completed by trigger on collider (ie. go to location)
    public bool onEnter;    //trigger complete upon enter or exit?
    //public bool isInteractable; //can be completed by interaction
    public string[] triggerTags;    //tags that will trigger completion

    public GameObject parent;   //main objective parent = gm
                                //could be another objective -> subobjectives
    public GameObject[] subObjectives;  //objects containing subobjectives -> act like objectives

    //objectives completed by interaction must be set complete in the interaction script and then call NextObjective()?
    bool isCompleted = false;
    public bool IsCompleted {
        get { return isCompleted; }
        set { isCompleted = value; }
    }

	// Use this for initialization
	void Start () {
        if(!parent.GetComponent<Objective>() && parent.name != "GM")
        {
            Debug.LogError("Invalid parent on objective");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        if (isTriggered && onEnter)
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
            CheckSubobjectives(gameObject);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (isTriggered && !onEnter)
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
            CheckSubobjectives(gameObject);
        }
    }

    /// <summary>
    /// Check for the completion of all subobjects in the parent or get the next objective.
    /// </summary>
    void NextObjective()
    {
        //next objective in gm list
        if (parent.name == "GM" && IsCompleted)
        {
            parent.GetComponent<GameManager>().NextObjective();
            return;
        }

        //for subobjectives
        CheckSubobjectives(parent);

    }

    /// <summary>
    /// Check if the parent of this has completed all its subobjectives. If so, get the next objective.
    /// </summary>
    void CheckSubobjectives(GameObject parent)
    {
        if (parent.GetComponent<Objective>().subObjectives.Length > 0)
        {
            int subsCompleted = 0;  //number of subobjectives completed
            foreach (GameObject g in parent.GetComponent<Objective>().subObjectives)
            {
                //check each sub in parent for completion
                if (g.GetComponent<Objective>().IsCompleted) subsCompleted++;
            }

            //if all subs are completed
            if (subsCompleted == parent.GetComponent<Objective>().subObjectives.Length)
            {
                //triggered ones must be completed by a trigger, not subs
                if (isTriggered)
                {
                    return;
                }

                //if not triggered
                parent.GetComponent<Objective>().IsCompleted = true;
                parent.GetComponent<Objective>().NextObjective();
            }
        }
    }
}
