﻿using System.Collections;
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
    Animator anim;
    Health healthScript;

    bool following;
    bool goTo;
    GameObject leader;
    Vector3 goToTarget;


    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("Player needs a NavMeshAgent");
        Stay();

        following = false;
        goTo = false;

        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogError("Player needs an Animator");

        // Finds the Health script
        healthScript = GetComponent<Health>();
        if (healthScript == null)
            Debug.Log("Enemy needs a Health script");
    }

    /// <summary>
	/// Updates at a fixed rate based on Physics
	/// </summary>
	void FixedUpdate()
	{
		if (!agent.enabled) return;	// Added to prevent this animation from overwriting controller animation

        // Ends game if there is no health
        if (healthScript != null && healthScript.health <= 0) return;
        if (healthScript == null) return;

        Animate();
    }

    // Update is called once per frame
    void Update () {
        if (!enabled) return;

        //follow the leader~
        if (following)
            agent.SetDestination(leader.transform.position);
	}

    /// <summary>
    /// Set the leader of this to the current player. Set the destination to the player location.
    /// </summary>
    /// <param name="target">The current player or the object specified as the leader</param>
    public void SetLeader(GameObject target)
    {
        following = true;
        leader = target;
        agent.SetDestination(leader.transform.position);
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
        if (goTo)
        {
            goTo = false;
            agent.SetDestination(goToTarget);
        }
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
