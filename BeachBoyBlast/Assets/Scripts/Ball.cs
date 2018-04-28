using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	public PlayerController playerController;
	public float yForce;

	float playerVelocity;
	float xForceDependOnPlayer;

	Rigidbody2D rigidBody2D;

	// Use this for initialization
	void Start () {
		rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		xForceDependOnPlayer = playerController.velocity.x;
		Debug.Log (xForceDependOnPlayer);
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Head") {
			rigidBody2D.AddForce (new Vector3(xForceDependOnPlayer, yForce, 0), ForceMode2D.Impulse);
		}
	}
}
