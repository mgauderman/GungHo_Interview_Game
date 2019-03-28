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
    private bool jump;
    private bool midJump;
    [SerializeField]
    private float jumpForce;
    private bool airControl;
    private bool facingLeft;

    ////UI stuff
    //[SerializeField]
    //Stat otherHealth;

    //// HACK MUSIC LA STUFF
    //Camera cam;
    //public Interactable focus;


    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jump = false;
        airControl = true;
        facingLeft = true;

        //otherHealth.Initialize("Healthy");

        //// HACKMUSICLA STUFF
        //cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        //otherHealth.CurrentValue -= 0.1f;

        HandleMovement(horizontal);

        HandleAnimation(horizontal);

        //HandleUI();


        // HACK MUSIC STUFF
        //if( Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    // If the ray hits
        //    if (Physics.Raycast(ray, out hit, 100))
        //    {
        //        Interactable interactable = hit.collider.GetComponent<Interactable>();
        //        if( interactable != null )
        //        {
        //            SetFocus(interactable);
        //        }
        //    }
        //}


    }

    void Update()
    {
        HandleInput();
    }

    private void HandleMovement(float horizontal)
    {
        if ((isGrounded || airControl))
        {
            rigidBody.velocity = new Vector2(speedMultiplier * horizontal, rigidBody.velocity.y);
        }

        if (isGrounded && jump)
        {
            animator.SetFloat("walk", 0);
            isGrounded = false;
            rigidBody.AddForce(new Vector2(0, jumpForce));
        }

        ResetValues();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void HandleAnimation(float horizontal)
    {
        animator.SetBool("jump", !isGrounded);
        animator.SetFloat("walk", Mathf.Abs(horizontal));

        if (facingLeft && horizontal > 0.01f)
        {
            facingLeft = false;
            Vector3 localScale = GetComponentInParent<Transform>().localScale; // flip player
            GetComponent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
        }
        else if (!facingLeft && horizontal < -0.01f)
        {
            facingLeft = true;
            Vector3 localScale = GetComponentInParent<Transform>().localScale;
            GetComponentInParent<Transform>().localScale = new Vector3(localScale.x, localScale.y, localScale.z * -1.0f);
        }

        if (midJump)
        {
            if (!isGrounded && animator.speed > 0)
            {
                animator.speed = 0;
            }
            else if (isGrounded)
            {
                animator.speed = 1;
                midJump = false;
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
        jump = false; // so player doesn't keep jumping
    }

    private void OnMidJump()
    {
        midJump = true;
    }

    // HACK MUSIC STUFF
    //void SetFocus(Interactable newFocus)
    //{
    //    focus = newFocus;
    //}

}
