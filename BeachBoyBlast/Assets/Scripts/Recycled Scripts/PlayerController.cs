using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (RaycastController))]

public class PlayerController : MonoBehaviour {

    public enum PlayerNum     {         Player1,         Player2     }

    private GameObject[] players = new GameObject[2];
    private List<BoxCollider2D> playerColliders = new List<BoxCollider2D>();

    public PlayerNum playerNum;
    //string self;
    string enemy;     string color;     string rightHorizontal;     string rightVertical;     string dash;     string teleport;
    string horizontalMove;     string verticalMove;     string jump;

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
        players[0] = GameObject.FindGameObjectWithTag("Player1");
        players[1] = GameObject.FindGameObjectWithTag("Player2");
		// Declare and calculate gravity
		gravity = -(2 * (maxJumpHeight) / Mathf.Pow(timeToMaxJump, 2));
		maxJumpvelocity = Mathf.Abs(gravity) * timeToMaxJump;
		minJumpvelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);


         if(playerNum == PlayerNum.Player1){             horizontalMove = "Horizontal";             verticalMove = "Vertical";             jump = "Jump";            // self = "Player1";             enemy = "Player2";             color = "Red";             rightHorizontal = "RightHorizontal";             rightVertical = "RightVertical";             dash = "Dash";             teleport = "Teleport";           }
        else if(playerNum == PlayerNum.Player2){             horizontalMove = "P2Horizontal";             verticalMove = "P2Vertical";             jump = "P2Jump"; //needs to be different             //self = "Player2";             enemy = "Player1";             color = "Blue";             rightHorizontal = "P2RightHorizontal";             rightVertical = "P2RightVertical";             dash = "P2Dash";             teleport = "P2Teleport";}


        for (int i = 0; i < players.Length; i++)
        {
            playerColliders.Add(players[i].GetComponent<BoxCollider2D>());
            //Debug.Log(players[i].name);
        }

	}

	// Update is called once per frame
	void Update () {
		// Get movement axis
		input2D = new Vector2 (Input.GetAxisRaw (horizontalMove), Input.GetAxisRaw (verticalMove));

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

            if (Input.GetAxis(horizontalMove) != 0.0f) {

			}
		}
	}
		
	public void Jump() {
		if (Input.GetButtonDown (jump)) {

			if (handler.collisions.below) {
				
				velocity.y = maxJumpvelocity;
			}
		}

		if (Input.GetButtonUp (jump)) {
			
			if (velocity.y > minJumpvelocity) {
				
				velocity.y = minJumpvelocity;
			}
		}
	}

	public void Reset() {
		if (velocity.x <= 0.01f && velocity.x >= -0.1f) {
			velocity.x = 0;
		}
	}
}
