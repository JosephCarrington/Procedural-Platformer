using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
	Rigidbody2D body;
	public float speed;
	public float jumpStrength = 10f;
	public float jumpVerticleStrength = 1f;
	bool grounded = false,
		lefted = false,
		righted = false;
	GameObject groundCheck,
		leftCheck,
		rightCheck;

	public float airSpeed = 20f;

	public float extraJumpTime = 1f;
	public float extraJumpStrength = 1f;

	void Start () {
		body = gameObject.GetComponent<Rigidbody2D> ();
		foreach (Transform child in transform) {
			switch (child.name) {
			case "GroundCheck":
				groundCheck = child.gameObject;
				break;
			case "LeftCheck":
				leftCheck = child.gameObject;
				break;
			case "RightCheck":
				rightCheck = child.gameObject;
				break;
			}
		}
	}
	
	// Update is called once per frame
	float h;
	bool jump = false;
	bool jumping = false;
	void Update() {
		if (Input.GetKeyDown (KeyCode.R)) {
			Die ();
		}
		h = Input.GetAxis ("Horizontal");
		jump = Input.GetButton ("Jump");
		if(Input.GetButtonUp("Jump")) {
			jumping = false;
		}
	}

	float currentVel;
	public float moveSmoothTime = 0.1f;

	private float lastJumpTime;
	public float wallJumpControlLoss = 0.25f;

	public AnimationCurve wallJumpControlCurve;
	void FixedUpdate () {
		// GET SOME VARS //
		Vector2 newVel = body.velocity;
		grounded = groundCheck.GetComponent<CheckController> ().Check ();
		lefted = leftCheck.GetComponent<CheckController> ().Check ();
		righted = rightCheck.GetComponent<CheckController> ().Check ();

		// FLOOR MOVEMENT ONLY //
		if (grounded) {
			newVel.x = h * speed;
		}

		// JUMPING ONLY //
		if (jump && (grounded || lefted || righted) && !jumping) {
			lastJumpTime = Time.time;
			jumping = true;
			if (lefted) {
				newVel.y = jumpStrength;
				newVel.x = jumpVerticleStrength;
			} else if (righted) {
				newVel.y = jumpStrength;
				newVel.x = -jumpVerticleStrength;
			} else {
				newVel.y = jumpStrength;

			}
		}
		if ((!jump && !jumping) && (lefted || righted)) {
			// WE ARE PRESSING ON A WALL
			newVel.x = h * speed;
		}

		// WALL JUMP LOSS OF CONTROL ONLY //
		if (!grounded && !lefted && !righted) {
			// We are in the air somewhere
			if (Time.time > lastJumpTime + wallJumpControlLoss) {
				// Complete control of air movement
				newVel.x =  h * speed;
			} else {
				// HERE LIE DRAGONS //
				newVel.x = Mathf.Lerp(newVel.x, h * speed, wallJumpControlCurve.Evaluate(GetBetweenValue(lastJumpTime, lastJumpTime + wallJumpControlLoss, Time.time)));
			}
		}
			
		// Add extra jump strength
		if (jump && jumping) {
			if (Time.time < lastJumpTime + extraJumpTime) {
				newVel.y += extraJumpStrength;
			}
		}
		body.velocity = newVel;
	}
		
	float GetBetweenValue(float min, float max, float inputValue) {
		return(inputValue - min) / (max - min);
	}

	public float stamina = 40f;
	void OnCollisionEnter2D(Collision2D col) {
		if (col.relativeVelocity.magnitude > stamina) {
			Die ();
		}

	}

	void Die() {
		Application.LoadLevel (0);
	}
}
