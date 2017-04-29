using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Combat))]
public class Enemy : MonoBehaviour {

    public GameManager gm;              //for general game info such as players and their locations

    public float walkSpeed;             // The speed at which the enemy walks/wanders,
    public float runSpeed;              // runs/sprints/chases,
    public float searchSpeed;           // and searches,

    public float angleOfVision;         //Cone representing field of view
    public float visionDistance;        //Distance the enemy can see at
    public float searchDuration;        //How long the enemy searches for players near the last seen location
    public float trapTime = 5.0f;              //How long an enemy will be immobilized by a trap

    public Transform target;            //Destination
    Vector3 lastSeen;                   //Last seen location of player
    Vector3 midWander;                  //Used in wander/search

    //Transform facing;                     //Direction the model is facing and therefore seeing out of, usually matches direction of movement, except when searching?

    bool searching;                     //True when target player is lost -> search upon reaching target -> involves rotating field of view
    bool targetingPlayer;               //True if the target is the player -> run
    bool isImmobilized;

    NavMeshAgent agent;                 //Used to easily navigate the environment
    Animator anim;                      //Animates model
    Combat combatScript;                // The combat script for scripting combat
    Health healthScript;				// Ditto for script

    int numRayChecks;                   //number of body parts to check with rays for sight
    int locSet;                         //current location set
    int locInd;                         //current index within set

    float searchTimer;                  //Current time spent searching
    float waitTime;                     //Used to set the location after a warp
    float trapTimer;                    //Used for immobilizing
    public float lookTime = 5.0f;              //Time spent at a location before moving on, reuse searchTimer
    Vector3 afterWarp;

