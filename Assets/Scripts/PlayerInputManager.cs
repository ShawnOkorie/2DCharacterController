using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController2D))]
public class PlayerInputManager : MonoBehaviour
{
    //References
    private CharacterController2D controller2D;
    
    //Variables
    private Vector2 input;
    
    private Vector3 velocity;
    private float gravity = -20;
    
    private float movespeed = 10;

    void Start()
    {
        controller2D = GetComponent<CharacterController2D>();
    }
    
    void Update()
    {
        velocity.x = input.x * movespeed;
        
        velocity.y += gravity * Time.deltaTime;
        controller2D.Move(velocity * Time.deltaTime);
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           input.x = context.ReadValue<float>();
        }

        if (context.canceled)
        {
            input.x = context.ReadValue<float>();
        }    
    }
}
