using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour {
    internal const float skinWidth = .015f;          //The width of the inset from where the rays are cast.

    [Header("Raycast Settings")]
    [Tooltip("The layers the player will collide with.")]
    public LayerMask collisionMask;         //The layers the player will collide with.

    public int horizontalRayCount = 4;      //The ammount of rays to be cast on the horizontal directions.
    public int verticalRayCount = 4;        //The ammount of rays to be cast in the verticel directions.

    internal float horizontalRaySpacing;
    internal float verticalRaySpacing;

    internal BoxCollider2D collider;
    internal RaycastOrigins raycastOrigins;


    public virtual void Awake()
    {
        //----------Referances------------
        collider = GetComponent<BoxCollider2D>();
        //--------------------------------
    }//Awake

    public virtual void Start()
    {
        CalculateRaySpacing();
    }//Start

    /// <summary>
    /// Raycast origin will be inset by small ammount to allow casting when player object is flat agains a surface.
    /// </summary>
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }//UpdateRaycastOrigins

    /// <summary>
    /// Minimum of 2 rays are casted and the space between the rays is calculated.
    /// </summary>
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }//CalculateRaySpacing





    /// <summary>
    /// Will Store corners of player collider in vector form.
    /// These vectors will be where the raycasts emit from.
    /// </summary>
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }//RaycastOrigins Scruct
}
