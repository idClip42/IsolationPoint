using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearTrap : MonoBehaviour, IInteractable {

    public bool isOpen = false;
    bool IsOpen
    {
        set
        {
            isOpen = value;
            if (isOpen)
            {
                tag = "Left_Object";//Left hand item, or should it be right?
                //col.isTrigger = true;
                //set rotation of teeth to 0
                if (LTeeth != null) LTeeth.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (RTeeth != null) RTeeth.transform.rotation = Quaternion.Euler(0, 0, 0);

                if(caughtEntity != null)
                {
                    if(caughtEntity.tag == "Player")
                    {
                        //free the player
                        caughtEntity.GetComponent<Follower>().IsWorking = false;
                        if (caughtEntity != PlayerController.controller.Player.gameObject)
                            caughtEntity.GetComponentInChildren<NavMeshAgent>().enabled = true;
                    }
                    else if(caughtEntity.tag == "Enemy")
                    {
                        //free the enemy
                        caughtEntity.GetComponent<Enemy>().Free();
                    }
                }
            }
            else
            {
                //col.isTrigger = false;
                tag = "BearTrap";
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
    public float damagePerSecond = 1.0f;        //Damage taken when in trap
    AudioSource src;
    float timer;
    float restTime;
    float sprayTimer;

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
    bool isResting;

    bool isCarried;
    bool IsCarried
    {
        set {
            isCarried = value;
            rb.isKinematic = isCarried;
            if (isCarried)
            {
                col.enabled = false;
            }
            else
            {
                col.enabled = true;
                transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
                isResting = true;
            }
        }
    }

    Follower worker;
    GameObject caughtEntity;
    public GameObject LTeeth;
    public GameObject RTeeth;
    public BoxCollider col;
    Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        isSetting = false;
        timer = 0.0f;
        restTime = 0.25f;
        sprayTimer = 5.0f;
        src = GetComponentInChildren<AudioSource>();
        rb = GetComponentInChildren<Rigidbody>();
        worker = null;
        caughtEntity = null;
        IsOpen = isOpen;
        isCarried = false;
        isResting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpen)
        {
            if (caughtEntity != null && caughtEntity != PlayerController.controller.Player.gameObject && caughtEntity.tag != "Enemy")
            {
                caughtEntity.GetComponentInChildren<NavMeshAgent>().enabled = false;
            }
            if (caughtEntity != null && caughtEntity.tag == "Player")
            {
                sprayTimer -= Time.deltaTime;
                bool spray = false;
                if(sprayTimer <= 0)
                {
                    spray = true;
                    sprayTimer = 5.0f;
                }
                // Gets the health script of the target
                Health healthScript = caughtEntity.GetComponent<Health>();
                Health_Part healthPartScript = caughtEntity.GetComponent<Health_Part>();
                // If there's no health scripts, returns
                if (healthScript == null && healthPartScript == null) return;

                // If colliding with another character, does damage
                Vector3 point = caughtEntity.GetComponentInChildren<Collider>().ClosestPointOnBounds(transform.position);
                Vector3 normal = Vector3.up;    // Perhaps this should be the dif between the closest point and the transform from above
                if (healthScript != null)
                    healthScript.Hit(damagePerSecond * Time.deltaTime, spray, point, normal, null);
                else if (healthPartScript != null)
                    healthPartScript.Hit(damagePerSecond * Time.deltaTime, spray, point, normal);
            }
        }

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
                isResting = true;
            }
        }
        else if (isResting)
        {
            timer += Time.deltaTime;
            if(timer >= restTime)
            {
                isResting = false;
                timer = 0;
                caughtEntity = null;
            }
        }
        //set rotation of teeth to 90 - (90 * timer / snapTime)
        else if (isSnapping)
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
        if (caughtEntity == PlayerController.controller.Player.gameObject)
        {
            return;
        }

        if (!isOpen && !isSetting && !isSnapping)
        {
            IsSetting = true;
            SetWorker();
            worker.IsWorking = true;
        }

        if(isOpen && !isSnapping)
        {
            IsCarried = true;
        }
    }

    public string ActionDescription()
    {
        if(caughtEntity == PlayerController.controller.Player.gameObject)
        {
            return " ";
        }

        if (!isOpen)
        {
            return "Set trap";
        }
        else if (isSetting)
        {
            return "Setting trap";
        }else if(isOpen && !isSnapping)
        {
            return " ";
        }

        return "Pick up";
    }

    void OnTriggerEnter(Collider c)
    {
        if (!isOpen || caughtEntity != null || isCarried || !col.enabled) return;
        timer = 0;

        // Gets the health script of the target
        Health healthScript = c.gameObject.GetComponent<Health>();
        Health_Part healthPartScript = c.gameObject.GetComponent<Health_Part>();
        // If there's no health scripts, returns
        if (healthScript == null && healthPartScript == null) return;

        if(c.gameObject.tag == "Player")
        {
            caughtEntity = c.gameObject;
            //should work for being stuck as well
            //don't cause damage -- used when freeing a character so they dont immediately get caught again
            if (isResting)
            {
                return;
            }
            caughtEntity.GetComponent<Follower>().IsWorking = true;
            if (caughtEntity != PlayerController.controller.Player.gameObject)
                caughtEntity.GetComponentInChildren<NavMeshAgent>().enabled = false;
        }
        else if(c.gameObject.tag == "Enemy")
        {
            caughtEntity = c.gameObject;
            //start the enemy's timer to break free
            //don't cause damage -- used when freeing a character so they dont immediately get caught again
            if (isResting)
            {
                return;
            }
            caughtEntity.GetComponent<Enemy>().StartCaughtTimer();
        }


        isSnapping = true;
        //play snap shut sound
        if (src != null && src.clip != null) src.PlayOneShot(src.clip);


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



    public void PickUpPutDown(bool pickUp, CharacterController c)
    {
        IsCarried = pickUp;
        if (pickUp)
        {
            col.gameObject.layer = 8;
        }
        else {
            col.gameObject.layer = 0;
        }
    }
}
