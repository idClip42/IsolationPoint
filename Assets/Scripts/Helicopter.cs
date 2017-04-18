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

	void Start () 
	{
		//heliModel.SetActive(false);
		heliModel.transform.position += startOffset;
		velocity = Vector3.zero;
		acceleration = Vector3.zero;

		heliModel.transform.LookAt(this.transform, Vector3.up);
		heliModel.transform.Rotate(-10, 45, 0);
		velocity = heliModel.transform.forward * speed;

	}
	
	void Update () 
	{
		if(heliModel.activeSelf == false) return;
		Move();
		Animate();
	}

	void Move()
	{
		Vector3 desiredVelocity = Vector3.ClampMagnitude(transform.position - heliModel.transform.position, speed);
		Vector3 velocityAdd = desiredVelocity - velocity;
		acceleration = Vector3.ClampMagnitude(acceleration + velocityAdd, accel);

		velocity += acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude(velocity, speed);

		heliModel.transform.position += velocity * Time.deltaTime;

		if(heliModel.transform.localPosition.y < minHeight)
		{
			Vector3 pos = heliModel.transform.localPosition;
			pos.y = minHeight;
			heliModel.transform.localPosition = pos;
			velocity.y = 0;
		}

		if(velocity.magnitude > turnWithSpeedCutoff)
		{
			Vector3 tiltVector = Vector3.ProjectOnPlane(acceleration, Vector3.ProjectOnPlane(heliModel.transform.forward, Vector3.up));
			tiltVector.y += 4;

			heliModel.transform.LookAt(
				heliModel.transform.position + Vector3.Lerp(
					heliModel.transform.forward,
					velocity,
					0.1f),
				tiltVector
			);
		}


		//Vector3 projectedAccel = Vector3.ProjectOnPlane(acceleration, Vector3.up);
		//float angleBetween = Vector3.Angle(heliModel.transform.forward, projectedAccel);
		//Debug.Log(angleBetween);
		//Vector3 rot = heliModel.transform.rotation.eulerAngles;
		//rot.z = angleBetween/2;
		//heliModel.transform.rotation = Quaternion.Euler(rot);

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
