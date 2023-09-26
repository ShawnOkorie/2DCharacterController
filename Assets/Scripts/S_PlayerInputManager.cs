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
    public float movespeed = 5;

    [SerializeField] private float jumpHeight = 4;
    [SerializeField] private float timeToJumpApex = .4f;

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
        
        velocity.x = input.x * movespeed;
        velocity.y += gravity * Time.fixedDeltaTime;
       
        controller2D.Move(velocity * Time.deltaTime);
    }

    void ResetGravity()
    {
        if (controller2D.collisionInfo.above || controller2D.collisionInfo.below) velocity.y = 0;
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        input.x = context.ReadValue<float>();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (controller2D.collisionInfo.below)
        {
            jumpTrigger = true;
            velocity.y = jumpVelocity;
        }
    }
}
