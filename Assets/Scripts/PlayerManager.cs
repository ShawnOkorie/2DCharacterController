using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent (typeof(S_CharacterController2D))]
public class PlayerManager : MonoBehaviour
{
    //References
    private S_CharacterController2D controller2D;
    private S_CharacterController2D.CollisionInfo Collisions => controller2D.collisions;

    //Variables
    private Vector2 input;
    private Vector3 velocity;
    private float velocityXSmoothing;
    [SerializeField] private float movespeed = 5;
    private float gravity;
    [SerializeField] private float gravityMultiplier = 1;
    
    //Jump
    [SerializeField] private float maxJumpHeight = 5;
    [SerializeField] private float minJumpHeight = 0.25f;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float accelTimeGround = .1f;
    [SerializeField] private float accelTimeAir = .2f;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private bool canJump;
    
    //Walljump
    private bool wallSliding;
    private int wallDirectionX;
    [SerializeField] private float wallSlideSpeedMax = 3;
    [SerializeField] private float wallStickTime = .25f;
    private float timeToWallUnstick;
    [SerializeField] private Vector2 wallJumpClimbVector;
    [SerializeField] private Vector2 wallJumpOffVector;
    [SerializeField] private Vector2 wallJumpLeapVector;
    
    //Enablers
    [SerializeField] private bool wallStickEnabled = true;
    [SerializeField] private bool wallJumpEnabled = true;
    [SerializeField] private bool wallClimbEnabled  = true;
    [SerializeField] private bool variableJumpEnabled = true;
    [SerializeField] private bool doubleJumpEnabled  = true;

