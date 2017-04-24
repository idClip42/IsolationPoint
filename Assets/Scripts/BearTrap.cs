using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour, IInteractable {

    public bool isOpen = false;
    bool IsOpen
    {
        set
        {
            isOpen = value;
            if (isOpen)
            {
                col.isTrigger = true;
                //set rotation of teeth to 0
                if (LTeeth != null) LTeeth.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (RTeeth != null) RTeeth.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                col.isTrigger = false;
                //set rot of teeth to 90
                if (LTeeth != null) LTeeth.transform.rotation = Quaternion.Euler(90, 0, 0);
                if (RTeeth != null) RTeeth.transform.rotation = Quaternion.Euler(-90, 0, 0);
            }
        }
    }
    public float setTime = 5.0f;
    public float snapTime = 0.5f;
    public float damage = 50.0f;                //Change this -- I dont do balance D:
    public bool drawBlood = true;				// Whether the trap draws blood
    AudioSource src;
    float timer;

    bool isSetting;
    bool IsSetting
    {
        set
        {
            isSetting = value;
            if (!isSetting)
            {
                worker.IsWorking = false;
                worker = null;
                if(timer >= setTime)//needed here too in case interrupted
                {
                    IsOpen = true;
                }
            }
        }
    }

    bool isSnapping;

    Follower worker;
    public GameObject LTeeth;
    public GameObject RTeeth;
    BoxCollider col;

    // Use this for initialization
    void Start()
    {
        isSetting = false;
        timer = 0.0f;
        src = GetComponentInChildren<AudioSource>();
        col = GetComponent<BoxCollider>();
        IsOpen = isOpen;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSetting)
        {
            timer += Time.deltaTime;
            //set rotation of teeth to 90 - (90 * timer / setTime)
            //should be 90 on start (closed) and 0 on finish (open and flat)
            if (LTeeth != null) LTeeth.transform.rotation = Quaternion.Euler(90 - (90 * timer / setTime), 0, 0);
            if (RTeeth != null) RTeeth.transform.rotation = Quaternion.Euler(-90 + (90 * timer / setTime), 0, 0);
            if (timer >= setTime)
            {
                IsSetting = false;
                timer = 0;
            }
        }

        //set rotation of teeth to 90 - (90 * timer / snapTime)
        if (isSnapping)
        {
            timer += Time.deltaTime;
            if (LTeeth != null) LTeeth.transform.rotation = Quaternion.Euler(90 - (90 * timer / snapTime), 0, 0);
            if (RTeeth != null) RTeeth.transform.rotation = Quaternion.Euler(-90 + (90 * timer / snapTime), 0, 0);
            if (timer >= snapTime)
            {
                isSnapping = false;
                timer = 0;
                IsOpen = false;
            }
        }


        //Should stop work if the character dies or is dead
        if(worker != null)
        {
            if(worker.gameObject.GetComponentInChildren<Health>().health <= 0)
            {
                IsSetting = false;
            }
        }
    }

    public void Action()
    {
        if (!isOpen)
        {
            IsSetting = true;
            SetWorker();
            worker.IsWorking = true;
        }
    }

    public string ActionDescription()
    {
        if (!isOpen)
        {
            return "Set trap";
        }
        else if (isSetting)
        {
            return "Setting trap";
        }

        return " ";
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isOpen) return;
        isSnapping = true;
        timer = 0;

        //play snap shut sound
        if (src != null) src.PlayOneShot(src.clip);

        // Gets the health script of the target
        Health healthScript = c.gameObject.GetComponent<Health>();
        Health_Part healthPartScript = c.gameObject.GetComponent<Health_Part>();
        // If there's no health scripts, returns
        if (healthScript == null && healthPartScript == null) return;

        // If colliding with another character, does damage
        Vector3 point = c.ClosestPointOnBounds(transform.position);
        Vector3 normal = Vector3.up;    // Perhaps this should be the dif between the closest point and the transform from above
        if (healthScript != null)
            healthScript.Hit(damage, drawBlood, point, normal, null);
        else if (healthPartScript != null)
            healthPartScript.Hit(damage, drawBlood, point, normal);
    }

    /// <summary>
    /// Set the character who is using this as the worker.
    /// </summary>
    void SetWorker()
    {
        worker = PlayerController.controller.Player.gameObject.GetComponent<Follower>();
    }
}
