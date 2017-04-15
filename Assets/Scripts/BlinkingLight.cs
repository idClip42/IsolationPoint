using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class BlinkingLight : MonoBehaviour {

    Light bLight;    //The light to control the intensity of to make it 'blink'
    float timer;    //x value in the sine function
    public float flashSpeedMod = 1.0f;

	// Use this for initialization
	void Start () {
        bLight = gameObject.GetComponent<Light>();
        timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        //sine function
        bLight.intensity = 4 * Mathf.Sin(flashSpeedMod * timer) + 4;
	}
}
