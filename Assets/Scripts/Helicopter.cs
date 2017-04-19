using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour 
{
	public GameObject heliModel;
	public GameObject rotors;
	public Vector3 startOffset;
	public float speed;
	public float accel;
	public float minHeight;
	public float turnWithSpeedCutoff = 1.0f;
	Vector3 velocity;
	Vector3 acceleration;

	float initialDist;

	void Start () 
	{
		//heliModel.SetActive(false);
		heliModel.transform.position += startOffset;
		velocity = Vector3.zero;
		acceleration = Vector3.zero;

		heliModel.transform.LookAt(this.transform, Vector3.up);
		heliModel.transform.Rotate(-10, 45, 10);
		velocity = heliModel.transform.forward * speed;

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
		float distPercentage = dist.magnitude/initialDist;

		// Adjusts acceleration towards target
		Vector3 desiredVelocity = Vector3.ClampMagnitude(transform.position - heliModel.transform.position, speed);
		Vector3 velocityAdd = desiredVelocity - velocity;
		acceleration = Vector3.ClampMagnitude(acceleration + velocityAdd, accel);
		// Lowers acceleration based on distance
		acceleration -= (1-distPercentage) * accel * dist.normalized;

		// adds acceleration to velocity
		velocity += acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, speed);

		// Adds drag to velocity
		velocity -= velocity * 0.1f * Time.deltaTime;

		// Adds velocity to position
		heliModel.transform.position += velocity * Time.deltaTime;

		// Keeps the helicopter above a given height
		if(heliModel.transform.localPosition.y < minHeight)
		{
			Vector3 pos = heliModel.transform.localPosition;
			pos.y = minHeight;
			heliModel.transform.localPosition = pos;
			velocity.y = 0;
		}

		// Aims the helicopter and tilts it with turns
		if(velocity.magnitude > turnWithSpeedCutoff)
		{
			Vector3 tiltVector = Vector3.ProjectOnPlane(acceleration, Vector3.ProjectOnPlane(heliModel.transform.forward, Vector3.up));
			tiltVector.y += 2;

			heliModel.transform.LookAt(
				heliModel.transform.position + Vector3.Lerp(
					heliModel.transform.forward,
					Vector3.ProjectOnPlane(acceleration + velocity, Vector3.up),
					0.1f),
				tiltVector
			);
		} else {
			heliModel.transform.up = Vector3.Lerp(
				heliModel.transform.up,
				Vector3.up,
				0.1f
			);
		}

	}

	void Animate()
	{
		rotors.transform.Rotate(0, Time.deltaTime * 1000, 0);
	}

	public void TriggerHelicopter()
	{
		heliModel.SetActive(true);
	}
}
