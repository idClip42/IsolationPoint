using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWarpEvent : Event {
    public GameObject[] enemiesToWarp;
    public Transform locationToWarpTo;
    public float delayIncrease = 0.5f;
    public Transform target;            //Location to go to after warp
    public bool willRun = false;        //Is the target 'seen' as a player?
    public bool willResumeMotion = true;    //Affects the nav agent, false will keep it in one place, true will set it to move


    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        if (!IsPlaying) return;
        base.Update();
	}

    public override void PlayEvent()
    {
        base.PlayEvent();
        //float delay = 0.0f;

        //warp after a small delay to hopefully avoid stacking multiple enemies, delay may need to be increased
        foreach(GameObject g in enemiesToWarp)
        {
            //g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo);
            //Invoke("Warp", 0.5f);
            //StartCoroutine(WarpWithDelay(delay, g));
            //delay += delayIncrease;
            Warp(g);
        }
    }

    IEnumerator WarpWithDelay(float delay, GameObject g)
    {
        yield return new WaitForSeconds(delay);//...?
        g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo.position);
        if (willRun)
        {
            if (target != null)
                g.GetComponent<Enemy>().ChaseTarget(target.position);
        }
        else {
            if (target != null)
                g.GetComponent<Enemy>().SetTarget(target.position, true);
        }
        g.GetComponent<Enemy>().CanMove = willResumeMotion;
    }

    void Warp(GameObject g)
    {
        g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo.position);
        if (willRun)
        {
            if (target != null)
                g.GetComponent<Enemy>().ChaseTarget(target.position);
        }
        else {
            if (target != null)
                g.GetComponent<Enemy>().SetTarget(target.position, true);
        }
        g.GetComponent<Enemy>().CanMove = willResumeMotion;
    }

}