    void Start()
    {
        controller2D = GetComponent<S_CharacterController2D>();

        CalculateJumpValues();
    }
    void CalculateJumpValues()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex,2) * gravityMultiplier;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        
        print("Gravity: " + gravity + " Max Jump Velocity: " + maxJumpVelocity + " Min Jump Velocity: " + minJumpVelocity);
    }
    void FixedUpdate()
    {
        wallDirectionX  = (Collisions.left) ? -1 : 1;

        wallSliding = false;
        
        if (wallStickEnabled)
        {
            if ((Collisions.left || Collisions.right) && !Collisions.below && velocity.y < 0)
            {
                wallSliding = true;
                if (velocity.y < -wallSlideSpeedMax) velocity.y = -wallSlideSpeedMax;
            
                if (timeToWallUnstick > 0) 
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (input.x != wallDirectionX && input.x != 0) 
                        timeToWallUnstick -= Time.deltaTime;
                
                    else timeToWallUnstick = wallStickTime;
                }
            
                else timeToWallUnstick = wallStickTime;
            } 
        }

        float targetvelocityX = input.x * movespeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetvelocityX, ref velocityXSmoothing, (Collisions.below) ? accelTimeGround : accelTimeAir);
        
        velocity.y += gravity * Time.fixedDeltaTime;
        
        controller2D.Move(velocity * Time.deltaTime, input);
        
        if (canJump) canJump = false;
        else ResetGravity();
    }
    void ResetGravity()
    {
        if (Collisions.above || Collisions.below) velocity.y = 0;
    }
    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        input.x = context.ReadValue<float>();
    }
    public void MoveVertical(InputAction.CallbackContext context)
    {
        input.y = context.ReadValue<float>();
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)      // TODO: replace with action instead of key 
        {
            if (wallSliding)
            {
                canJump = true;
                //TODO: make wall juping feel better 
                //Climbing Wall
                if (wallDirectionX == input.x)
                {
                    if (wallClimbEnabled)
                    {
                        print("climb");
                        velocity.x = -wallDirectionX * wallJumpClimbVector.x;
                        velocity.y = wallJumpClimbVector.y; 
                    }
                }
                //Jumping away from the Wall
                else if (input.x == 0)
                {
                    if (wallJumpEnabled)
                    {
                        print("away");
                        velocity.x = -wallDirectionX * wallJumpLeapVector.x;
                        velocity.y = wallJumpLeapVector.y;
                    }
                }
                //Jumping Off Wall
                else if (wallDirectionX != input.x)          
                {
                    print("off");
                    velocity.x = -wallDirectionX * wallJumpOffVector.x;
                    velocity.y = wallJumpOffVector.y;
                }
            }
        
            if (Collisions.below)
            {
                canJump = true;
                velocity.y = maxJumpVelocity;
            }
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
    }

    #region Settings
        #region Values
            #region Setters
                public void SetMoveSpeed(float speed)
                {
                    movespeed = speed;
                }
                public void SetGravityMultiplier(float multiplier)
                {
                    gravityMultiplier = multiplier;
                    CalculateJumpValues();
                }
                public void SetMaxJumpHeight(float height)
                {
                    maxJumpHeight = height;
                    CalculateJumpValues();
                }
                public void SetMinJumpHeight(float height)
                {
                    minJumpHeight = height;
                    CalculateJumpValues();
                }
                public void SetTimeToJumpApex(float time)
                {
                    timeToJumpApex = time;
                    CalculateJumpValues();
                }
                public void SetAccelTimeGround(float time)
                {
                    accelTimeGround = time;
                }
                public void SetAccelTimeAir(float time)
                {
                    accelTimeAir = time;
                }
                public void SetWallSlideSpeedMax(float speed)
                {
                    wallSlideSpeedMax = speed;
                }
                public void SetWallStickTime(float time)
                {
                    wallStickTime = time;
                }
                public void SetJumpClimbVector(Vector2 vector)
                {
                    wallJumpClimbVector = vector;
                }
                public void SetJumpOffVector(Vector2 vector)
                {
                    wallJumpOffVector = vector;
                }
                public void SetJumpLeapVector(Vector2 vector)
                {
                    wallJumpLeapVector = vector;
                }
            #endregion
            #region Getters
                public float GetMoveSpeed()
                {
                   return movespeed;
                }
                public float GetGravityMultiplier()
                {
                    return gravityMultiplier;
                }
                public float GetMaxJumpHeight()
                {
                    return maxJumpHeight;
                }
                public float GetMinJumpHeight()
                {
                    return minJumpHeight;
                }
                public float GetTimeToJumpApex()
                {
                    return timeToJumpApex;
                }
                public float GetAccelTimeGround()
                {
                    return accelTimeGround;
                }
                public float GetAccelTimeAir()
                {
                   return accelTimeAir;
                }
                public float GetWallSlideSpeedMax()
                {
                   return wallSlideSpeedMax;
                }
                public float GetWallStickTime()
                {
                   return wallStickTime;
                }
                public Vector2 GetJumpClimbVector()
                {
                   return wallJumpClimbVector;
                }
                public Vector2 SetJumpOffVector()
                {
                    return wallJumpOffVector;
                }
                public Vector2 SetJumpLeapVector()
                {
                    return wallJumpLeapVector;
                }
            #endregion
        #endregion
        
        #region Enablers
            #region Setters
                public void EnableWallStick(bool enable)
                {
                    wallStickEnabled = enable;
                }
                public void EnableWallJump(bool enable)
                {
                    wallJumpEnabled = enable;
                }
                public void EnableWallClimb(bool enable)
                {
                    wallClimbEnabled = enable;
                }
                public void EnableVariableJump(bool enable)
                {
                    variableJumpEnabled = enable;
                }
                public void EnableDoubleJump(bool enable)
                {
                    doubleJumpEnabled = enable;
                }
            #endregion
            #region Getters
                public bool IsWallStickEnabled()
                {
                    return wallStickEnabled;
                }
                public bool IsWallJumpEnabled()
                {
                    return wallJumpEnabled;
                }
                public bool IsWallClimbEnabled()
                {
                    return wallClimbEnabled;
                }
                public bool IsVariableJumpEnabled()
                {
                    return variableJumpEnabled;
                }
                public bool IsDoubleJumpEnabled()
                {
                    return doubleJumpEnabled;
                }
            #endregion
        #endregion
    #endregion
}
