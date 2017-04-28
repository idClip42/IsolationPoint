using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayerWhenNotLooking : MonoBehaviour {

    public Transform playerCamera;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(!GetComponent<Renderer>().isVisible)
        {
            Vector3 point = this.transform.position + Vector3.Normalize(playerCamera.transform.position - this.transform.position);
            point.y = 0 + this.transform.position.y;
            this.transform.LookAt(point);
        }
    }
}
