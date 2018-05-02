using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour {

	public PlayerController playerController1;

	public RaycastController raycastController1;

	public float yForce;

	public float yForceMax;

	public string thePointGoesTo;

	public Text scoreTextPlayer1;

	float playerVelocity;
	float xForceDependOnPlayer1;

	bool addForceCheck;

	Rigidbody2D rigidBody2D;

    public Transform arrow;

 


	// Use this for initialization
	void Start () {
		rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 arrowPosition = new Vector3(transform.position.x, 4.5f, 0f);	

		Debug.Log (thePointGoesTo);

		if (playerController1.handler.collisions.below) {
			addForceCheck = true;
		}

        arrow.position = arrowPosition;
        arrow.gameObject.SetActive(false);         if(transform.position.y >= 5){             arrow.gameObject.SetActive(true);         }  
	}

	void FixedUpdate()
	{
		if (rigidBody2D.velocity.magnitude > yForceMax)
		{
			rigidBody2D.velocity = rigidBody2D.velocity.normalized * yForceMax;
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player 1 Head") {
			if (addForceCheck == true) {
				rigidBody2D.AddForce (new Vector3 (xForceDependOnPlayer1, yForce, 0), ForceMode2D.Impulse);
				addForceCheck = false;
				thePointGoesTo = "Player 1";
			}
		}

		if (collider.gameObject.tag == "Ground") {
			transform.position = new Vector3 (0.0f, 5.0f, 0.0f);

			rigidBody2D.velocity = new Vector2 (0.0f, 0.0f);
		}
	}
}
