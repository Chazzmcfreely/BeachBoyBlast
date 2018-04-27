using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Raycast2DHandler))]

public class PlayerController : MonoBehaviour {

	// Public float declaration
	public float moveSpeed = 6;
	public float maxJumpHeight = 5;
	public float minJumpHeight = 1;
	public float timeToMaxJump = .5f;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;

	// Jumping variables
	float upTime;
	float gravity2D;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;

	// T H E M O S T I M P O R T A N T
	public Vector3 velocity2D;
	// S E C O N D I M P O R T A N T
	public Vector2 input2D;

	// public Animator animator;

	public SpriteRenderer spriteRenderer;

	Raycast2DHandler handler;
	BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		handler = GetComponent<Raycast2DHandler> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		// Declare and calculate gravity
		gravity2D = -(2 * (maxJumpHeight) / Mathf.Pow(timeToMaxJump, 2));
		maxJumpVelocity = Mathf.Abs(gravity2D) * timeToMaxJump;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity2D) * minJumpHeight);
	}

	// Update is called once per frame
	void Update () {
		// Get movement axis
		input2D = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		float targetVelocityX = input2D.x * moveSpeed;

		velocity2D.x = Mathf.SmoothDamp (velocity2D.x, targetVelocityX, ref velocityXSmoothing, (handler.collisions2D.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

		// Grounded check
		if (handler.collisions2D.above || handler.collisions2D.below) {
			velocity2D.y = 0;
		}

		Jump ();

		Movement ();

		Reset();

		velocity2D.y += gravity2D * Time.deltaTime;
		handler.Move (velocity2D * Time.deltaTime);
	}
		
	public void Movement() {
		if (handler.collisions2D.below) {
			
			// animator.SetBool ("isGrounded", true);

			if (Input.GetAxis("Horizontal") != 0.0f) {
				
				// animator.SetBool ("isMoving", true);
			}
		}
	}
		
	public void Jump() {
		if (Input.GetButtonDown ("Jump")) {
			
			// animator.SetBool ("isGrounded", false);

			if (handler.collisions2D.below) {
				
				velocity2D.y = maxJumpVelocity;
			}
		}

		if (Input.GetButtonUp ("Jump")) {
			
			if (velocity2D.y > minJumpVelocity) {
				
				velocity2D.y = minJumpVelocity;
			}
		}
	}

	public void Reset() {
		if (velocity2D.x <= 0.01f && velocity2D.x >= -0.1f) {
			velocity2D.x = 0;
			// animator.SetBool ("isMoving", false);
		}

		if(Input.GetAxis("Horizontal") == 1.0f) {
			spriteRenderer.flipX = false;
		} else if (Input.GetAxis("Horizontal") == -1.0f) {
			spriteRenderer.flipX = true;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		
	}
}
