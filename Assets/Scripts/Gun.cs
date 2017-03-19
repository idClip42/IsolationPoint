using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour 
{
	// TODO:
	// animations for one handed and rifle
	// kick?


	public enum GunAnimation { TwoHandedPistolAim, LongGunAim };	// The set of animations for aiming 
	[Space(10)]
	public GunAnimation gunAnim;						// The selected aiming animation
	[Space(10)]
	public float damage = 25.0f;						// How much damage a bullet does
	public int ammo = 11;								// How many bullets the gun has
	public float range = 50.0f;							// The range on the gun
	public float wait = 0.5f;							// The minimum time between shots
	[Space(10)]
	public float kickDist = 0;							// The distance the gun moves back when it kicks
	public float kickAngle = 0;							// The angle it rotates upwards when it kicks
	public float kickReturnLerp = 0.5f;					// The lerp value for the gun returning to normal position and rotation
	[Space(10)]
	public Transform[] barrels;							// Empty GameObjects that each will fire one bullet
														// from their position along their forward vector
														// when the gun is fired
	[Space(10)]
	public GameObject muzzleFlash;						// The muzzle flash that shows when the gun is fired
	public AudioClip shotSound;							// The gunshot sound
	public float audioDelay;							// Used for skipping over silence at the beginning of the audio clip
	public Material trailMat;							// The material for the bullet trail
	public GameObject bulletHole;						// The bullet hole game object, with particle effects
	[Space(10)]
	public bool debugInfiniteAmmo;						// Whether the gun has infinite ammo

	float timer;										// Used for timing purposes
	Camera cam;											// The main camera

	UI_Manager UIScript;

	void Start () 
	{
		timer = 0;
		cam = Camera.main;
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager> ();
	}
	
	void Update () 
	{
		if(timer > 0)
			timer -= Time.deltaTime;

		// Returns gun to normal position after kick
		if(GetComponent<Collider>().enabled == false)
		{
			transform.localPosition = Vector3.Lerp(
				transform.localPosition,
				Vector3.zero,
				kickReturnLerp);
			transform.localRotation = Quaternion.Lerp(
				transform.localRotation,
				Quaternion.identity,
				kickReturnLerp);
		}
	}




	/// <summary>
	/// Shoots the gun
	/// </summary>
	/// <param name="accurate">If true, bullets travel in the direction the camera is facing. If false, they travel in the direction the gun barrel(s) face, which will wobble with the animation.</param>
	public void Shoot(bool accurate)
	{
		// Timer prevents shooting too soon after previous shot
		if(timer > 0) return;

		// If no ammo left (and infinite ammo is off), no shooting for you
		if(ammo <= 0 && !debugInfiniteAmmo) return;

		// Goes through each of the gun's barrels
		for(int n = 0; n < barrels.Length; ++n)
		{
			Transform b = barrels[n];
			Vector3 forward = accurate ? cam.transform.forward : b.forward;

			MuzzleFlash(b);

			// Uses raycasting to determine where the bullet hits
			RaycastHit hitInfo;
			if(Physics.Raycast(b.position, forward, out hitInfo, range))
			{
				// If the target has a health script, they are damaged
				// Otherwise, a simple bullet hole is made
				Health h = hitInfo.collider.GetComponent<Health>();
				Health_Part hp = hitInfo.collider.GetComponent<Health_Part>();
				if(h != null)
				{
					h.Hit(damage, true, hitInfo.point, hitInfo.normal, null);
				} else if(hp != null) {
					hp.Hit(damage, true, hitInfo.point, hitInfo.normal);
				} else {
					MakeBulletHole(hitInfo.point, hitInfo.normal);
				}
				// A trail shows where the bullet went
				MakeTrail(b.position, hitInfo.point);
			} else {
				MakeTrail(b.position, b.position + forward * range);
			}
		}
		Kick();
		--ammo;
		timer = wait;
	}


	/// <summary>
	/// Instantiates a muzzle flash object
	/// </summary>
	/// <param name="barrel">The barrel it comes out of</param>
	void MuzzleFlash(Transform barrel)
	{
		GameObject flash = (GameObject) Instantiate(
			muzzleFlash,
			barrel.position,
			Quaternion.identity
		);
		flash.name = "MuzzleFlash";
		flash.transform.SetParent(barrel);
		GunshotSound(flash);
		Destroy(flash, 1.0f);
	}


	/// <summary>
	/// Adds a gunshot sound to the muzzle flash
	/// </summary>
	/// <param name="muzzleFlash">Muzzle flash that will make the sound</param>
	void GunshotSound(GameObject muzzleFlash)
	{
		AudioSource a = muzzleFlash.AddComponent<AudioSource>();
		a.clip = shotSound;
		a.time = audioDelay;
		a.Play();
	}



	/// <summary>
	/// Instantiates a bullet hole in the target object
	/// </summary>
	/// <param name="point">Location of bullet hole</param>
	/// <param name="normal">Normal of surface for bullet hole</param>
	void MakeBulletHole(Vector3 point, Vector3 normal)
	{
		GameObject b = (GameObject) Instantiate(
			bulletHole,
			point + normal * 0.005f,
			Quaternion.identity
		);
		b.transform.forward = normal;
		b.transform.Rotate(0, 0, Random.Range(0, 360));
		Destroy(b, 10);
	}


	/// <summary>
	/// Instantiates a game object with a trail to visualize the path of the bullet
	/// </summary>
	/// <param name="a">The start point</param>
	/// <param name="b">The end point</param>
	void MakeTrail(Vector3 a, Vector3 b)
	{
		GameObject line = new GameObject();
		line.transform.position = b;
		line.name = "BulletTrail";
		LineRenderer lr = line.AddComponent<LineRenderer>();
		lr.startWidth = 0.02f;
		lr.endWidth = 0.02f;
		lr.SetPosition(0, a);
		lr.SetPosition(1, b);
		lr.material = trailMat;
		Destroy(line, 1.0f);
	}


	/// <summary>
	/// Makes the gun kick when fired
	/// </summary>
	void Kick()
	{
		transform.Translate(Vector3.up * -kickDist);
		transform.Rotate(Vector3.forward * kickAngle);
	}



	/// <summary>
	/// Finds the point on the screen (as a percentage) where the character is aiming their weapon
	/// Only returns center of screen right now
	/// </summary>
	/// <returns>The point on the screen (as a percentage)</returns>
	public Vector2 WhereAmIAiming()
	{
		Vector2 screenSpacePos = new Vector2(0.5f, 0.5f);
		RaycastHit hitInfo;
		if(Physics.Raycast(barrels[0].position, cam.transform.forward, out hitInfo, range))
		{
			Vector3 screenPos = cam.WorldToViewportPoint(hitInfo.point);
			screenSpacePos = new Vector2(screenPos.x, screenPos.y);
		}
		return screenSpacePos;
	}

	/// <summary>
	/// Put the crosshair where the player is aiming
	/// </summary>
	public void UpdateCrosshair(){
		Vector2 screenPos = WhereAmIAiming ();
		UIScript.crosshair.rectTransform.anchorMin = screenPos;
		UIScript.crosshair.rectTransform.anchorMax = screenPos;
	}

	public void IsHeld(bool value)
	{
		GetComponent<Collider>().enabled = !value;
	}
}
