using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Raycast2DHandler : UniversalRaycastHandler {

	public float maxClimbingAngle = 60;
	public float maxDescendAngle = 60;

	public CollisionInfo collisions2D;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start(){
		base.Start ();
		collisions2D.faceDirection = 1;
	}

	// B I G G E S T I M P O R T A N T
	public void Move(Vector3 velocity2D, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions2D.Reset ();
		collisions2D.velocity2DOld = velocity2D;

		if (velocity2D.x != 0) {
			collisions2D.faceDirection = (int)Mathf.Sign (velocity2D.x);
		}

		if (velocity2D.y < 0) {
			DescendSlope (ref velocity2D);
		}

		HorizontalCollisions (ref velocity2D);
		if (velocity2D.y != 0) {
			VerticalCollisions (ref velocity2D);
		}
			
		transform.Translate (velocity2D);

		if (standingOnPlatform == true) {
			collisions2D.below = true;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="velocity2D">Velocity2 d.</param>
	void HorizontalCollisions(ref Vector3 velocity2D) {
		float directionX = collisions2D.faceDirection;
		float rayLength = Mathf.Abs (velocity2D.x) + skinWidth;

	if (Mathf.Abs(velocity2D.x) < skinWidth) {
		rayLength = 2 * skinWidth;
	}

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit2D = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask); 

			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit2D) {

				float slopeAngle = Vector2.Angle (hit2D.normal, Vector2.up);

				if (hit2D.distance == 0) {
					continue;
				}

				if (i == 0 && slopeAngle <= maxClimbingAngle) {
					if (collisions2D.descendingSlope) {
						collisions2D.descendingSlope = false;
						velocity2D = collisions2D.velocity2DOld;
					}
						
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions2D.slopeAngleOld) {
						distanceToSlopeStart = hit2D.distance - skinWidth;
						velocity2D.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity2D, slopeAngle);
					velocity2D.x += distanceToSlopeStart * directionX;
				}

				if (!collisions2D.climbingSlope || slopeAngle > maxClimbingAngle) {
					velocity2D.x = (hit2D.distance - skinWidth) * directionX;
					rayLength = hit2D.distance;

					collisions2D.left = directionX == -1;
					collisions2D.right = directionX == 1;
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="velocity2D">Velocity2 d.</param>
	void VerticalCollisions(ref Vector3 velocity2D) {
		float directionY = Mathf.Sign (velocity2D.y);
		float rayLength = Mathf.Abs (velocity2D.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity2D.x);
			RaycastHit2D hit2D = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask); 

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit2D) {
				velocity2D.y = (hit2D.distance - skinWidth) * directionY;
				rayLength = hit2D.distance;

				if (collisions2D.climbingSlope) {
					velocity2D.x = velocity2D.y / Mathf.Tan (collisions2D.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity2D.x);
				}

				collisions2D.below = directionY == -1;
				collisions2D.above = directionY == 1;
			}
		}

		if (collisions2D.climbingSlope) {
			float directionX = Mathf.Sign (velocity2D.x);
			rayLength = Mathf.Abs (velocity2D.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity2D.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions2D.slopeAngle) {
					velocity2D.x = (hit.distance - skinWidth) * directionX; 
					collisions2D.slopeAngle = slopeAngle;
				}
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="velocity2D">Velocity2 d.</param>
	void ClimbSlope(ref Vector3 velocity2D, float slopeAngle){
		float moveDistance = Mathf.Abs (velocity2D.x);
		float climbingVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
			
		if (velocity2D.y <= climbingVelocityY) {
			velocity2D.y = climbingVelocityY;
			velocity2D.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity2D.x);

			collisions2D.below = true;
			collisions2D.climbingSlope = true;
			collisions2D.slopeAngle = slopeAngle;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="velocity2D">Velocity2 d.</param>
	void DescendSlope(ref Vector3 velocity2D){
		float directionX = Mathf.Sign (velocity2D.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity2D.x)) {
						float moveDistance = Mathf.Abs (velocity2D.x);
						float descendingVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity2D.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity2D.x);
						velocity2D.y -= descendingVelocityY;

						collisions2D.slopeAngle = slopeAngle;
						collisions2D.descendingSlope = true;
						collisions2D.below = true;
					}
				}
			}
		}
	}

	// Unsure what struct exactly does. Read the documentation but was no quite sure if they
	// were better than a function for this purpose. But this is a small data structure so
	// struct is best apparently?
	public struct CollisionInfo {
		public int faceDirection;

		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;

		public float slopeAngle, slopeAngleOld;

		public Vector3 velocity2DOld;

		public void Reset(){
			
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
