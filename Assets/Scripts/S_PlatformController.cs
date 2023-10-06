using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class S_PlatformController : S_RaycastController
{
    [SerializeField] private LayerMask passengerMask;
    
    [SerializeField] private Vector3 platformMovement;
    
    protected override void Start()
    {
        
    }
    
    private void Update()
    {
        Vector3 velocity = platformMovement * Time.deltaTime;
        
        MovePassengers(velocity);
        transform.Translate(velocity);
    }

    private void MovePassengers(Vector3 velocity)
    {
        HashSet<Transform> movedPassangers = new HashSet<Transform>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        
        //Vertically moving Platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.blue);

                if (hit)
                {
                    //prevent the same passenger from having velocity added for each raycast hit
                    if (!movedPassangers.Contains(hit.transform))
                    {
                        movedPassangers.Add(hit.transform);
                        
                        // x only affected if platform moves up
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        //subtract distance between player and platform 
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                    
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }

        //horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);
                
                if (hit)
                {
                    //prevent the same passenger from having velocity added for each raycast hit
                    if (!movedPassangers.Contains(hit.transform)) 
                    {
                        movedPassangers.Add(hit.transform);
                        
                        //subtract distance between player and platform 
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = 0;
                    
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
        
        //player on top of horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

                if (hit)
                {
                    //prevent the same passenger from having velocity added for each raycast hit
                    if (!movedPassangers.Contains(hit.transform))
                    {
                        movedPassangers.Add(hit.transform);
                        
                        float pushX = velocity.x;
                        float pushY = velocity.y;
                    
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
    }
}