﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : UniversalRaycastHandler {

	public float maxClimbingAngle = 60;
	public float maxDescendAngle = 60;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start(){
		base.Start ();
		collisions.faceDirection = 1;
	}

	// B I G G E S T I M P O R T A N T
	public void Move(Vector3 velocity, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;

		if (velocity.x != 0) {
			collisions.faceDirection = (int)Mathf.Sign (velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope (ref velocity);
		}

		HorizontalCollisions (ref velocity);
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}
			
		transform.Translate (velocity);

		if (standingOnPlatform == true) {
			collisions.below = true;
		}
	}
		
	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDirection;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

	if (Mathf.Abs(velocity.x) < skinWidth) {
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
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
						
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit2D.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbingAngle) {
					velocity.x = (hit2D.distance - skinWidth) * directionX;
					rayLength = hit2D.distance;

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}
		
	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit2D = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask); 

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit2D) {
				velocity.y = (hit2D.distance - skinWidth) * directionY;
				rayLength = hit2D.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX; 
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}
		
	void ClimbSlope(ref Vector3 velocity, float slopeAngle){
		float moveDistance = Mathf.Abs (velocity.x);
		float climbingVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
			
		if (velocity.y <= climbingVelocityY) {
			velocity.y = climbingVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);

			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}
		
	void DescendSlope(ref Vector3 velocity){
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x)) {
						float moveDistance = Mathf.Abs (velocity.x);
						float descendingVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendingVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}
		
	public struct CollisionInfo {
		public int faceDirection;

		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;

		public float slopeAngle, slopeAngleOld;

		public Vector3 velocityOld;

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
