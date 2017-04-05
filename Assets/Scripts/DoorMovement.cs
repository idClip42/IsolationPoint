using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorMovement : MonoBehaviour, IInteractable {

    float openRot;
    bool isOpen;
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
    public AudioClip lockSound;
    public AudioClip slamOpenSound;

    public float openTime = 5.0f;
    public float fastOpenTime = 1.5f;

    // Use this for initialization
    void Start () {
        src = GetComponent<AudioSource>();
        navOb = GetComponent<NavMeshObstacle>();
        startRot = transform.rotation;
        if (transform.localScale.x < 0)
        {
            openRot = -90.0f;
        }
        else {
            openRot = 90.0f;
        }
        IsLocked = isLocked;    //should set the correct nav obstacle settings
        isOpen = false;
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
                if (!isOpen)
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
            Physics.Raycast(g.transform.position, Vector3.Normalize(transform.position - g.transform.position), out hit, g.GetComponent<NavMeshAgent>().radius);
            if (hit.transform == transform)
            {
                SmashOpen();
            }
        }
    }

    /// <summary>
    /// Quickly open the door
    /// </summary>
    public void SmashOpen()
    {
        if (!isOpen && !isLocked)
        {
            timeLeft = fastOpenTime - (timeLeft / openTime) * fastOpenTime;
            moving = true;
            speed = GetRotSpeed(fastOpenTime);
            isOpen = true;
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
        if (isOpen && !isLocked)
        {
            timeLeft = fastOpenTime - (timeLeft / openTime) * fastOpenTime;
            moving = true;
            speed = -GetRotSpeed(fastOpenTime);
            isOpen = false;
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
        Open();
    }

    public string ActionDescription()
    {
        if (IsLocked)
        {
            return "Locked";
        }
        if (isOpen)
        {
            return "Close";
        }
        return "Open";
    }
}
