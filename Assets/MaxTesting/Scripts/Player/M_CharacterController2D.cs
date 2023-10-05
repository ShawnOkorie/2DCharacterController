using UnityEngine;

// This script is a basic 2D character controller that allows
// the player to run and jump. It uses Unity's new input system,
// which needs to be set up accordingly for directional movement
// and jumping buttons.

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class M_CharacterController2D : MonoBehaviour
{

    [Header("Movement Params")]
    public float runSpeed = 6.0f;

    [SerializeField] private Animator _animation;

    // components attached to player
    private BoxCollider2D coll;
    private Rigidbody2D rb;

    // other

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (M_DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 moveDirection = M_InputManager.GetInstance().GetMoveDirection();
        rb.velocity = new Vector2(moveDirection.x * runSpeed, moveDirection.y * runSpeed);
        if (moveDirection == Vector2.zero)
        {
            _animation.SetBool("Moving", false);
        }
        else
        {
            _animation.SetBool("Moving", true);
            _animation.SetFloat("Horizontal", moveDirection.x);
            _animation.SetFloat("Vertical", moveDirection.y);
        }

        

    }
    

}