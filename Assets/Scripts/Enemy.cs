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
    public float acceleration;          // as well as their rate of acceleration

    public float angleOfVision;         //Cone representing field of view
    public float visionDistance;        //Distance the enemy can see at
    public float searchDuration;

    public Transform target;            //Destination

    public float health;                //Enemy health value
    public float meleeDamage;           //Damage enemy does at close range

    public Transform facing;              //Direction the model is facing and therefore seeing out of, usually matches direction of movement, except when searching

    bool faceTarget;                   //True when chasing player ???
    bool searching;                     //True when target player is lost -> search upon reaching target -> involves rotating field of view
    bool targetingPlayer;               //True if the target is the player -> run

    NavMeshAgent agent;                 //Used to easily navigate the environment


    // Use this for initialization
    void Start () {
        InitializeVariables();
	}

    void InitializeVariables()
    {
        faceTarget = true;
        searching = false;

        facing = transform;//match to head of model

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.acceleration = acceleration;
    }
	
	// Update is called once per frame
	void Update () {
        if(target != null) agent.SetDestination(target.position);

        //check for players in view
        CheckView();
	}

    void CheckView()
    {
        foreach(GameObject obj in gm.players)
        {
            Vector3 toObj = obj.transform.position - transform.position;
            if (toObj.sqrMagnitude <= Mathf.Pow(visionDistance, 2))//necessary with ray?
            {
                if(WithinFieldOfView(toObj))
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, toObj, out hit, visionDistance);
                    if (hit.Equals(obj))//check for obstacles in the way
                    {
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
                        }
                    }
                }
            }
        }
    }

    bool WithinFieldOfView(Vector3 toObj)
    {
        toObj.Normalize();
        float rad = Mathf.Deg2Rad * (90 - angleOfVision);
        Transform face = transform;
        //check within right most field
        face.Rotate(rad, 0, 0);
        if (Vector3.Dot(toObj, transform.right) < Vector3.Dot(face.forward, transform.right))
        {
            //check within left most 
            face.Rotate(-2 * rad, 0, 0);
            if(Vector3.Dot(toObj, -transform.right) > Vector3.Dot(face.forward, -transform.right))
            {
                face.Rotate(rad, 0, 0);
                //check up
                face.Rotate(0, rad, 0);
                if(Vector3.Dot(toObj, transform.up) < Vector3.Dot(face.forward, transform.up))
                {
                    //check down
                    face.Rotate(0, -2 * rad, 0);
                    if(Vector3.Dot(toObj, -transform.up) < Vector3.Dot(face.forward, -transform.up))
                    {
                        face.Rotate(0, rad, 0);//sets back in center
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
