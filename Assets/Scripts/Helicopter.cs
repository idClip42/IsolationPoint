using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour 
{
	public GameObject heliModel;
	public GameObject rotor;
	public GameObject rearRotor;
	public Vector3 startOffset;
	public Vector3 startRotation;
	public float speed;
	public float accel;
	public float minHeight;
	public float turnWithSpeedCutoff = 1.0f;
	Vector3 velocity;
	Vector3 acceleration;
	bool landed;

	float initialDist;

	void Start () 
	{
		//heliModel.SetActive(false);
		heliModel.transform.position += startOffset;
		velocity = Vector3.zero;
		acceleration = Vector3.zero;

		heliModel.transform.LookAt(this.transform, Vector3.up);
		heliModel.transform.Rotate(startRotation);
		velocity = heliModel.transform.forward * speed;

		landed = false;

		initialDist = startOffset.magnitude;

	}
	
	void Update () 
	{
		if(heliModel.activeSelf == false) return;
		Move();
		Animate();
	}

	void Move()
	{
		// Gets the distance from the target position
		Vector3 dist = (transform.position - heliModel.transform.position);
		float distMag = dist.magnitude;
		float distPercentage = distMag/initialDist;

		// Adjusts acceleration towards target
		Vector3 desiredVelocity = Vector3.ClampMagnitude(dist, speed);
		Vector3 velocityAdd = desiredVelocity - velocity;
		acceleration += velocityAdd;
		acceleration = Vector3.ClampMagnitude(acceleration, accel);

		// adds acceleration to velocity
		velocity += acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, speed);
		if(distPercentage < 0.1f)
			velocity = Vector3.ClampMagnitude(velocity, speed * (distPercentage/0.1f * distPercentage/0.1f));
		else
			velocity = Vector3.ClampMagnitude(velocity, speed);

		// add velocity to position
		heliModel.transform.position += velocity * Time.deltaTime;

		// Aim and tilt helicopter 
		if(velocity.magnitude > turnWithSpeedCutoff)
			heliModel.transform.LookAt(
				heliModel.transform.position + Vector3.ProjectOnPlane(acceleration + velocity, Vector3.up),
				Vector3.up + -velocity
			);

		if(landed == false && distMag < 5)
			MakeEscape();

	}

	void Animate()
	{
		rotor.transform.Rotate(0, 0, Time.deltaTime * 1000);
		rearRotor.transform.Rotate(0, 0, Time.deltaTime * 1000);
	}

	void MakeEscape()
	{
		heliModel.tag = "Escape";
		heliModel.AddComponent<Escape>();
		landed = true;
	}

	public void TriggerHelicopter()
	{
		heliModel.SetActive(true);
	}
}
