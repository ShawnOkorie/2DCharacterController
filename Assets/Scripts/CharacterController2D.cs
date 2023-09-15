using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    //Refrences
    private BoxCollider2D collider2D;
    private RaycastOrigins raycastOrigins;
    public LayerMask collisionMask;
    
    //Variables
    private const float skinWidth = 0.015f;
    
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    private void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        
        CalculateRaySpacing();
    }
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        
        if(velocity.x != 0) HorizontalColisisons(ref velocity); //doesnt calculate if no input
        if(velocity.y != 0) VerticalColisisons(ref velocity);
        
        transform.Translate(velocity);
    }

    #region Collision Detection

    private void HorizontalColisisons(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; //same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastOrigins.bottomRight

            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin ,Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;
            }
        }
    }
    
    private void VerticalColisisons(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft

            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin ,Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
            }
        }
    }

    #endregion


    #region Collision Info

    public struct CollisionInfo
    {
        
    }

    #endregion    
    
    #region RaycastOrigins
   
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2); //shrinking Bounds by skinWidth

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2); //shrinking Bounds by skinWidth

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue); //Setting Limit of 2 min Rays
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / horizontalRayCount - 1;
        verticalRaySpacing = bounds.size.x / verticalRayCount - 1;
    }
    #endregion
   
}
