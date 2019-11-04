using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Animator animator;
    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public float jumpForce;
    public LayerMask whatIsGround;
    public float jumpTime;

    private Rigidbody2D rb;
    private float moveInput;
    private Vector2 velVector = new Vector2();
    private bool isGrounded;
    private float counter;
    private bool isJumping = false;
    private int jumps = 2;
    private bool isDoubleJumpPlaying;
 
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        //feetCollider = GetComponent<CircleCollider2D>();
        StageController.Instance.Test("hejsan");
        StageController.Instance.Test("whatsappp");
    }

    // Update is called once per frame
    void FixedUpdate() {
        moveInput = Input.GetAxisRaw("Horizontal");
        velVector.x = moveInput * speed;
        velVector.y = rb.velocity.y;
        rb.velocity = velVector;
    }

    private void Update() {
        isGrounded = Physics2D.OverlapCircle(feetCollider.position, 0.1f, whatIsGround);

        if ((jumps > 0 || isGrounded) && Input.GetKeyDown(KeyCode.Space)) {
            if (isGrounded) {
                jumps = 2;
            }
            if (jumps < 2) {
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
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            isJumping = false;
        }

        animator.SetBool("isGrounded", isGrounded);

    }

    public void ResetJumpTrigger() {
        isDoubleJumpPlaying = false;
        animator.SetBool("shouldPlayDoubleJump", isDoubleJumpPlaying);
        animator.ResetTrigger("doubleJump");
    }
}
