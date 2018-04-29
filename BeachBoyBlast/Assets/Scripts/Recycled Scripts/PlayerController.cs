using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (RaycastController))]

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
	float gravity;
	float maxJumpvelocity;
	float minJumpvelocity;
	float velocityXSmoothing;

	// T H E M O S T I M P O R T A N T
	public Vector3 velocity;
	// S E C O N D I M P O R T A N T
	public Vector2 input2D;

	// public Animator animator;

	public SpriteRenderer spriteRenderer;

	public RaycastController handler;
	BoxCollider2D boxCollider;

	// Use this for initialization
	void Start () {
		handler = GetComponent<RaycastController> ();
		boxCollider = GetComponent<BoxCollider2D> ();
		// Declare and calculate gravity
		gravity = -(2 * (maxJumpHeight) / Mathf.Pow(timeToMaxJump, 2));
		maxJumpvelocity = Mathf.Abs(gravity) * timeToMaxJump;
		minJumpvelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	// Update is called once per frame
	void Update () {
		// Get movement axis
		input2D = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		float targetvelocityX = input2D.x * moveSpeed;

		velocity.x = Mathf.SmoothDamp (velocity.x, targetvelocityX, ref velocityXSmoothing, (handler.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

		// Grounded check
		if (handler.collisions.above || handler.collisions.below) {
			velocity.y = 0;
		}

		Jump ();

		Movement ();

		Reset();

		velocity.y += gravity * Time.deltaTime;
		handler.Move (velocity * Time.deltaTime);
	}
		
	public void Movement() {
		if (handler.collisions.below) {
			
			// animator.SetBool ("isGrounded", true);

			if (Input.GetAxis("Horizontal") != 0.0f) {
				
				// animator.SetBool ("isMoving", true);
			}
		}
	}
		
	public void Jump() {
		if (Input.GetButtonDown ("Jump")) {
			
			// animator.SetBool ("isGrounded", false);

			if (handler.collisions.below) {
				
				velocity.y = maxJumpvelocity;
			}
		}

		if (Input.GetButtonUp ("Jump")) {
			
			if (velocity.y > minJumpvelocity) {
				
				velocity.y = minJumpvelocity;
			}
		}
	}

	public void Reset() {
		if (velocity.x <= 0.01f && velocity.x >= -0.1f) {
			velocity.x = 0;
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
