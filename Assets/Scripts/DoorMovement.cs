using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour {

    float openRot;
    bool isOpen;
    Collider box;
    bool moving;
    float timeLeft;
    float speed;

    public float openTime = 5.0f;
    public float fastOpenTime = 1.5f;
    public KeyCode key = KeyCode.E;

    // Use this for initialization
    void Start () {
        openRot = 90.0f;
        isOpen = false;
        box = GetComponent<Collider>();
        moving = false;
        timeLeft = 0;
        speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(key))
        {
            Open();
            GameObject door = CheckForDoor();
            if (door != null)
            {
                //door.GetComponent<DoorMovement>().Open();
            }
        }

        if (moving)
        {
            timeLeft -= Time.deltaTime;
            transform.Rotate(0, speed, 0);
            if (timeLeft <= 0)
            {
                speed = 0;
                moving = false;
            }
        }
    }
    /// <summary>
    /// What to do when colliding
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            SmashOpen();
        }
    }

    /// <summary>
    /// Quickly open the door
    /// </summary>
    void SmashOpen()
    {
        timeLeft = fastOpenTime;
        if (!isOpen)
        {
            moving = true;
            speed = GetRotSpeed(fastOpenTime);
        }
        isOpen = true;
    }

    /// <summary>
    /// Open door
    /// </summary>
    void Open()
    {
        moving = true;
        timeLeft = openTime;
        if (!isOpen)
        {
            speed = GetRotSpeed(openTime);
        }
        else
        {
            speed = -GetRotSpeed(openTime);
        }
        isOpen = !isOpen;
    }

    /// <summary>
    /// Get the speed at which the door rotates per second and multiply by deltaTime
    /// </summary>
    /// <param name="timeToOpen">Time it takes to open door fully</param>
    /// <returns>Speed of rotation</returns>
    float GetRotSpeed(float timeToOpen)
    {
        return openRot / timeToOpen * Time.deltaTime;
    }

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
