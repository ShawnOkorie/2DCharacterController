using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class S_PlatformController : S_RaycastController
{
    [SerializeField] private LayerMask passengerMask;
    
    [SerializeField] private Vector3 platformMovement;

    private List<Passengermovement> passengermovement;

    private Dictionary<Transform, S_CharacterController2D> passengerDictionary =
        new Dictionary<Transform, S_CharacterController2D>();

    protected override void Start()
    {
        base.Start();
    }
    
    private void Update()
    {
        UpdateRaycastOrigins();
        
        Vector3 velocity = platformMovement * Time.deltaTime;
        
        CalculatePassengerMovement(velocity);
        
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    private void MovePassengers(bool beforeMovePlatform)
    {
        foreach (Passengermovement passenger in passengermovement)
        {
            // saving passenger component in dictionary for performance, only 1 call the first time it gets moved
            if(!passengerDictionary.ContainsKey(passenger.transform)) 
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<S_CharacterController2D>());
            
            if (passenger.moveBeforePlatform == beforeMovePlatform) 
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.isOnPlatform);
        }
        
    }
    
    private void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassangers = new HashSet<Transform>();
        passengermovement = new List<Passengermovement>();

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
                    
                        passengermovement.Add(new Passengermovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
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
}