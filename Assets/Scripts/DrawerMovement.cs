using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerMovement : MonoBehaviour, IInteractable {

    public bool isOpen;
    bool IsOpen
    {
        get { return isOpen; }
        set
        {
            isOpen = value;
            if (isOpen)
            {
                //set to open position
                transform.localPosition = startPos + axis * openDistance;
            }else
            {
                //set to closed position
                transform.localPosition = startPos;
            }
        }
    }
    public bool isLocked = false;
    bool moving;
    float timeLeft;
    float speed;
    AudioSource src;

    public AudioClip lockSound;
    public AudioClip openSound;

    public enum AxisRotation
    {
        XAxis,
        YAxis,
        ZAxis
    };
    public AxisRotation axisToRotateAbout = AxisRotation.YAxis;
    Vector3 axis;
    public float openTime = 0.5f;
    public float openDistance;
    Vector3 startPos;

    // Use this for initialization
    void Start () {
        src = GetComponent<AudioSource>();
        if (src != null) src.spatialBlend = 1.0f;
        startPos = transform.localPosition;
        switch (axisToRotateAbout)
        {
            case AxisRotation.XAxis:
                axis = new Vector3(1, 0, 0);
                break;
            case AxisRotation.YAxis:
                axis = new Vector3(0, 1, 0);
                break;
            case AxisRotation.ZAxis:
                axis = new Vector3(0, 0, 1);
                break;
            default:
                axis = new Vector3(0, 0, 0);
                break;
        }
        moving = false;
        timeLeft = 0;
        speed = 0;
        IsOpen = isOpen;
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
                IsOpen = !IsOpen;
                timeLeft = 0;
            }
            else
            {
                transform.localPosition = transform.localPosition + axis * speed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Open/Close the drawer.
    /// </summary>
    void Open()
    {
        moving = true;
        timeLeft = openTime - timeLeft;
        if (!IsOpen)
        {
            speed = GetSpeed(openTime);
        }
        else
        {
            speed = -GetSpeed(openTime);
        }
        if(src != null)
        {
            src.clip = openSound;
            src.Play();
        }
    }

    /// <summary>
    /// Get the speed at which the drawer will move.
    /// </summary>
    /// <param name="timeToOpen">Time it takes to fully open or close.</param>
    /// <returns>Speed to move.</returns>
    float GetSpeed(float timeToOpen)
    {
        return openDistance / timeToOpen;
    }

    public void Action()
    {
        if (isLocked)
        {
            if (lockSound == null || src == null) return;
            src.clip = lockSound;
            src.Play();
            return;
        }

        if (moving)
        {
            return;
        }

        Open();
    }

    public string ActionDescription()
    {
        if (isLocked)
        {
            return "Locked";
        }
        if (moving)
        {
            return " ";
        }
        if (IsOpen)
        {
            return "Close";
        }
        return "Open";
    }
}
