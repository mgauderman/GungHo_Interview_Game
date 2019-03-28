using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    [SerializeField]
    private float speedMultiplier;


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
    private bool airControl;
    private bool facingLeft;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jump = false;
        airControl = true; // allows player to change directions in air
        facingLeft = true;
        inAirFromJump = false;
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);

        HandleAnimation(horizontal);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleMovement(float horizontal)
    {
        if (inAirFromJump && isGrounded)
        {
            inAirFromJump = false;
        }
        
        //if (isPunching)
        //{
        //    rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        //}
   
        if ((isGrounded && !isPunching) || (!isGrounded && airControl)) // update player's left/right velocity
        {
            rigidBody.velocity = new Vector2(speedMultiplier * horizontal, rigidBody.velocity.y);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            punch = true;
            isPunching = true;
        }
    }

    private void HandleAnimation(float horizontal)
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
                Vector3 localScale = GetComponentInParent<Transform>().localScale;
                GetComponent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
            }
            else if (!facingLeft && horizontal < -0.01f) // flip player from right to left
            {
                facingLeft = true;
                Vector3 localScale = GetComponentInParent<Transform>().localScale;
                GetComponentInParent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
            }
        }
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
}
