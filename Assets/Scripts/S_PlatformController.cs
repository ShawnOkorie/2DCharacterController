using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class S_PlatformController : S_RaycastController
{
    [SerializeField] private LayerMask passengerMask;

    private List<Passengermovement> passengermovement;

    private Dictionary<Transform, S_CharacterController2D> passengerDictionary =
        new Dictionary<Transform, S_CharacterController2D>();

    [SerializeField] private float speed;
    [SerializeField] private bool isCyclic;
    [SerializeField] private float waitTime;
    private float nextMoveTime;

    [SerializeField] [Range(0,2)] private float easeAmount;
    
    [SerializeField] private Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;
    private int previousWaypointIndex;
    private float percentDistanceMoved;
    
    protected override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];

        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }
    
    private void Update()
    {
        UpdateRaycastOrigins();
        
        Vector3 velocity = CalculatePlatformMovement();
        
        CalculatePassengerMovement(velocity);
        
        MovePassengers(true);
        transform.Translate(velocity);  
        MovePassengers(false);
    }
    private Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime) return Vector3.zero;
        
        //prevent index out of bounds resets to 0 when index > length
        previousWaypointIndex %= globalWaypoints.Length;
        int nextWaypointIndex = (previousWaypointIndex + 1) % globalWaypoints.Length;
        
        float waypointDistance = Vector3.Distance(globalWaypoints[previousWaypointIndex], globalWaypoints[nextWaypointIndex]);
        percentDistanceMoved += Time.deltaTime * speed / waypointDistance;
        percentDistanceMoved = Mathf.Clamp01(percentDistanceMoved);

        float easedPercentDistanceMoved = EasePlatformMovement(percentDistanceMoved);
            
        Vector3 newPos = Vector3.Lerp(globalWaypoints[previousWaypointIndex], globalWaypoints[nextWaypointIndex], easedPercentDistanceMoved);

        if (percentDistanceMoved >= 1)
        {
            percentDistanceMoved = 0;
            previousWaypointIndex ++;

            if (!isCyclic)
            {
                if (previousWaypointIndex >= globalWaypoints.Length - 1)
                {
                    previousWaypointIndex = 0;
                    Array.Reverse(globalWaypoints);
                }
            }
            
            nextMoveTime = Time.time + waitTime;
        }
        
        return newPos - transform.position;
    }

    float EasePlatformMovement(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
    }

    private void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassangers = new HashSet<Transform>();
        passengermovement = new List<Passengermovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        
        //TODO: Fix diagonal downward movement
        
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
                    
                        passengermovement.Add(new Passengermovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }
        
        //horizontally moving platform pushing the character
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
                        float pushY = -skinWidth; // small downward movement so character checks collisions below it 
                    
                        passengermovement.Add(new Passengermovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }
        
        //character on top of horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 3;

            for (int i = 0; i < verticalRayCount; i++)
            {
                // same as if(directionY == -1) rayOrigin = raycastorigins.bottomLeft else rayOrigin = raycastorigins.topLeft
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.blue);

                if (hit)
                {
                    //prevent the same passenger from having velocity added for each raycast hit
                    if (!movedPassangers.Contains(hit.transform))
                    {
                        movedPassangers.Add(hit.transform);
                        
                        float pushX = velocity.x;
                        float pushY = velocity.y;
                    
                        passengermovement.Add(new Passengermovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    private void MovePassengers(bool moveBeforePlatform)
    {
        foreach (Passengermovement passenger in passengermovement)
        {
            // saving passenger component in dictionary for performance, only 1 call the first time it gets moved
            if(!passengerDictionary.ContainsKey(passenger.transform)) 
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<S_CharacterController2D>());
            
            if (passenger.moveBeforePlatform == moveBeforePlatform) 
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.isOnPlatform);
        }
    }
    
    struct Passengermovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool isOnPlatform;
        public bool moveBeforePlatform;

        public Passengermovement(Transform transform, Vector3 velocity, bool isOnPlatform, bool moveBeforePlatform)
        {
            this.transform = transform;
            this.velocity = velocity;
            this.isOnPlatform = isOnPlatform;
            this.moveBeforePlatform = moveBeforePlatform;
        }

    }
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.green;
            float size = 0.3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position; 
                Gizmos.DrawLine(globalWaypointPos - new Vector3(0.5f, 0.5f) * size, globalWaypointPos + new Vector3(0.5f, 0.5f) * size);
                Gizmos.DrawLine(globalWaypointPos - new Vector3(-0.5f, 0.5f) * size, globalWaypointPos + new Vector3(-0.5f, 0.5f) * size);
                
                GUI.color = Color.green;
                UnityEditor.Handles.Label(globalWaypointPos + Vector3.right * 0.15f, i.ToString());
            }
            
            
        }
    }
    #endif
}