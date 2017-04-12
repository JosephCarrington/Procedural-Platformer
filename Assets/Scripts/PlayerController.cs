using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utils;

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


		hpDisplay = GameObject.Find ("Hearts");
		hpDisplay.GetComponent<HeartPanelController> ().SetHeartCount (hp);

		map = GameObject.Find("TileMap").GetComponent<TileMapController>();

		regularColor = gameObject.GetComponent<SpriteRenderer> ().color;

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

	public int doubleJumps = 3;
	private float lastDoubleJumpTime = 0f;
	public float doubleJumpFallDeathDelay = 0.25f;

	public AnimationCurve wallJumpControlCurve;
	void FixedUpdate () {
		// GET SOME VARS //
		Vector2 newVel = body.velocity;
		if (Time.time < lastKnockBack + knockBackControlLoss) {
			return;
		}

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

//		// Double Jumping
//		if (doubleJumps > 0 && doubleJumping) {
//			doubleJumping = false;
//			lastJumpTime = Time.time;
//			lastDoubleJumpTime = Time.time;
//			newVel.y = jumpStrength * 2;
//			transform.Find ("DoubleJump Particles").GetComponent<ParticleSystem> ().Emit (10);
//			doubleJumps--;
//		}

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

	TileMapController map;
	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.layer == LayerMask.NameToLayer("BadWall")) {
			TakeDamage (1);
			Vector2 knockBackForce = gameObject.GetComponent<Rigidbody2D> ().velocity;
//			knockBackForce.x *= 2f;
			knockBackForce.y = 20f;
			KnockBack (knockBackForce);
		}
		if (col.relativeVelocity.magnitude > stamina) {
			if (Time.time > lastDoubleJumpTime + doubleJumpFallDeathDelay) {
				bool isWallBelow = map.IsWallAtCoords(
					new Coordinates(
						Mathf.RoundToInt(gameObject.transform.position.x) + 128,
						Mathf.RoundToInt(gameObject.transform.position.y) + 128 - 1
					)
				);
				if (isWallBelow) {
					TakeDamage (1);
					Vector2 knockBackStrength = gameObject.GetComponent<Rigidbody2D> ().velocity;
					knockBackStrength.x = -knockBackStrength.x;
					KnockBack (knockBackStrength);
				}
			}
		}
	}
//
//	void OnCollisionStay2D(Collision2D col) {
//		if (col.gameObject.layer == LayerMask.NameToLayer("BadWall")) {
//			TakeDamage (1);
//			Vector2 knockBackForce = gameObject.GetComponent<Rigidbody2D> ().velocity;
//			knockBackForce.x *= 2f;
//			knockBackForce.y = 20f;
//			KnockBack (knockBackForce);
//		}
//		if (col.relativeVelocity.magnitude > stamina) {
//			if (Time.time > lastDoubleJumpTime + doubleJumpFallDeathDelay) {
//				bool isWallBelow = map.IsWallAtCoords(
//					new Coordinates(
//						Mathf.RoundToInt(gameObject.transform.position.x) + 128,
//						Mathf.RoundToInt(gameObject.transform.position.y) + 128 - 1
//					)
//				);
//				if (isWallBelow) {
//					TakeDamage (1);
//					Vector2 knockBackStrength = gameObject.GetComponent<Rigidbody2D> ().velocity;
//					knockBackStrength.x = -knockBackStrength.x;
//					KnockBack (knockBackStrength);
//				}
//			}
//		}
//	}

	public float enemyBounceAmount = 10f;
	public void BounceOffEnemy(float factor) {
		Vector2 newVel = gameObject.GetComponent<Rigidbody2D> ().velocity;
		newVel.y = Mathf.Max(newVel.y, enemyBounceAmount * factor);
		gameObject.GetComponent<Rigidbody2D> ().velocity = newVel;
//		lastJumpTime = Time.time;
//		jumping = true;

	}

	public int hp = 1;
	private GameObject hpDisplay;
	public void TakeDamage(int amount) {
		if (Time.time < lastKnockBack + knockBackControlLoss) {
			// be invincible
			return;
		}
		transform.Find ("Blood Particles").GetComponent<ParticleSystem> ().Play ();
		hp -= amount;
		hpDisplay.GetComponent<HeartPanelController> ().SetHeartCount (hp);
		if (hp <= 0) {
			Die ();
		}
	}
	public float knockBackControlLoss = 0.5f;
	private float lastKnockBack;
	public void KnockBack(Vector2 knockBackForce) {
		if (Time.time < lastKnockBack + knockBackControlLoss) {
			// be invincible
			return;
		}
		StartCoroutine (BecomeInvincible ());
		Vector2 newVel = gameObject.GetComponent<Rigidbody2D> ().velocity;
		newVel.y = 0;
		gameObject.GetComponent<Rigidbody2D> ().velocity = newVel;
		gameObject.GetComponent<Rigidbody2D> ().AddForce (knockBackForce, ForceMode2D.Impulse);
		lastKnockBack = Time.time;
	}

	Color regularColor;
	public Color invincibleColor;
	IEnumerator BecomeInvincible() {
		gameObject.GetComponent<SpriteRenderer> ().color = invincibleColor;
		yield return new WaitForSeconds (knockBackControlLoss);
		gameObject.GetComponent<SpriteRenderer> ().color = regularColor;
	}


	private int score = 0;
	void Score(int scoreToAdd) {
		score += scoreToAdd;
		ShowScore ();
	}

	void ShowScore() {
		Camera.main.SendMessage ("SetScore", score);
	}

	void Die() {
		StartCoroutine (ReloadScene ());
	}

	IEnumerator ReloadScene(){
		yield return new WaitForSeconds (knockBackControlLoss);
		SceneManager.LoadScene(0);
	}
}
