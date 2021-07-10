using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;

    public float jumpForce = 3;
    public float jumpHoldForce = 1;
    public float jumpHoldDuration = 0.15f;
    public bool isJumping;
    private float jumpTime;
    public bool isOnGround;

    public float coyoteDuration = 0.1f;
    private float coyoteTime;

    public float leftFootOffset = -0.3f;
    public float rightFootOffset = 0.3f;
    public float groundOffset = 0.5f;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Ladder")]
    public float climbSpeed = 3;              
    public LayerMask ladderMask;               
    public float vertical;                      
    public bool climbing;                       
    public float checkRadius = 0.3f;
    private Transform ladder;

    [HideInInspector]
    public int direction = 1;
    public float horizontal;
    public bool jumpHeld;
    public bool jumpStarted;
    public bool canMove = true;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        AirMovement();
        ClimbLadder();
    }

    void PhysicsCheck()
    {
        isOnGround = false;

        RaycastHit2D leftFoot = Raycast(new Vector2(leftFootOffset, -groundOffset), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightFoot = Raycast(new Vector2(rightFootOffset, -groundOffset), Vector2.down, groundDistance, groundLayer);

        if (leftFoot || rightFoot)
        {
            if (!isJumping)
                isOnGround = true;
        }

        animator.SetBool("OnGround", isOnGround);
    }

    bool TouchingLadder()
    {
       
        return col.IsTouchingLayers(ladderMask);
    }

    void ClimbLadder()
    {


       
        bool up = Physics2D.OverlapCircle(transform.position, checkRadius, ladderMask);
        bool down = Physics2D.OverlapCircle(transform.position + new Vector3(0, -1), checkRadius, ladderMask);



        if (vertical != 0 && TouchingLadder()) 
        {
            climbing = true;                                 
            rb.isKinematic = true;         
            canMove = false;                


           
            float xPos = ladder.position.x;


            transform.position = new Vector2(xPos, transform.position.y);


        }

        if (climbing)
        {


            if (!up && vertical >= 0)
            {
                FinishClimb();
                return;
            }

            if (!down && vertical <= 0)
            {
                FinishClimb();
                return;
            }


            float y = vertical * climbSpeed;           
            rb.velocity = new Vector2(0, y);            





        }


    }

    void FinishClimb()
    {

      
        climbing = false;
        rb.isKinematic = false;
        canMove = true;
    }

    void GroundMovement()
    {
        if (!canMove)
            return;

        float xVelocity = speed * horizontal;

        rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        if (direction * xVelocity < 0)
        {
            Flip();
        }

        if (isOnGround)
        {
            coyoteTime = Time.time + coyoteDuration;
        }

        animator.SetFloat("Speed", Mathf.Abs(xVelocity));
    }

    void AirMovement()
    {
        if (jumpStarted && (isOnGround || coyoteTime > Time.time))
        {

            isOnGround = false;

            isJumping = true;
            jumpStarted = false;

            rb.velocity = Vector2.zero;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpTime = Time.time + jumpHoldDuration;

            coyoteTime = Time.time;
        }
        if (isJumping)
        {
            if (jumpHeld)
            {
                rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Impulse);
            }

            if (jumpTime <= Time.time)
            {
                isJumping = false;
            }
        }

        jumpStarted = false;

    }

    void Flip()
    {
        direction *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        horizontal = ctx.ReadValue<float>();
    }

    public void Climb(InputAction.CallbackContext ctx)
    {
        vertical = ctx.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            jumpStarted = true;
        }

        jumpHeld = ctx.performed;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask layerMask)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layerMask);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDirection * length, color);

        return hit;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            ladder = other.transform;
        }
    }
}