using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class S_RaycastController : MonoBehaviour
{
    //References
    protected RaycastOrigins raycastOrigins;
    public LayerMask collisionMask;
    
    //Variables
    protected BoxCollider2D collider2D;
    protected const float skinWidth = 0.015f;
    
    [SerializeField] protected int horizontalRayCount = 4;
    [SerializeField] protected int verticalRayCount = 4;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;
    
    protected virtual void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        
        CalculateRaySpacing();
    }
    
    #region RaycastOrigins
   
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    protected void UpdateRaycastOrigins()
    {
        //shrinking Bounds by skinWidth
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2); 

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected void CalculateRaySpacing()
    {
        //shrinking Bounds by skinWidth
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2); 

        //Setting Limit of 2 min Rays
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue); 
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    #endregion
}
