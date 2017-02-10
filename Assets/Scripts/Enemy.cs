using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {

    public GameManager gm;              //for general game info such as players and their locations

    public float walkSpeed;             // The speed at which the enemy walks/wanders,
    public float runSpeed;              // runs/sprints/chases,
    public float searchSpeed;           // and searches,
    public float acceleration;          // as well as their rate of acceleration -> redundant if never changes b/c agent has same field

    public float angleOfVision;         //Cone representing field of view
    public float visionDistance;        //Distance the enemy can see at
    public float searchDuration;        //How long the enemy searches for players near the last seen location
    public float timeBetweenAttacks;    //Delay between attack sequences
    public float attackSpeed;           //How long does it take for attack sequence to play

    public Transform target;            //Destination

    public float health;                //Enemy health value
    public float meleeDamage;           //Damage enemy does at close range

    public Transform facing;            //Direction the model is facing and therefore seeing out of, usually matches direction of movement, except when searching

    bool faceTarget;                   //True when chasing player, false when searching v -> need head node
    bool searching;                     //True when target player is lost -> search upon reaching target -> involves rotating field of view
    bool targetingPlayer;               //True if the target is the player -> run

    NavMeshAgent agent;                 //Used to easily navigate the environment

    int numRayChecks;                   //number of body parts to check with rays for sight


    // Use this for initialization
    void Start () {
        InitializeVariables();
	}

    /// <summary>
    /// Initialize all private variables.
    /// </summary>
    void InitializeVariables()
    {
        faceTarget = true;
        searching = false;
        targetingPlayer = false;

        facing = transform;//match to head of model

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.acceleration = acceleration;
        numRayChecks = 6;   //~heeeead, shoulders, knees, and toes(center) ~knees and toes(center)
    }
	
	// Update is called once per frame
	void Update () {
        if (agent.remainingDistance < agent.radius)
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
                }
            }
            //find new target if locations size is greater than 0
            else if (gm.locations.Length > 0)
            {
                target = gm.locations[Random.Range(0, gm.locations.Length)].transform;
            }
        }

        //check for players in view
        CheckView();

        if (target != null) agent.SetDestination(target.position);
	}

    /// <summary>
    /// Checks whether a player is going to be the target. If so, make it the nearest player.
    /// </summary>
    void CheckView()
    {
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
                    Physics.Raycast(transform.position, vecTo, out hit, visionDistance);
                    //check for obstacles blocking vision -> may need to check around center of player (ie. head, knees, left shoulder, and right shoulder) to better "see"
                    if (hit.transform == obj.transform)
                    {
                        Debug.Log("Body part seen: #" + i);
                        if (targetingPlayer)
                        {
                            //check for closest distance when chasing a player
                            if ((target.position - transform.position).sqrMagnitude > (obj.transform.position - transform.position).sqrMagnitude)
                            {
                                target = obj.transform;
                            }
                        }
                        else {
                            target = obj.transform;
                            targetingPlayer = true;//now chasing a player
                            agent.autoBraking = false;
                        }//end targeting
                        break;//break for loop
                    }//end hit
                }//end for loop
            }//end if within view

        }//end foreach
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
    /// Checks the angle within which the player is comopared to the direction the enemy is facing
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
}
