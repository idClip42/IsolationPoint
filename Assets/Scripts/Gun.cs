using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour 
{
	// TODO:
	// animations for one handed and rifle
	// Organize the public variables in the inspector


	public enum GunAnimation { TwoHandedPistolAim };
	public GunAnimation gunAnim;
	public float damage = 25.0f;
	public float wait = 0.5f;
	public float range = 50.0f;
	public int ammo = 11;
	public Transform[] barrels;
	public GameObject muzzleFlash;
	public AudioClip shotSound;
	public float audioDelay;
	public Material trailMat;
	public GameObject bulletHole;
	public bool debugInfiniteAmmo;

	float timer;
	Camera cam;

	void Start () 
	{
		timer = 0;
		cam = Camera.main;
	}
	
	void Update () 
	{
		if(timer > 0)
			timer -= Time.deltaTime;
	}

	public void Shoot(bool accurate)
	{
		if(timer > 0) return;

		if(ammo <= 0 && !debugInfiniteAmmo) return;

		for(int n = 0; n < barrels.Length; ++n)
		{
			Transform b = barrels[n];

			Vector3 forward = accurate ? cam.transform.forward : b.forward;

			RaycastHit hitInfo;
			if(Physics.Raycast(b.position, forward, out hitInfo, range))
			{
				Health h = hitInfo.collider.GetComponent<Health>();
				if(h != null)
				{
					h.Hit(damage, true, hitInfo.point, hitInfo.normal);
				} else {
					MakeBulletHole(hitInfo.point, hitInfo.normal);
				}
				MakeTrail(b.position, hitInfo.point);
			} else {
				MakeTrail(b.position, b.position + forward * range);
			}
		}

		MuzzleFlash(barrels[0]);
		--ammo;
		timer = wait;
	}

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

	void GunshotSound(GameObject muzzleFlash)
	{
		AudioSource a = muzzleFlash.AddComponent<AudioSource>();
		a.clip = shotSound;
		a.time = audioDelay;
		a.Play();
	}

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

	public Vector2 WhereAmIAiming()
	{
		// Where oh where?
		return Vector2.zero;
	}

	public string GetAnim()
	{
		return gunAnim.ToString();
	}
}
