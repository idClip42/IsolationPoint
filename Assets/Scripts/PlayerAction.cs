using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put other actions for other objects here?
/// </summary>
public class PlayerAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Action"))
        {
            GameObject door = CheckForDoor();
            if (door != null)
            {
                door.GetComponent<DoorMovement>().Open();
            }
        }
    }

    /// <summary>
    /// Check if the gameobject is a door.
    /// </summary>
    /// <returns>Returns the object if it is a door.</returns>
    GameObject CheckForDoor()
    {
        // creates ray at mouse position
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        // Raycasts from main camera forward vector and returns true if the item is within 2 units and has correct tag
        if (Physics.Raycast(Camera.main.transform.position, forward, out hit, 5))
        {
            if (hit.transform.tag == "Door")
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
}
