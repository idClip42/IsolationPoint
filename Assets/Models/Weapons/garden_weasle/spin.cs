using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour {
    public float revolutionsPerSecond;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate ()
    {
        this.transform.Rotate(new Vector3(0, 1, 0), revolutionsPerSecond * 6  + Random.Range(0,1));
    }
}
