using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWarpEvent : Event {
    public GameObject[] enemiesToWarp;
    public Vector3 locationToWarpTo;
    public float delayIncrease = 0.5f;

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
        float delay = 0.0f;

        //warp after a small delay to hopefully avoid stacking multiple enemies, delay may need to be increased
        foreach(GameObject g in enemiesToWarp)
        {
            //g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo);
            //Invoke("Warp", 0.5f);
            StartCoroutine(WarpWithDelay(delay, g));
            delay += delayIncrease;
        }
    }

    IEnumerator WarpWithDelay(float delay, GameObject g)
    {
        yield return new WaitForSeconds(delay);
        g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo);
    }

    void Warp(GameObject g)
    {
        g.GetComponent<NavMeshAgent>().Warp(locationToWarpTo);
    }

}
