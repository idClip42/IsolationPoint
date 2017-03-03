using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorMovement : MonoBehaviour {

    float openRot;
    bool isOpen;
    bool moving;
    float timeLeft;
    float speed;

    public float openTime = 5.0f;
    public float fastOpenTime = 1.5f;

    // Use this for initialization
    void Start () {
        openRot = 90.0f;
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
            }
            else
            {
                transform.Rotate(0, speed, 0);
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
            if (hit.transform == this.transform)
            {
                SmashOpen();
            }
        }
    }

    /// <summary>
    /// Quickly open the door
    /// </summary>
    void SmashOpen()
    {
        if (!isOpen)
        {
            timeLeft = fastOpenTime - timeLeft;
            moving = true;
            speed = GetRotSpeed(fastOpenTime);
            isOpen = true;
        }
    }

    /// <summary>
    /// Open door
    /// </summary>
    public void Open()
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
        return openRot / timeToOpen * Time.deltaTime;
    }
}
