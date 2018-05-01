using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	public PlayerController playerController;

	public RaycastController raycastController;

	public float yForce;

	public float yForceMax;

	float playerVelocity;
	float xForceDependOnPlayer;

	bool addForceCheck;

	Rigidbody2D rigidBody2D;

	// Use this for initialization
	void Start () {
		rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		xForceDependOnPlayer = playerController.velocity.x;

		if (playerController.handler.collisions.below) {
			addForceCheck = true;
		}
	}

	void FixedUpdate()
	{
		if (rigidBody2D.velocity.magnitude > yForceMax)
		{
			rigidBody2D.velocity = rigidBody2D.velocity.normalized * yForceMax;
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Head") {
			if (addForceCheck == true) {
				rigidBody2D.AddForce (new Vector3 (xForceDependOnPlayer, yForce, 0), ForceMode2D.Impulse);
				addForceCheck = false;
			}
		}

		if (collider.gameObject.tag == "Ground") {
			StartCoroutine ("Respawn_");
		}
	}

	IEnumerator Respawn_() {
		transform.position = new Vector3 (0.0f, 5.0f, 0.0f);

		rigidBody2D.velocity = new Vector2 (0.0f, 0.0f);

		yield return new WaitForSeconds (0.2f);
	}
}
