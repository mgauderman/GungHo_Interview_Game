using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private GrappleSystem grappleSystem;

    // camera
    [SerializeField]
    private Camera mainCamera;

    // is grounded stuff
    private bool isGrounded;
    [SerializeField]
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private LayerMask whatIsGround;

    // jumping
    private bool jump; // only used to trigger the jump
    private bool inAirFromJump; // true if player is in air because they jumped (instead of falling)
    [SerializeField]
    private float jumpForce;

    // punching
    private bool punch; // only used to trigger the punch
    private bool isPunching;

    // left/right control
    private float horizontal; // horizontal axis
    private bool airControl;
    private bool facingLeft;
    [SerializeField]
    private float speedMultiplier;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        grappleSystem = GetComponent<GrappleSystem>();
        jump = false;
        airControl = true; // allows player to change directions in air
        facingLeft = true;
        inAirFromJump = false;
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        HandleMovement(horizontal);
    }
    
    void Update()
    {
        HandleInput();
        HandleAnimation();
        HandleCamera();
    }

    private void HandleMovement(float horizontal)
    {
        if (inAirFromJump && isGrounded)
        {
            inAirFromJump = false;
        }
   
        if (((isGrounded && !isPunching) || (!isGrounded && airControl)) && !grappleSystem.IsSwinging()) // update player's left/right velocity
        {
            float maxXVelocity = Mathf.Max(Mathf.Abs(rigidBody.velocity.x), speedMultiplier); // in case player currently has a higher x velocity than speed multiplier
            rigidBody.velocity = new Vector2(Mathf.Clamp(rigidBody.velocity.x + speedMultiplier * horizontal, -maxXVelocity, maxXVelocity), rigidBody.velocity.y);
        }

        if (isGrounded && jump && !isPunching) // jump the player
        {
            isGrounded = false;
            rigidBody.AddForce(new Vector2(0, jumpForce));
            inAirFromJump = true;
        }

        ResetValues();
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPunching)
            {
                punch = true;
                isPunching = true;
            }
            else if (grappleSystem.IsSwinging())
            {
                grappleSystem.RetractGrapple();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            grappleSystem.StopGrapple();
        }
    }

    private void HandleAnimation()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("inAirFromJump", inAirFromJump);
        animator.SetBool("punch", isPunching);
        animator.SetFloat("walk", Mathf.Abs(horizontal));

        if (!isPunching)
        {
            if (facingLeft && horizontal > 0.01f) // flip player from left to right
            {
                facingLeft = false;
                //transform.eulerAngles = new Vector3(0, 90, 0);
                Vector3 localScale = GetComponentInParent<Transform>().localScale;
                GetComponent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
            }
            else if (!facingLeft && horizontal < -0.01f) // flip player from right to left
            {
                facingLeft = true;
                //transform.eulerAngles = new Vector3(0, -90, 0);
                Vector3 localScale = GetComponentInParent<Transform>().localScale;
                GetComponentInParent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
            }
        }
    }

    private void HandleCamera()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);
    }

    private bool IsGrounded()
    {
        if (rigidBody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void ResetValues() 
    {
        // prevent processing input twice
        jump = false;
        punch = false;
    }

    private void OnMidJumpAnim()
    {
        inAirFromJump = false;
    }

    private void onEndPunchAnim()
    {
        isPunching = false;
    }

    private void onMidPunchAnim()
    {
        grappleSystem.DoGrapple();
        animator.speed = 0;
        inAirFromJump = false;
    }

    public void OnEndGrapple()
    {
        animator.speed = 1;
    }

    public bool IsFacingLeft()
    {
        return facingLeft;
    }
}
