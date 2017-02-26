﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {

    public GameManager gm;              //for general game info such as players and their locations

    public float walkSpeed;             // The speed at which the enemy walks/wanders,
    public float runSpeed;              // runs/sprints/chases,
    public float searchSpeed;           // and searches,
    //public float acceleration;          // as well as their rate of acceleration -> redundant if never changes b/c agent has same field

    public float angleOfVision;         //Cone representing field of view
    public float visionDistance;        //Distance the enemy can see at
    public float searchDuration;        //How long the enemy searches for players near the last seen location
    public float timeBetweenAttacks;    //Delay between attack sequences
    public float attackSpeed;           //How long does it take for attack sequence to play

    public Transform target;            //Destination
    public Transform wanderTarget;      //Target to be used for wandering
    Vector3 lastSeen;                   //Last seen location of player
    Vector3 midWander;                  //Used in wander/search

    public Transform facing;            //Direction the model is facing and therefore seeing out of, usually matches direction of movement, except when searching?

    //bool faceTarget;                   //True when chasing player, false when searching v -> need head node
    bool searching;                     //True when target player is lost -> search upon reaching target -> involves rotating field of view
    bool targetingPlayer;               //True if the target is the player -> run

    NavMeshAgent agent;                 //Used to easily navigate the environment
    Animator anim;                      //Animates model
    Combat combatScript;                // The combat script for scripting combat
    Health healthScript;				// Ditto for script

    int numRayChecks;                   //number of body parts to check with rays for sight

    public float searchTimer;           //Current time spent searching


    // Use this for initialization
    void Start () {
        InitializeVariables();
	}

    /// <summary>
    /// Initialize all private variables.
    /// </summary>
    void InitializeVariables()
    {
        //faceTarget = true;
        searching = true;
        targetingPlayer = false;

        facing = transform;//match to head of model?
        lastSeen = Vector3.zero;
        midWander = Vector3.zero;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Need NavMeshAgent component");
        }
        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("Need Animator component");
        }
        agent.speed = walkSpeed;
        numRayChecks = 6;   //~heeeead, shoulders, knees, and toes(center) ~knees and toes(center)
        searchTimer = 0;

        // Finds the Combat script
        combatScript = GetComponent<Combat>();
        if (combatScript == null) Debug.Log("Enemy needs a Combat script");

        // Finds the Health script
        healthScript = GetComponent<Health>();
        if (healthScript == null) Debug.Log("Enemy needs a Health script");

    }

    /// <summary>
	/// Updates at a fixed rate based on Physics
	/// </summary>
	void FixedUpdate()
    {
        // Ends game if there is no health
        if (healthScript != null && healthScript.health <= 0) return;
        if (healthScript == null) return;

        Animate();
    }

    // Update is called once per frame
    void Update () {
        if (agent.remainingDistance < agent.radius * 2 && !searching)
        {
            if (targetingPlayer)
            {
                targetingPlayer = false;
                agent.autoBraking = true;

                //check if player is still in view
                CheckView();

                //attack if still targeting player
                if (targetingPlayer)
                {
                    //attack
                }
                else
                {
                    //search
                    searching = true;
                    agent.speed = searchSpeed;
                    midWander = transform.forward;
                    lastSeen = target.position;
                }
            }
            else
            {
                if (gm.locations.Length > 0) target.position = gm.locations[Random.Range(0, gm.locations.Length)].transform.position;
            }

        }

        if (searching)
        {
            searchTimer += Time.deltaTime;
            Wander();
            if(searchTimer >= searchDuration)
            {
                searching = false;
                //target = null;
                agent.speed = walkSpeed;
                searchTimer = 0;
                //new location
                if (gm.locations.Length > 0) target.position = gm.locations[Random.Range(0, gm.locations.Length)].transform.position;
            }
        }

        //check for players in view -> set to target
        CheckView();

        //go to last seen location
        /*
        if(target == null && lastSeen != transform.position)
        {
            target.position = lastSeen;
        }
        */

        if (target != null) agent.SetDestination(target.position);
	}

    /// <summary>
    /// Checks whether a player is going to be the target. If so, make it the nearest player.
    /// </summary>
    void CheckView()
    {
        
        if (!searching)
        {
            lastSeen = target.position;
        }

        bool seekingPlayer = false;
        

        foreach (GameObject obj in gm.players)
        {
            Vector3 toObj = obj.transform.position - transform.position;
            //within angle of vision?
            if (WithinFieldOfView(toObj) && toObj.sqrMagnitude <= Mathf.Pow(visionDistance,2))
            {
                //check for distance and obstacles using ray
                Transform[] parts = new Transform[numRayChecks];//array to store parts to check
                parts[0] = obj.transform;
                Animator anim = obj.GetComponentInChildren<Animator>();
                if (anim == null) Debug.LogError("Player must have Animator component");
                //loop to get locations of player parts in range
                foreach (Transform child in anim.GetComponentsInChildren<Transform>())
                {
                    switch (child.name)
                    {
                        case "Head_end":
                            parts[1] = child;
                            break;
                        case "LowerLeg_L":
                            parts[2] = child;
                            break;
                        case "LowerLeg_R":
                            parts[3] = child;
                            break;
                        case "UpperArm_R":
                            parts[4] = child;
                            break;
                        case "UpperArm_L":
                            parts[5] = child;
                            break;
                        default:
                            break;
                    }//end switch
                }

                for(int i = 0; i < numRayChecks; i++)
                {
                    RaycastHit hit;
                    Vector3 vecTo = parts[i].position - transform.position;
                    //something weird with distance...
                    Physics.Raycast(transform.position, vecTo, out hit, visionDistance);
                    //check for obstacles blocking vision -> may need to check around center of player (ie. head, knees, left shoulder, and right shoulder) to better "see"
                    if (hit.transform == obj.transform)
                    {
                        if (targetingPlayer)
                        {
                            //check for closest distance when chasing a player
                            if ((target.position - transform.position).sqrMagnitude > (obj.transform.position - transform.position).sqrMagnitude)
                            {
                                target.position = obj.transform.position;
                            }
                        }
                        else {
                            target.position = obj.transform.position;
                            searching = false;
                            searchTimer = 0;
                            targetingPlayer = true;//now chasing a player
                            agent.speed = runSpeed;
                            agent.autoBraking = false;
                        }//end targeting
                        seekingPlayer = true;//sees player currently
                        break;//break bone check for loop
                    }//end hit
                }//end for loop
            }//end if within view

        }//end foreach

        //if no player in sight and saw player recently
        if (!seekingPlayer && targetingPlayer)
        {
            target.position = lastSeen;
        }
    }

    /*
    see below for shorter version, this one uses dot products for 
    bool WithinFieldOfView(Vector3 toObj)
    {
        toObj.Normalize();
        float rad = Mathf.Deg2Rad * angleOfVision;
        //check within up most field
        facing.Rotate(rad, 0, 0);
        if (Vector3.Dot(toObj, transform.right) < Vector3.Dot(facing.forward, transform.right))
        {
            //check within down most 
            facing.Rotate(-2 * rad, 0, 0);
            if(Vector3.Dot(toObj, -transform.right) > Vector3.Dot(facing.forward, -transform.right))
            {
                facing.Rotate(rad, 0, 0);
                //check right
                facing.Rotate(0, rad, 0);
                if(Vector3.Dot(toObj, transform.up) < Vector3.Dot(facing.forward, transform.up))
                {
                    //check left
                    facing.Rotate(0, -2 * rad, 0);
                    if(Vector3.Dot(toObj, -transform.up) < Vector3.Dot(facing.forward, -transform.up))
                    {
                        facing.Rotate(0, rad, 0);//sets back in center
                        return true;
                    }
                }
            }
        }
        return false;
    }
    */


    /// <summary>
    /// Checks the angle within which the player is compared to the direction the enemy is facing
    /// </summary>
    /// <param name="toObj">The Vector3 from the facing transform of enemy to the transform of player/target</param>
    /// <returns></returns>
    bool WithinFieldOfView(Vector3 toObj)
    {
        toObj.Normalize();
        if (Mathf.Abs(Vector3.Angle(facing.forward, toObj)) <= angleOfVision)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Change destination to be within range of last seen target location.(Not implemented yet)
    /// Rotate facing transform to sim turning head.(Not implemented yet)
    /// Wander strangely.
    /// </summary>
    void Wander()
    {
        float one = 3;//length in front to search
        float two = one / 2;
        float three = two / 3;

        float randomAngle = Mathf.Deg2Rad * Random.Range(0, 360);
        Vector3 farWander = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
        //farWander = Vector3.ClampMagnitude(farWander,three);
        //farWander.Normalize();
        //farWander = farWander * three;
        //midWander += farWander;
        //midWander = Vector3.ClampMagnitude(midWander, two);
        //midWander.Normalize();
        //midWander = midWander * two;
        midWander += farWander;
        //wanderTarget.position = Vector3.ClampMagnitude(midWander, one) + transform.position;
        wanderTarget.position = midWander + transform.position + (transform.forward * one);
        target.position = wanderTarget.position;
    }


    void TurnModel()
    {
        anim.transform.forward = Vector3.Lerp(
                anim.transform.forward,
                agent.velocity,
                0.1f
            );
    }

    /// <summary>
	/// Animate the player model.
	/// </summary>
	void Animate()
    {
        // Turns the model in the correct direction
        TurnModel();

        // Sends necessary values to the Animator
        // Speed
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);
        // Angle Between forward facing direction and velocity direction
        float angleBetween = Vector3.Angle(anim.transform.forward, agent.velocity);
        anim.SetFloat("Angle", angleBetween);
        // Right Vector Dot Product (determines whether velocity is moving to right)
        float rightDot = Vector3.Dot(anim.transform.right, agent.velocity);
        anim.SetFloat("RightDot", rightDot);
    }
}