    public float startAttackDistance = 0.5f;   //Distance from player at which the enemy will start its attack
    public bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
        set {
            canMove = value;
            if (canMove)
            {
                agent.Resume();
            }
            else
            {
                agent.Stop();
            }
        }
    }

    public bool autoChangePath = true;


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
        isImmobilized = false;

        //facing = transform;//match to head of model
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if(child.name == "Head_end")
            {
                //facing = child.transform;
            }
        }
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

        locSet = 0;
        locInd = 0;

        waitTime = 0;
        trapTimer = 0;

        CanMove = canMove;

		anim.SetLayerWeight(anim.GetLayerIndex("HeadLayer"), 0);
		anim.SetLayerWeight(anim.GetLayerIndex("LeftHandLayer"), 0);
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
        //gameover / cannot move
        if (agent.enabled)
        {
            if (gm.PauseInput)
            {
                agent.Stop();
                return;
            }
            else
            {
                agent.Resume();
            }
        }

        if (healthScript.health <= 0)
        {
            gm.gameover = true;
            return;
        }

        if(waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            if(waitTime < 0)
            {
                target.position = afterWarp;
                if (agent.enabled) agent.SetDestination(target.position);
            }
        }

        if (isImmobilized)
        {
            trapTimer += Time.deltaTime;
            if(trapTimer >= trapTime)
            {
                Free();
            }
        }

        //move
        if (searching)
        {
            searchTimer += Time.deltaTime;
            Wander();
            if (searchTimer >= searchDuration)
            {
                searching = false;
                agent.speed = walkSpeed;
                searchTimer = 0;

                //new location
                NewLocation();
            }
        }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= agent.stoppingDistance + startAttackDistance && !searching)
        {
            if (targetingPlayer)
            {
                targetingPlayer = false;

                //check if player is still in view
                CheckView();

                //attack if still targeting player
                if (targetingPlayer)
                {
                    //attack
                    combatScript.Attack();
                    //FaceTarget();
                }
                else
                {
                    //search
                    searching = true;
                    agent.speed = searchSpeed;
                    midWander = transform.forward * 3;
                    lastSeen = target.position;
                    locInd = -1;
                }
            }
            else
            {
                searchTimer += Time.deltaTime;
                if (searchTimer >= lookTime)
                {
                    searchTimer = 0;
                    NewLocation();
                }
            }

        }


        //check for players in view -> set to target
        CheckView();

        if (targetingPlayer) FaceTarget();
        if (target != null && agent.enabled) agent.SetDestination(target.position);
    }

    /// <summary>
    /// Find a new location to go to, within the subset if any left.
    /// </summary>
    void NewLocation()
    {
        //if locations exist
        if (gm.locations.Length > 0)
        {
            //if finished with locations in set
            // --> switch set
            if (gm.locations[locSet].GetComponent<LocationList>().locations.Length - 1 <= locInd || locInd < 0)
            {
                if (autoChangePath)
                {
                    int loc;
                    do
                    {
                        loc = Random.Range(0, gm.locations.Length);
                    } while (loc == locSet);
                    locSet = loc;
                    locInd = -1;
                }
                else
                {
                    // --> return to start of current set
                    locInd = -1;
                }
            }
        }
        locInd++;
        target.position = gm.locations[locSet].GetComponent<LocationList>().locations[locInd].transform.position;
    }

    /// <summary>
    /// Checks whether a player is going to be the target. If so, make it the nearest player.
    /// </summary>
    void CheckView()
    {
        //If not searching the position is where the enemy last "saw" the player
        if (!searching)
        {
            lastSeen = target.position;
        }

        //Currently seeing and seeking player?
        bool seekingPlayer = false;
        
        //Check each player...
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

                    //to see through one layer of glass..(needs testing)
                    if (hit.transform)
                    {
                        if (hit.transform.tag == "Glass")
                        {
                            //start from where ray hit the glass, same direction as previous, with distance to glass subtracted from vision distance
                            Physics.Raycast(hit.transform.position, vecTo, out hit, visionDistance - hit.distance);
                        }
                    }

                    //check for obstacles blocking vision -> may need to check around center of player (ie. head, knees, left shoulder, and right shoulder) to better "see"
                    if (hit.transform == parts[i])
                    {
                        //Currently seeking player and targeting a player...
                        if (targetingPlayer && seekingPlayer)
                        {
                            //check for closest distance when chasing a player
                            if ((target.position - transform.position).sqrMagnitude > (obj.transform.position - transform.position).sqrMagnitude)
                            {
                                //set closer player as target
                                target.position = obj.transform.position;
                            }
                        }
                        else //Not seeking player or not targeting player...
                        {
                            //set player as target
                            target.position = obj.transform.position;
                            searching = false;//has a destination that is player
                            searchTimer = 0;
                            targetingPlayer = true;//now chasing a player
                            agent.speed = runSpeed;
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
            //go to last seen position
            target.position = lastSeen;
        }
    }


    /// <summary>
    /// Checks the angle within which the player is compared to the direction the enemy is facing
    /// </summary>
    /// <param name="toObj">The Vector3 from the facing transform of enemy to the transform of player/target</param>
    /// <returns></returns>
    bool WithinFieldOfView(Vector3 toObj)
    {
        //get direction to object
        toObj.Normalize();
        //within view...
        if (Mathf.Abs(Vector3.Angle(transform.forward, toObj)) <= angleOfVision)
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
        float three = one / 9;

        float randomAngle = Mathf.Deg2Rad * Random.Range(0, 360);
        Vector3 farWander = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
        farWander = Vector3.ClampMagnitude(farWander,three);

        midWander += farWander;
        midWander *= 10;
        midWander = Vector3.ClampMagnitude(midWander, one);

        target.position = midWander + transform.position;

        //i dunno if this actually works
        RaycastHit hit;
        Physics.Raycast(transform.position, target.position - transform.position, out hit, one);
        //if it hit something
        if (hit.transform)
        {
            //choose new loc away
            Vector3 norm = Vector3.Normalize(hit.normal);
            midWander += norm;
            //reset midWander
            midWander = transform.forward * 3;
        }
    }

    /// <summary>
    /// Turn to face the target.
    /// </summary>
    void FaceTarget()
    {
        //if (target.position == transform.position) return;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z), new Vector3(0, 1, 0));    // flattens the vector3
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1.0f);
        transform.rotation = lookRotation;
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
	/// Animate the model.
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

    /// <summary>
    /// Alert the enemy to an 'interesting' location it should go to. Not a high priority so it only walks. 
    /// </summary>
    /// <param name="pos">Location to go to</param>
    public void SetTarget(Vector3 pos, bool forgetPlayer)
    {
        //if still remember the player position and targeting player, dont set new target
        if (!forgetPlayer && targetingPlayer) return;

        //if not already targeting player...
        waitTime = 1;
        searching = false;
        afterWarp = pos;
    }

    /// <summary>
    /// Alert the enemy to the location where it thinks a player is.
    /// </summary>
    /// <param name="playerPos">Supposed position of the player</param>
    public void ChaseTarget(Vector3 playerPos)
    {
        waitTime = 1;
        afterWarp = playerPos;
        targetingPlayer = true;
        searching = false;
        lastSeen = playerPos;
    }

    /// <summary>
    /// Manually set which path, from GM, that the enemy will wander on.
    /// </summary>
    /// <param name="pathIndex">Index to the location set in GM.</param>
    public void SetPath(int pathIndex)
    {
        if (pathIndex < 0 && pathIndex >= gm.locations.Length) return;
        locSet = pathIndex;
        locInd = 0;
        target.position = gm.locations[locSet].GetComponentInChildren<LocationList>().locations[locInd].position;
    }

    /// <summary>
    /// Manually set which path, from GM, that the enemy will wander on.
    /// </summary>
    /// <param name="pathName">The name of the path to travel.</param>
    public void SetPath(string pathName)
    {
        for(int i = 0; i < gm.locations.Length; i++)
        {
            if(gm.locations[i].name == pathName)
            {
                locSet = i;
                locInd = 0;
                target.position = gm.locations[locSet].GetComponentInChildren<LocationList>().locations[locInd].position;
            }
        }
    }

    /// <summary>
    /// Sets the immobilized enemy able to move again.
    /// </summary>
    public void Free()
    {
        isImmobilized = false;
        trapTimer = 0;
        agent.enabled = true;
        agent.Resume();
    }

    /// <summary>
    /// Stop the enemy from moving as if caught by a trap. Immobilized.
    /// </summary>
    public void StartCaughtTimer()
    {
        trapTimer = 0;
        isImmobilized = true;
        if (agent.enabled) agent.Stop();
        agent.enabled = false;
    }
}
