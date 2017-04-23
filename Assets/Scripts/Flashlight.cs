using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour 
{
	Light light;
	Collider col;
	Rigidbody rb;
	Camera cam;
	CharacterController currentChar;
	Animator charAnim;
	GameObject flashightObj;
	Quaternion flashlightOrigPos;

	GameManager gm;

	int leftHandLayerIndex;


	void Start () 
	{
		light = GetComponentInChildren<Light>();
		col = GetComponentInChildren<Collider>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main;
		currentChar = null;
		charAnim = null;
		flashightObj = col.gameObject;
		flashlightOrigPos = flashightObj.transform.rotation;

		gm = GameObject.Find("GM").GetComponent<GameManager>();
	}

	void Update()
	{
		if(currentChar != PlayerController.controller.Player) return;

		// Aims the flashlight where the camera is looking, within reason
		// SCREW REASON! MAKE FLASHLIGHT GO EVERYWHERE!!!

		//float angleLimit = 90.0f;
		float lerpVal = 0.5f;

		Vector3 charForward = charAnim.transform.forward;
		Vector3 camForward = cam.transform.forward;
		float angle = Vector3.Angle(charForward, camForward);
		//if(angle < angleLimit)
		///*
			flashightObj.transform.up = Vector3.Lerp(		// Must be up because of way object is oriented
				flashightObj.transform.up,
				-camForward,
				lerpVal
			);
//*/
		//flashightObj.transform.up = -camForward;
		/*
		else
			flashightObj.transform.up = Vector3.Lerp(
				flashightObj.transform.up,
				-charAnim.transform.forward,
				lerpVal
			);
			*/

        //Check if an enemy is within the light
        CheckEnemyInLight();

		// Animates player with flashlight
		if(charAnim == null) return;
		float animFrame = (Vector3.Angle(-charAnim.transform.right, cam.transform.forward))/180.0f;
		charAnim.Play("FlashlightAim", leftHandLayerIndex, animFrame);

	}

	public void PickUpPutDown(bool pickUp, CharacterController c)
	{
		col.isTrigger = pickUp;
		//light.enabled = pickUp;
		rb.isKinematic = pickUp;
		if(pickUp)
		{
			currentChar = c;
			charAnim = currentChar.GetComponentInChildren<Animator>();
			leftHandLayerIndex = charAnim.GetLayerIndex("LeftHandLayer");
			charAnim.SetLayerWeight(leftHandLayerIndex, 1);
			col.gameObject.layer = 8;
		} else {
			charAnim.SetLayerWeight(leftHandLayerIndex, 0);
			currentChar = null;
			charAnim = null;
			col.gameObject.layer = 0;
		}
	}

    /// <summary>
    /// Cast 9 spheres within the cone of light and send enemy to this location if hit.
    /// </summary>
    void CheckEnemyInLight()
	{
		if(gm == null) return;

        Vector3 forward = light.transform.forward;
        RaycastHit hit;
        
        Physics.SphereCast(transform.position, 0.2f, forward, out hit, light.range);
        Debug.DrawRay(transform.position, forward);
        if(hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }
        /*
        Vector3 aim = Vector3.RotateTowards(forward, transform.up, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, transform.up + transform.right, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, transform.right, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, transform.right - transform.up, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, -transform.up, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, -transform.right - transform.up, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, -transform.right, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }

        aim = Vector3.RotateTowards(forward, transform.up - transform.right, (3 / 4) * light.spotAngle * Mathf.PI / 180, 0);
        Physics.SphereCast(transform.position, 0.2f, aim, out hit, light.range);
        Debug.DrawRay(transform.position, aim);
        if (hit.transform != null && hit.transform.tag == "Enemy")
        {
            hit.transform.gameObject.GetComponent<Enemy>().SetTarget(transform.position, false);
        }
        */
    }
}
