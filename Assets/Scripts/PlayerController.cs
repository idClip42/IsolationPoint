using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	CharacterController player;			// The Character Controller of the current player character
										// This is the object that gets moved around
	public CharacterController[] playerList;
										// The list of Character Controllers to be swapped between

	public float camXSpeed;				// The speed at which the mouse (or other input?)
	public float camYSpeed;				// moves the camera

	public float walkSpeed;				// The speed at which the player walks,
	public float runSpeed;				// runs,
	public float crouchSpeed;			// and sneaks,
	public float acceleration;			// as well as their rate of acceleration


	Camera cam;							// The Main Camera
	Transform cameraAxis;				// The axis around which the main camera will move
	Transform cameraTarget;				// The location is space, a child of the axis,
										// which the camera smoothly moves towards

	Animator anim;						// The character model of the current player character
	Transform headBone;					// The head bone of the character model - used for camera purposes
	Transform fpsCamTarget;				// An empty object positioned at the eyes of the character model, used as the FPS cam position

	Combat combatScript;				// The combat script for scripting combat
	Health healthScript;				// Ditto for script
	Pickup_Drop_Items pickupScript; 	// Script for picking up/ putting down items

	Vector3 velocity;					// The velocity the Character Controller will move at every frame

	bool alwaysFaceForward;				// Whether the character model will always turn to face the way the camera is facing (true),
										// or face in the direction it is moving (false)
										// The latter is for running around freely, and the former is for aiming weapons

	float camXOffset;					// How far to the side of the player the third person, "over-the-shoulder" camera is

	int crouchState;					// Whether the players is standing (3), crouching (2), or crawling (1)
	bool firstPerson;					// Whether the player is in first-person mode (true), or third-person mode (false)

	int playerNum;						// The index of the current player character


	int bitFieldAllLayers;				// The bitfield used to signify all layers




	public static PlayerController controller;
										// The statuc player controller variable with which other
										// scripts will access it







	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		InitializeVariables();
		//LockMouse();
	}

	/// <summary>
	/// Initializes all declared private variables
	/// </summary>
	void InitializeVariables()
	{
		// Initializes the Player Controller
		controller = this;

		// Initializes the player
		playerNum = 0;
		if(playerList.Length == 0) Debug.LogError("Need at least one player in Player List");
		player = playerList[playerNum];

		// Finds the Main Camera
		cam = Camera.main;
		if(cam == null) Debug.LogError("Must have main camera tagged 'MainCamera'");

		// Gets necessary pieces of the selected player
		SetPlayerVars();

		// Starts with no velocity, standing, in third person.
		velocity = Vector3.zero;
		crouchState = 3;
		firstPerson = false;

		// Character model faces whichever direction they're moving
		alwaysFaceForward = false;

		// Camera X Offset is stored for later use
		camXOffset = cameraTarget.transform.localPosition.x;

		// A bitfield that translates to 111111. Will likely need to be increased
		bitFieldAllLayers = 63;

		// Makes it so everyone doesn't wipe their nose in sync
		// Also prevents the character controller itself from registering on raycast
		//  (In favor of actual body colliders)
		for(int n = 0; n < playerList.Length; ++n)
		{
			playerList[n].GetComponentInChildren<Animator>().SetFloat("NoseWipeOffset", Random.value);
		}
	}




	/// <summary>
	/// Sets the player variables.
	/// </summary>
	void SetPlayerVars()
	{
		// Finds the Camera Axis
		cameraAxis = player.transform.FindChild("CameraAxis");
		if(cameraAxis == null) Debug.Log("Must have camera axis named 'CameraAxis'");

		// Finds the Camera Target
		cameraTarget = cameraAxis.FindChild("CameraTarget");
		if(cameraTarget == null) Debug.Log("Must have camera target named 'CameraTarget'");

		// Finds the Character Model
		anim = player.GetComponentInChildren<Animator>();
		if(anim == null) Debug.Log("Player needs an animator");

		// Finds the Head Bone
		foreach (Transform child in anim.GetComponentsInChildren<Transform>())
			if(child.name == "Head") headBone = child;
		if(headBone == null) Debug.Log("Must have head bone named 'Head'");

		// Finds the FPS Camera Target
		foreach (Transform child in anim.GetComponentsInChildren<Transform>())
			if(child.name == "FPSCamTarget") fpsCamTarget = child;
		if(fpsCamTarget == null) Debug.Log("Must have head bone named 'FPSCamTarget'");

		// Finds the Combat script
		combatScript = player.GetComponent<Combat>();
		if(combatScript == null) Debug.Log("Player needs a Combat script");

		// Finds the Health script
		healthScript = player.GetComponent<Health>();
		if(healthScript == null) Debug.Log("Player needs a Health script (otherwise they are considered dead and cannot move)");

		pickupScript = player.GetComponent<Pickup_Drop_Items> ();
		if (pickupScript == null) Debug.Log ("Player needs a Pickup script");

		// Gets the current transform of the player model
		if (pickupScript != null)
			pickupScript.PlayerTransform = player.GetComponent<Transform> ();
	}


















	/// <summary>
	/// Updates at a fixed rate based on Physics
	/// </summary>
	void FixedUpdate () 
	{
		// If camera is smoothly following player around in third-person,
		// updates it here in Fixed Update
		if(!firstPerson) CameraTurn();

		// Ends if there is no health or no health script, which means the player is dead
		if(healthScript != null && healthScript.health <= 0) return;
		if(healthScript == null) return;

		MovePlayer();
		Animate();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		// If camera is in player's head in third person,
		// updates it here in Update 
		if(firstPerson) CameraTurn();

		// Switch between player characters
		SwapCharactersInput();

		if(Input.GetMouseButtonDown(0))
			LockMouse();

		// Ends if there is no health or no health script, which means the player is dead
		if(healthScript != null && healthScript.health <= 0) return;
		if(healthScript == null) return;

		// User input to toggle crouching
		CrouchInput();

		// User input to attack
		AttackInput();

		// User input to switch between first and third person
		SwapFirstThirdPerson();
	}

	/// <summary>
	/// A debugging GUI that shows special player controls
	/// </summary>
	void OnGUI()
	{
		string fText = firstPerson ? 
			"Third Person" :
			"First Person";
		GUI.Box(new Rect(0,0,200,80), "C - Crouch\nF - " + fText + "\nQ - Switch Characters");
	}










	/// <summary>
	/// Moves the player.
	/// </summary>
	void MovePlayer()
	{
		// Updates velocity with user input
		MoveInput();

		// Determines and enforces player speed based on how they are moving
		bool runInput = Input.GetButton("Run");
		float maxSpeed = crouchState < 3 ? crouchSpeed : runInput ? runSpeed : walkSpeed;
		if(runInput) Crouch(false);
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

		// Drag
		velocity -= velocity * 0.1f;

		// Gets a new velocity value based on the ground the player is on and with gravity.
		Vector3 newVelocity = velocity;
		RaycastHit hitinfo;
		if(Physics.SphereCast(player.transform.position, player.radius, Vector3.down, out hitinfo, player.height/2))
		{
			// Makes sure ground incline isn't too steep
			float angle = Vector3.Angle(Vector3.up, hitinfo.normal);
			if(angle < player.slopeLimit)
				newVelocity = Vector3.ProjectOnPlane(velocity, hitinfo.normal);
			else newVelocity = Vector3.ProjectOnPlane(Physics.gravity, hitinfo.normal);
		}
		newVelocity += Physics.gravity;

		// Moves the player with the new velocity
		player.Move(newVelocity * Time.fixedDeltaTime);
	}

	/// <summary>
	/// Takes user input to change player velocity
	/// </summary>
	void MoveInput()
	{
		// Gets the forward and right vectors of the camera
		Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
		Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;

		// Gets the movement input from the user
		float fwdInput = Input.GetAxisRaw("Vertical");		// Corresponds to the forward direction of the camera
		float rightInput = Input.GetAxisRaw("Horizontal");	// Corresponds to the right direction of the camera

		// Adds in controller input. Not a great way to do this but whatever.
		fwdInput += Input.GetAxisRaw("Joy Vertical");
		rightInput += Input.GetAxisRaw("Joy Horizontal");

		// Adds input movement to the velocity
		if(player.isGrounded)
			velocity += (forward * fwdInput + right * rightInput) * acceleration * Time.fixedDeltaTime;
	}



















	/// <summary>
	/// Checks for crouch input, then checks if player is crawling
	/// </summary>
	void CrouchInput()
	{
		if(Input.GetButtonDown("Crouch") && !Input.GetButton("Run"))
			Crouch(crouchState == 3);	// If player is standing, they will crouch, and vice versa
		if(crouchState < 3)
			Crawl();
	}

	/// <summary>
	/// Toggles croutching on and off
	/// </summary>
	/// <param name="crouch">Whether or not you want the player to crouch</param>
	void Crouch(bool crouch)
	{
		float camCrouchOffset = 0.5f;

		if(crouchState == 1)								// If crawling, player cannot stand so don't do anything
		{
			return;
		} else if(crouch == true && crouchState == 3)		// If intending to crouch and currently standing
		{
			player.height /= 3;								// Halve player height and
			player.center -= new Vector3(0, player.height, 0);	// Move the collider center down
			crouchState = 2;
			cameraAxis.transform.position -= Vector3.up * camCrouchOffset;
		} else if (crouch == false && crouchState != 3)		// If intending to stand and currently crouching
		{													// Return player height and center to normal
			player.center += new Vector3(0, player.height, 0);
			player.height *= 3;	
			crouchState = 3;
			cameraAxis.transform.position += Vector3.up * camCrouchOffset;
		}
	}

	/// <summary>
	/// Checks whether player should be crawling, turns crawling on and off
	/// </summary>
	void Crawl()
	{
		// Adding colliders to the character will probably break this
		// Perhaps do something with layers

		// This uses a thingy* to check if there is anything right above the crouched player,
		// signaling that they need to be crawling

		// *It's like a raycast, 'cept thicker

		RaycastHit hitInfo;
		Vector3 top = player.transform.position + player.center + Vector3.down * player.height/2;
		float maxDist = 0.6f;
		float radius = 0.8f;

		// Keep these Debug lines in case they're needed for fiddling later
		// Debug.DrawLine(top, top + Vector3.up * maxDist, Color.blue);
		// Debug.DrawLine(top, top + Vector3.right * radius, Color.green);
		// Debug.DrawLine(top + Vector3.up * maxDist, top + Vector3.up * maxDist + Vector3.up * radius, Color.green);

		if(Physics.SphereCast(top, radius, Vector3.up, out hitInfo, maxDist, bitFieldAllLayers, QueryTriggerInteraction.Ignore))
		{
			crouchState = 1;
		} else {
			crouchState = 2;
		}
	}









	/// <summary>
	/// Cameras the turn. Thanks, Unity auto-summarizer.
	/// </summary>
	void CameraTurn()
	{
		// Gets the user mouse input
		float horiz = Input.GetAxis("Mouse X") * camXSpeed;
		float vert = Input.GetAxis("Mouse Y") * -camYSpeed;

		horiz += Input.GetAxis("Joy Look X") * camXSpeed;
		vert += Input.GetAxis("Joy Look Y") * -camYSpeed;

		// Gets a Vector3 from the Camera Axis' rotation quaternion, for modification
		Vector3 rotVector = cameraAxis.rotation.eulerAngles;

		// Rotates the vector with the user mouse input
		rotVector += new Vector3(vert, horiz, 0);
		// Zeroes out the Z-axis to avoid camera tilt/roll and general mayhem
		rotVector.z = 0;

		// Limits the X-axis from going too far up or down to avoid other general mayhem
		if(rotVector.x < 300.0f && rotVector.x > 180.0) rotVector.x = 300.0f;
		if(rotVector.x > 60.0f && rotVector.x < 180.0) rotVector.x = 60.0f;

		// Sets the Camera Axis' rotation with the new rotation vector
		cameraAxis.rotation = Quaternion.Euler(rotVector);

		if(firstPerson)
		{
			// If in First-Person mode,
			// puts the camera directly into the character's skull
			cam.transform.position = fpsCamTarget.position;
			// And constrain's its rotation to that of the Camera Axis
			cam.transform.rotation = cameraAxis.rotation;
		} else {
			// If in Third-Person mode,
			// Offsets the X value of the camera based on where it's looking
			CamXOffset();
			// Makes the camera smoothly move towards the Camera Target
			CameraFollow();
			// Makes the camera always able to see the player character
			CameraAgainstWalls();
		}
	}

	/// <summary>
	/// Offsets the Camera's x value based on its y rotation.
	/// If looking over shoulder from behind, will be a little to the right.
	/// If in front of character looking at them, is centered.
	/// </summary>
	void CamXOffset()
	{
		Vector3 playerFwd = anim.transform.forward;
		Vector3 camFwd = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
		float dot = Vector3.Dot(playerFwd, camFwd);

		Vector3 pos = cameraTarget.transform.localPosition;
		pos.x = camXOffset * (dot + 1f)/2f;
		cameraTarget.transform.localPosition = pos;
	}

	/// <summary>
	/// Moves the camera smoothly to the target position
	/// </summary>
	void CameraFollow()
	{
		cam.transform.position = Vector3.Lerp(
			cam.transform.position,
			cameraTarget.position,
			0.2f
		);
		cam.transform.rotation = Quaternion.Lerp(
			cam.transform.rotation,
			cameraTarget.rotation,
			0.2f
		);
	}

	/// <summary>
	/// Makes sure character is not obscured from camera.
	/// If a wall (or something) gets between player and camera,
	/// camera moves past the wall to see player.
	/// </summary>
	void CameraAgainstWalls()
	{
		RaycastHit hitInfo;
		if(Physics.Raycast(headBone.position, cam.transform.position - headBone.position, out hitInfo, 100, bitFieldAllLayers, QueryTriggerInteraction.Collide))
		{
			float offset = 0.04f;

			// This is probably a bad solution to an issue
			if(hitInfo.collider.gameObject.GetComponent<CharacterController>() != null)
				return;

			if(hitInfo.collider.gameObject != cam.gameObject)
			{
				//Debug.Log("Obstructing object: " + hitInfo.collider.gameObject.name);
				cam.transform.position = hitInfo.point + hitInfo.normal * offset;
			}
		}
	}








	/// <summary>
	/// Attacks vis user input
	/// </summary>
	void AttackInput()
	{
		if(Input.GetButtonDown("Attack"))
		{
			if(combatScript == null) return;
			combatScript.Attack();
		}
	}










	/// <summary>
	/// Turns the player model, corresponding to its motion
	/// </summary>
	void TurnPlayerModel()
	{
		if(alwaysFaceForward)
		{
			// Turns the player model to always face the direction of the camera,
			// with their back to it
			// This is primarily for aiming weapons
			// With this, the player will strafe and walk backwards
			anim.transform.forward = Vector3.Lerp(
				anim.transform.forward,
				Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up),
				0.2f
			);
		} else {
			// Always turns the player model in the direction it is moving
			// Used for when player is not aiming anything, just running around
			anim.transform.forward = Vector3.Lerp(
				anim.transform.forward,
				velocity,
				0.1f
			);
		}
	}

	/// <summary>
	/// Animate the player model.
	/// </summary>
	void Animate()
	{
		// Turns the player model in the correct direction
		TurnPlayerModel();

		// Sends necessary values to the Animator
		// Speed
		float speed = velocity.magnitude;
		anim.SetFloat("Speed", speed);
		// Crouching state
		anim.SetInteger("CrouchState", crouchState);
		// Angle Between forward facing direction and velocity direction
		float angleBetween = Vector3.Angle(anim.transform.forward, velocity);
		anim.SetFloat("Angle", angleBetween);
		// Right Vector Dot Product (determines whether velocity is moving to right)
		float rightDot = Vector3.Dot(anim.transform.right, velocity);
		anim.SetFloat("RightDot", rightDot);
	}














	/// <summary>
	/// Locks the mouse to the screen and hides it
	/// </summary>
	void LockMouse()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}






	/// <summary>
	/// Sets the aim modm
	/// </summary>
	/// <param name="aim">If set to <c>true</c>, character always faces camera direction.</param>
	public void SetAimMode(bool aim)
	{
		if(firstPerson) return;
		alwaysFaceForward = aim;
	}




	/// <summary>
	/// Swaps the characters via input
	/// </summary>
	void SwapCharactersInput()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			SwapCharacters();
		}
	}

	/// <summary>
	/// Swaps the characters.
	/// </summary>
	void SwapCharacters()
	{
		if(playerList.Length == 0) return;

		// Stops character from moving
		velocity = Vector3.zero;
		// Sets the animator to stop moving
		Animate();

		// Moves to the next player
		++playerNum;
		if(playerNum >= playerList.Length) playerNum = 0;
		player = playerList[playerNum];
		SetPlayerVars();
		// Check animation state for crouch state
		crouchState = anim.GetInteger("CrouchState");
	}







	/// <summary>
	/// Switched between First- and Third-Person modes
	/// </summary>
	void SwapFirstThirdPerson()
	{
		// TODO: Make this a Button in the Input Menu, rather than a KeyCode

		if(Input.GetKeyDown(KeyCode.F))
		{
			firstPerson = !firstPerson;
			if(firstPerson)
			{
				// In addition to being necessary for aiming, this is also needed for First Person view
				alwaysFaceForward = true;
				// Disables the camera collider because it triggers crawling when in the model's head
				cam.GetComponent<Collider>().enabled = false;
			} else {
				alwaysFaceForward = false;
				cam.GetComponent<Collider>().enabled = true;
			}
		}
	}

	public CharacterController Player {
		get { return player; }
	}

	/// <summary>
	/// Removes the player from playerList.
	/// </summary>
	/// <param name="deadPlayer">Dead player.</param>
	public void RemovePlayerFromList(CharacterController deadPlayer)
	{
		CharacterController[] newList = new CharacterController[playerList.Length - 1];
		int count = 0;
		for(int n = 0; n < playerList.Length; ++n)
		{
			if(playerList[n] != deadPlayer)
			{
				newList[count] = playerList[n];
				++count;
			}
		}
		playerList = newList;
	}







}
