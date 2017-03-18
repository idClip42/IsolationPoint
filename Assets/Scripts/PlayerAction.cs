using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put other actions for other objects here?
/// </summary>
public class PlayerAction : MonoBehaviour {

    //contains the tag and associated script that holds the interaction action for the object
    Dictionary<string, System.Type> componentDict = new Dictionary<string, System.Type>();

	// Use this for initialization
	void Start () {
        //add tag and associated script type to dictionary
        componentDict.Add("Door", typeof(DoorMovement));
        componentDict.Add("Timeskipper", typeof(DaySkip));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Action"))
        {
            GameObject obj = CheckForTag();
            if (obj != null)
            {
                //door.GetComponent<DoorMovement>().Action();
                System.Type t = componentDict[obj.tag];
                if(obj.GetComponent(t) is IInteractable)
                {
                    (obj.GetComponent(t) as IInteractable).Action();
                }
            }
        }
    }



    /// <summary>
    /// Check if the gameobject is a door.
    /// </summary>
    /// <returns>Returns the object if it is a door.</returns>
    GameObject CheckForTag()
    {
        // creates ray at mouse position
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        // Raycasts from main camera forward vector and returns true if the item is within 2 units and has correct tag
        if (Physics.Raycast(Camera.main.transform.position, forward, out hit, 5))
        {
            /*
            if (hit.transform.tag == "Door")
            {
                return hit.transform.gameObject;
            }
            */
            foreach(string key in componentDict.Keys)
            {
                if(hit.transform.tag == key)
                {
                    return hit.transform.gameObject;
                }
            }
        }
        return null;
    }
}
