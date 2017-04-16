using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorMovement : MonoBehaviour, IInteractable {

    float openRot;

    bool isOpen;
    public bool IsOpen
    {
        get { return isOpen; }
        set {
            isOpen = value;
            if (isOpen)
            {
                navOb.enabled = false;
            }
            else
            {
                navOb.enabled = true;
            }
        }
    }

    public bool isLocked = false;
    public bool IsLocked
    {
        get { return isLocked; }
        set {
            isLocked = value;
            if (isLocked)
            {
                navOb.carving = true;
            }
            else
            {
                navOb.carving = false;
            }
        }
    }

    bool moving;
    float timeLeft;
    float speed;
    Quaternion startRot;
    NavMeshObstacle navOb;
    AudioSource src;
    Transform doorCenter;

    public AudioClip lockSound;
    public AudioClip slamOpenSound;

    public float openTime = 5.0f;
    public float fastOpenTime = 1.5f;

    // Use this for initialization
    void Start () {
        src = GetComponent<AudioSource>();
		src.spatialBlend = 1.0f;
        navOb = GetComponent<NavMeshObstacle>();
        doorCenter = transform.FindChild("DoorCenter");
        startRot = transform.rotation;
        if (transform.localScale.x < 0)
        {
            openRot = -90.0f;
        }
        else {
            openRot = 90.0f;
        }
        IsOpen = false;
        IsLocked = isLocked;    //should set the correct nav obstacle settings
        moving = false;
        timeLeft = 0;
        speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (moving)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                speed = 0;
                moving = false;
                timeLeft = 0;
                if (!IsOpen)
                {
                    transform.rotation = startRot;
                }
            }
            else
            {
                transform.Rotate(0, speed * Time.deltaTime, 0);
            }
        }

        CheckEnemyCollision();
    }

    /// <summary>
    /// What to do when 'colliding' with an enemy
    /// </summary>
    void CheckEnemyCollision()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            RaycastHit hit;
            Physics.Raycast(g.transform.position + new Vector3(0, 1, 0), doorCenter.position - (g.transform.position + new Vector3(0, 1, 0)), out hit, g.GetComponent<NavMeshAgent>().radius + 1);
            //Debug.DrawRay(doorCenter.position, Vector3.Normalize(g.transform.position + new Vector3(0, 1, 0) - doorCenter.position) * 1.5f, Color.white);
            if (hit.transform == g.transform)
            {
                //Debug.Log("Open sesame");
                SmashOpen();
            }else if(hit.transform == transform)
            {
                //Debug.Log("Grr");
                SmashOpen();
            }
            else
            {
                //Debug.Log(hit.transform);
            }
        }
    }

    /// <summary>
    /// Quickly open the door
    /// </summary>
    public void SmashOpen()
    {
        if (!IsOpen && !isLocked)
        {
            timeLeft = fastOpenTime - (timeLeft / openTime) * fastOpenTime;
            moving = true;
            speed = GetRotSpeed(fastOpenTime);
            IsOpen = true;
            if(slamOpenSound != null)
            {
                src.clip = slamOpenSound;
                src.Play();
            }
        }
    }

    /// <summary>
    /// Quickly close the door
    /// </summary>
    public void SmashClose()
    {
        if (IsOpen && !IsLocked)
        {
            timeLeft = fastOpenTime - (timeLeft / openTime) * fastOpenTime;
            moving = true;
            speed = -GetRotSpeed(fastOpenTime);
            IsOpen = false;
            if (slamOpenSound != null)
            {
                src.clip = slamOpenSound;
                src.Play();
            }
        }
    }

    /// <summary>
    /// Open the door
    /// </summary>
    void Open()
    {
        moving = true;
        timeLeft = openTime - timeLeft;
        if (!IsOpen)
        {
            speed = GetRotSpeed(openTime);
        }
        else
        {
            speed = -GetRotSpeed(openTime);
        }
        IsOpen = !IsOpen;
    }

    /// <summary>
    /// Get the speed at which the door rotates per second and multiply by deltaTime
    /// </summary>
    /// <param name="timeToOpen">Time it takes to open door fully</param>
    /// <returns>Speed of rotation</returns>
    float GetRotSpeed(float timeToOpen)
    {
        return openRot / timeToOpen;
    }

    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    public void Action()
    {
        if (isLocked)
        {
            if (lockSound == null) return;
            src.clip = lockSound;
            src.Play();
            return;
        }

        //For double key press - 'E' and 'LShift'
        if (Input.GetButton("Run"))
        {
            if (IsOpen)
            {
                SmashClose();
            }
            else
            {
                SmashOpen();
            }
        }
        else
        {
            Open();
        }
    }

    public string ActionDescription()
    {
        if (IsLocked)
        {
            return "Locked";
        }
        if (IsOpen)
        {
            return "Close";
        }
        return "Open";
    }

}
