using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(S_CharacterController2D))]
public class S_PlayerInputManager : MonoBehaviour
{
    //References
    private S_CharacterController2D controller2D;
    
    //Variables
    private Vector2 input;
    
    private Vector3 velocity;
    private float velocityXSmoothing;
    public float movespeed = 5;

    [SerializeField] private float jumpHeight = 4;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float accelTimeGround = .1f;
    [SerializeField] private float accelTimeAir = .2f;

    private float gravity;
    private float jumpVelocity;
    private bool jumpTrigger;

    void Start()
    {
        controller2D = GetComponent<S_CharacterController2D>();
        
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    
    void FixedUpdate()
    {
        if (jumpTrigger) jumpTrigger = false;
        else ResetGravity();
        
        float targetvelocityX = input.x * movespeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetvelocityX, ref velocityXSmoothing, (controller2D.collisions.below) ? accelTimeGround : accelTimeAir);
        velocity.y += gravity * Time.fixedDeltaTime;
       
        controller2D.Move(velocity * Time.deltaTime);
    }

    void ResetGravity()
    {
        if (controller2D.collisions.above || controller2D.collisions.below) velocity.y = 0;
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        input.x = context.ReadValue<float>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (controller2D.collisions.below)
        {
            jumpTrigger = true;
            velocity.y = jumpVelocity;
        }
    }
}
