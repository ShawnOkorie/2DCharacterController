using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class S_CharacterController2D : MonoBehaviour
{
    //Refrences
    private BoxCollider2D collider2D;
    private RaycastOrigins raycastOrigins;
    public CollisionInfo collisionInfo;
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
        collisionInfo.Reset();
        
        //doesnt calculate if no input
        if(velocity.x != 0) HorizontalColisisons(ref velocity); 
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
            //same as if(directionX == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastOrigins.bottomRight
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin ,Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                if (directionX == -1) collisionInfo.left = true;
                else if(directionX == 1) collisionInfo.right = true;
            }
        }
    }
    
    private void VerticalColisisons(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        
        for (int i = 0; i < verticalRayCount; i++)
        {
            // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            
            Debug.DrawRay(rayOrigin ,Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (directionY == -1) collisionInfo.below = true;
                else if (directionY == 1) collisionInfo.above = true;
            }
        }
    }

    #endregion
    
    #region Collision Info

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
        }
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
        //shrinking Bounds by skinWidth
        Bounds bounds = collider2D.bounds;
        bounds.Expand(skinWidth * -2); 

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
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
