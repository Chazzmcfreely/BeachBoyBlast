using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class UniversalRaycastHandler : MonoBehaviour {

	public LayerMask collisionMask;

	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	public const float skinWidth = .015f;

	// The tutorial I'm following said to use [HideInInspector] but I don't think
	// I need to? What's the point of hiding it?
	public float horizontalRaySpacing;
	public float verticalRaySpacing;

	public RaycastOrigins raycastOrigins;
	public BoxCollider2D boxCollider2D;

	// Use this for initialization
	public virtual void Awake () {
		// Gets the boxCollider2D from the player so that the calculations below can work lol
		boxCollider2D = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}

	public virtual void Start(){
		// H U M O N G O U S I M P O R T A N T
		CalculateRaySpacing ();
	}

	// Determines the location for where the first ray will be fired
	public void UpdateRaycastOrigins () {
		Bounds bounds2D = boxCollider2D.bounds;
		bounds2D.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds2D.min.x, bounds2D.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds2D.max.x, bounds2D.min.y);

		raycastOrigins.topLeft = new Vector2 (bounds2D.min.x, bounds2D.max.y);
		raycastOrigins.topRight = new Vector2 (bounds2D.max.x, bounds2D.max.y);
	}

	// Given the bounds2D and the RayCount for both horizontal and vertical, calculates the
	// spacing between rach ray fired.
	public void CalculateRaySpacing() {
		Bounds bounds2D = boxCollider2D.bounds;
		bounds2D.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds2D.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds2D.size.x / (verticalRayCount - 1);
	}

	// Again with the struct. I learned about structs in Intermediate Programming a while ago, so after migrating
	// this code from Raycast2DHandler, and making it its own script, I thought I'd make this function a struct,
	// maybe if I want to add some enemies?
	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
