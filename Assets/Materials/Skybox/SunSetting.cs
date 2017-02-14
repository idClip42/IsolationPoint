using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSetting : MonoBehaviour {
	public Vector3 sunRotation;
	Quaternion newRotation;
	public float speed = 0.1f;
	Light lt;
	// Use this for initialization
	void Start () {
		Debug.Log (transform.position);
		newRotation = Quaternion.Euler(sunRotation);
		lt = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.rotation = Quaternion.Lerp (transform.rotation, newRotation, Time.time * speed);

		if (transform.rotation == newRotation) {
			lt.intensity = 0;
		}

	}
}
