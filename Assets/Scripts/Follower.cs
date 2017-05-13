using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Follower : MonoBehaviour {

    NavMeshAgent agent;
    public NavMeshAgent Agent
    {
        get { return agent; }
    }
    NavMeshObstacle obst;
    public NavMeshObstacle Obstacle
    {
        get { return obst; }
    }
    Animator anim;
    Health healthScript;
    CharacterController player;
    Transform cameraAxis;

    bool following;
    bool goTo;
    public bool GoTo
    {
        set { goTo = value; }
    }
    public bool isWorking;
    public bool IsWorking
    {
        get { return isWorking; }
        set {
            isWorking = value;
            if (agent.enabled)
            {
                if (isWorking)
                {
                    agent.Stop();
                }
                else
                {
                    agent.Resume();
                }
            }
        }
    }
    GameObject leader;
    Vector3 goToTarget;
    int crouchState;
    public int CrouchState
    {
        get { return crouchState; }
        set
        {
            if (player != PlayerController.controller.Player)
            {
                float camCrouchOffset = 0.5f;


                if (crouchState == 3 && value != 3)        // If intending to crouch and currently standing
                {
                    player.height /= 3;                             // Halve player height and
                    player.center -= new Vector3(0, player.height, 0);  // Move the collider center down
                    value = 2;
                    cameraAxis.transform.position -= Vector3.up * camCrouchOffset;
                }
                else if (crouchState != 3 && value == 3)       // If intending to stand and currently crouching
                {                                                   // Return player height and center to normal
                    player.center += new Vector3(0, player.height, 0);
                    player.height *= 3;
                    value = 3;
                    cameraAxis.transform.position += Vector3.up * camCrouchOffset;
                }
            }
            crouchState = value;
        }
    }


    // Use this for initialization
    void Start () {
        //set false in inspector
        obst = GetComponent<NavMeshObstacle>();

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("Player needs a NavMeshAgent");
        Stay();

        following = false;
        goTo = false;

        crouchState = 3; //Standing

        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogError("Player needs an Animator");

        // Finds the Health script
        healthScript = GetComponent<Health>();
        if (healthScript == null)
            Debug.Log("Player needs a Health script");

        player = GetComponent<CharacterController>();
        cameraAxis = player.transform.FindChild("CameraAxis");
    }

    /// <summary>
	/// Updates at a fixed rate based on Physics
	/// </summary>
	void FixedUpdate()
	{
		if (!agent.enabled) return;	// Added to prevent this animation from overwriting controller animation

        if (healthScript != null && healthScript.health <= 0)
        {
            Stay();
            return;
        }
        if (healthScript == null)
        {
            Stay();
            return;
        }

        Animate();
    }

    // Update is called once per frame
    void Update () {
        if (!enabled) return;

        //follow the leader~
        if (following && agent.enabled)
            agent.SetDestination(leader.transform.position);
	}

    /// <summary>
    /// Set the leader of this to the current player. Set the destination to the player location.
    /// </summary>
    /// <param name="target">The current player or the object specified as the leader</param>
    public void SetLeader(GameObject target)
    {
        following = true;
        obst.enabled = false;
        leader = target;
        if (agent.enabled) agent.SetDestination(leader.transform.position);
    }

    /// <summary>
    /// Set the go to destination to go to this becomes not the player.
    /// </summary>
    /// <param name="destination">Location to seek</param>
    public void SetDestination(Vector3 destination)
    {
        following = false;
        goTo = true;
        goToTarget = destination;
    }

    /// <summary>
    /// Resets current target to self.
    /// </summary>
    public void Stay()
    {
        following = false;
        //set target to self to stop movement
		if(agent.isOnNavMesh != true) return;	// Prevents an error
        agent.SetDestination(transform.position);
    }

    /// <summary>
    /// Enables the agent on this gameObject and sets the destination if the player has set a target with 't'.
    /// </summary>
    public void EnableAgent()
    {
        agent.enabled = true;
        obst.enabled = false;
        following = false;
        if (goTo)
        {
            goTo = false;
            agent.SetDestination(goToTarget);
        }

        if (isWorking)
        {
            agent.Stop();
        }
        else
        {
            agent.Resume();
        }
    }

    void TurnModel()
    {
        anim.transform.forward = Vector3.Lerp(
                anim.transform.forward,
			Vector3.ProjectOnPlane(agent.velocity, Vector3.up),
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
        // Crouching state -- match to leader
        if (leader != null)
        {
            CrouchState = leader.GetComponentInChildren<Follower>().CrouchState;
        }
        if (player != PlayerController.controller.Player) anim.SetInteger("CrouchState", crouchState);
        // Right Vector Dot Product (determines whether velocity is moving to right)
        float rightDot = Vector3.Dot(anim.transform.right, agent.velocity);
        anim.SetFloat("RightDot", rightDot);
    }

    public bool IsOnNavMesh()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position, out hit, agent.baseOffset + 0.1f, NavMesh.AllAreas);
        return hit.hit;
    }


}
