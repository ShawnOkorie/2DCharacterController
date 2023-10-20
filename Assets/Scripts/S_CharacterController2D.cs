using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class S_CharacterController2D : S_RaycastController
{
    public CollisionInfo collisions;
    
    private float maxClimbAngle = 75;
    private float maxDecendAngle = 75;

    private bool standingOnPlatform;

    protected override void Start()
    {
        base.Start();

        collisions.faceDirection = 1;
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        this.standingOnPlatform = standingOnPlatform;

        if (velocity.x != 0) collisions.faceDirection = (int) Mathf.Sign(velocity.x);
            
        if(velocity.y < 0) DescendSlope(ref velocity);
        HorizontalColisisons(ref velocity); 
        if(velocity.y != 0 || !this.standingOnPlatform) VerticalColisisons(ref velocity); // 

        transform.Translate(velocity);

        if (this.standingOnPlatform) collisions.below = true;
    }

    #region Slopes

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity. x);
        
        // prevent overwriting of velocity y so jump is possible
        float climbVelocitY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocitY)
        {
            velocity.y = climbVelocitY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            
            collisions.below = true;
            collisions.isClimbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= maxDecendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float decendVelocitY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= decendVelocitY;

                        collisions.below = true;
                        collisions.isDecendingSlope = true;
                        collisions.slopeAngle = slopeAngle;
                    }
                }
            }
        }
    }

    #endregion
    

    #region Collision Detection

    private void HorizontalColisisons(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth) rayLength = skinWidth * 2;
        
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //same as if(directionX == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastOrigins.bottomRight
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                if (hit.distance == 0) continue;
                
                //slope angle = surface normal and global up
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.isDecendingSlope)
                    {
                        //reset velocity to previous value if starting to climb while descending to prvent slowdown
                        collisions.isDecendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    
                    float distanceToSlopeStart = 0;
                    
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    
                    ClimbSlope(ref velocity, slopeAngle);

                    velocity.x += distanceToSlopeStart * directionX;
                }
                
                if (!collisions.isClimbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.isClimbingSlope) velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    
                    if (directionX == -1) collisions.left = true;
                    else if(directionX == 1) collisions.right = true;
                }
                    
            }
        }
    }
    private void VerticalColisisons(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth ;
        
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

                if (collisions.isClimbingSlope) velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                
                if (directionY == -1) collisions.below = true;
                else if (directionY == 1) collisions.above = true;
            }
        }

        // account for slope angle change while climbing
        if (collisions.isClimbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;

            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    #endregion
    
    #region Collision Info

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool isClimbingSlope;
        public bool isDecendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public int faceDirection;

        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            isClimbingSlope = false;
            isDecendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    #endregion    
}
