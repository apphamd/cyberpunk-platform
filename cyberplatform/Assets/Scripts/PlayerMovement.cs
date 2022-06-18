using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
  

    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Variables")]
    private float moveInput;
    [SerializeField] private float maxSpd = 12f;
    [SerializeField] private float accel = 70f;
    [SerializeField] private float groundDeccel = 7f;
    private bool changingDirection => (rb.velocity.x > 0f && moveInput < 0f) || (rb.velocity.x <0f && moveInput > 0f);


    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float airDeccel = 2.5f;

    [SerializeField] public float fallMultiplier = 8f;
    [SerializeField] public float lowJumpMultiplier = 5f;

    [SerializeField] private int extraJumps = 1;
    private int jumpCount;
    private bool canJump => Input.GetButtonDown("Jump") && (hangCounter > 0f || jumpCount > 0);

    public float hangTime = .2f;
    private float hangCounter;


    [Header("Collision Variables")]
    [SerializeField] private float groundRaycastLength;
    private bool onGround;
    
    private enum MovementState {idle, running, jumping, falling} //create different movement states
    private MovementState state = MovementState.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = GetInput().x; //Freq updates input for movement
        if (canJump) Jump();
        RunAnim();

    }

    private void FixedUpdate()
    {

        CheckCollisions();
        MoveCharacter();
        ApplyGroundLinearDrag();
        if (onGround)
        {
            jumpCount = extraJumps;
            hangCounter = hangTime;
            ApplyGroundLinearDrag();

        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
            hangCounter -= Time.deltaTime;
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //checks if the player wants to move left or right
    private void MoveCharacter() 
    {
        rb.AddForce(new Vector2(moveInput, 0f) * accel);

        if (Mathf.Abs(rb.velocity.x) > maxSpd)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpd, rb.velocity.y);
        }
       
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(moveInput) < 0.4f || changingDirection)
        {
            rb.drag = groundDeccel;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = airDeccel;

    }

    //handles run animation
    private void RunAnim()
    {
        if (moveInput < 0f && onGround) //anim for move left
        {
            sprite.flipX = true;
            anim.SetBool("running", true);
        }
        else if (moveInput > 0f && onGround) //move right
        {
            sprite.flipX = false;
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    private void CheckCollisions()
    {
        onGround = Physics2D.Raycast(transform.position * groundRaycastLength, Vector2.down, groundRaycastLength, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void Jump()
    {
        if (!onGround)
            jumpCount--;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

}
