using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const int DEFAULT_JUMPS = 3;

    public Animator animator;
    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public float jumpForce;
    public float downForce;
    public LayerMask whatIsGround;
    public float jumpTime;

    private Rigidbody2D rb;
    private Vector2 velVector = new Vector2();
    private bool isGrounded;
    private float counter;
    private bool isJumping;
    private int jumps = 3;
    private bool isDoubleJumpPlaying;
    //private bool applyDownbForce;
 
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetJumpTrigger() {
        isDoubleJumpPlaying = false;
        animator.SetBool("shouldPlayDoubleJump", isDoubleJumpPlaying);
        animator.ResetTrigger("doubleJump");
    }

    private void FixedUpdate() {
    }

    private void Update() {
        isGrounded = Physics2D.OverlapCircle(feetCollider.position, 0.1f, whatIsGround);

        if ((jumps > 0 || isGrounded) && Input.GetKeyDown(KeyCode.Space)) {
            if (isGrounded) {
                jumps = DEFAULT_JUMPS;
            }
            if (jumps < DEFAULT_JUMPS) {
                isDoubleJumpPlaying = true;
                animator.SetBool("shouldPlayDoubleJump", isDoubleJumpPlaying);
                animator.SetTrigger("doubleJump");
                
            }
            jumps--;
            rb.velocity = Vector2.up * jumpForce;
            counter = jumpTime;
            isJumping = true;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping) {
            if (counter  > 0f) {
                rb.velocity = Vector2.up * jumpForce;
                counter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        } else if (Input.GetKey(KeyCode.DownArrow) && !isGrounded) {
            rb.velocity += Vector2.down * downForce * Time.deltaTime;
            //applyDownbForce = true;
            if (rb.velocity.y <= -18f) {
                velVector.y = -18f;
                rb.velocity = velVector;
            }

        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            isJumping = false;
        }

        animator.SetBool("isGrounded", isGrounded);
    }
}
