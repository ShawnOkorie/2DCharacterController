using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(S_CharacterController2D))]
public class S_PlayerInputManager : MonoBehaviour
{
    //References
    private S_CharacterController2D controller2D;
    private S_CharacterController2D.CollisionInfo Collisions => controller2D.collisions;
    
    //Variables
    private Vector2 input;
    
    private Vector3 velocity;
    private float velocityXSmoothing;
    public float movespeed = 5;
    private float gravity;
    
    //Jump
    [SerializeField] private float jumpHeight = 4;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float accelTimeGround = .1f;
    [SerializeField] private float accelTimeAir = .2f;
    private float jumpVelocity;
    private bool canJump;
    
    //Walljump
    private bool wallSliding;
    private int wallDirectionX;
    [SerializeField] private float wallSlideSpeedMax = 3;
    [SerializeField] private float wallStickTime = .25f;
    private float timeToWallUnstick;
    [SerializeField] private Vector2 wallJumpClimb;
    [SerializeField] private Vector2 wallJumpOff;
    [SerializeField] private Vector2 wallJumpLeap;

    void Start()
    {
        controller2D = GetComponent<S_CharacterController2D>();
        
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    
    void FixedUpdate()
    {
        wallDirectionX  = (Collisions.left) ? -1 : 1;

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
        else wallSliding = false;
        

        if (canJump) canJump = false;
        else ResetGravity();
        
        float targetvelocityX = input.x * movespeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetvelocityX, ref velocityXSmoothing, (Collisions.below) ? accelTimeGround : accelTimeAir);
        
        velocity.y += gravity * Time.fixedDeltaTime;
        
        controller2D.Move(velocity * Time.deltaTime);
    }

    void ResetGravity()
    {
        if (Collisions.above || Collisions.below) velocity.y = 0;
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        input.x = context.ReadValue<float>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (wallSliding)
        {
            canJump = true;
            //Climbing Wall
            if (wallDirectionX == input.x)
            {
                print("Climb");
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y; 
            }
            //Jumping Off Wall
            else if (input.x == 0)
            {
                print("Off");
                velocity.x = -wallDirectionX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            //Jumping away from the Wall
            else if (wallDirectionX != input.x)
            {
                print("Leap");
                velocity.x = -wallDirectionX * wallJumpLeap.x;
                velocity.y = wallJumpLeap.y;
            }
        }
        
        if (Collisions.below)
        {
            canJump = true;
            velocity.y = jumpVelocity;
        }
    }
}
