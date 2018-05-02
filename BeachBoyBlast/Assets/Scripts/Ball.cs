using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour {

	public PlayerController playerController1;
	public PlayerController playerController2;

	public RaycastController raycastController1;
	public RaycastController raycastController2;

	public float yForce;

	public float yForceMax;

	float playerVelocity;

	float xForceDependOnPlayer1;
	float xForceDependOnPlayer2;

	bool addForceCheck1;
	bool addForceCheck2;

	Rigidbody2D rigidBody2D;

    public Transform arrow;

	public GameManager gameManager;

	public ParticleSystem sparkleSystem1;
	public ParticleSystem sparkleSystem2;

	// Use this for initialization
	void Start () {
		rigidBody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (xForceDependOnPlayer2);

		xForceDependOnPlayer1 = playerController1.velocity.x;
		xForceDependOnPlayer2 = playerController2.velocity.x;

        Vector3 arrowPosition = new Vector3(transform.position.x, 4.5f, 0f);	
	
		if (playerController1.handler.collisions.below) {
			addForceCheck1 = true;
		}

		if (playerController2.handler.collisions.below) {
			addForceCheck2 = true;
		}
			
        arrow.position = arrowPosition;
        arrow.gameObject.SetActive(false);
        if(transform.position.y >= 5){
            arrow.gameObject.SetActive(true);
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
		if (collider.gameObject.tag == "Player 1 Head") {
			if (addForceCheck1 == true) {
				rigidBody2D.AddForce (new Vector3 (xForceDependOnPlayer1, yForce, 0), ForceMode2D.Impulse);
				sparkleSystem1.Emit (Random.Range (5, 8));
				addForceCheck1 = false;
			}
		}

		if (collider.gameObject.tag == "Player 2 Head") {
			if (addForceCheck2 == true) {
				rigidBody2D.AddForce (new Vector3 (xForceDependOnPlayer2, yForce, 0), ForceMode2D.Impulse);
				sparkleSystem2.Emit (Random.Range (5, 8));
				addForceCheck2 = false;
			}
		}

		if (collider.gameObject.tag == "Player 1 Zone") {
			gameManager.player1Score++;
		}

		if (collider.gameObject.tag == "Player 2 Zone") {
			gameManager.player2Score++;
		}

		if (collider.gameObject.tag == "Ground") {
			transform.position = new Vector3 (0.0f, 5.0f, 0.0f);

			rigidBody2D.velocity = new Vector2 (0.0f, 0.0f);
		}
	}
}
