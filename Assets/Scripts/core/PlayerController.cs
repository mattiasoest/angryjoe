using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private const int DEFAULT_JUMPS = 3;
    private const int DIED_FORCE = 2;

    public Animator animator;
    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public float jumpForce;
    public float downForce;
    public LayerMask whatIsGround;
    public float jumpTime;
    public BoxCollider2D upperCollider;

    private Rigidbody2D rb;
    private Vector2 velVector = new Vector2();
    private bool isGrounded;
    private bool isSliding;
    private float counter;
    private bool isJumping;
    private int jumps = 3;
    private bool isDoubleJumpPlaying;

    public void ResetJumpTrigger() {
        isDoubleJumpPlaying = false;
        animator.SetBool("shouldPlayDoubleJump", isDoubleJumpPlaying);
        animator.ResetTrigger("doubleJump");
    }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        GameEventManager.instance.onReset += OnReset;
    }

    void Update() {
        if (StageController.instance.isPlayerAlive) {

            isGrounded = Physics2D.OverlapCircle(feetCollider.position, 0.1f, whatIsGround);

            if ((jumps > 0 || isGrounded) && Input.GetKeyDown(KeyCode.Space)) {

                ResetSliding();
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
                if (counter > 0f) {
                    rb.velocity = Vector2.up * jumpForce;
                    counter -= Time.deltaTime;
                } else {
                    isJumping = false;
                }
            } else if (Input.GetKey(KeyCode.DownArrow) && !isGrounded) {
                rb.velocity += Vector2.down * downForce * Time.deltaTime;
                if (rb.velocity.y <= -30) {
                    velVector.y = -30;
                    rb.velocity = velVector;
                }

            } else if (Input.GetKey(KeyCode.DownArrow) && isGrounded && !isSliding) {
                isSliding = true;
                animator.SetBool("isSliding", true);
                upperCollider.enabled = false;
            }

            if (Input.GetKeyUp(KeyCode.Space)) {
                isJumping = false;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow)) {
                ResetSliding();
            }

            animator.SetBool("isGrounded", isGrounded);
        }
    }


    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Obstacle" && StageController.instance.isPlayerAlive) {
            //collision.gameObject.SendMessage("ApplyDamage", 10);
            //animator.SetBool("isAlive", true);
            //rb.velocity = Vector2.up * DIED_FORCE;
            animator.SetTrigger("diedTrigger");
            GameEventManager.instance.OnPlayerDied();
        }
    }

    private void OnReset() {
        animator.SetTrigger("resetTrigger");
    }

    private void ResetSliding() {
        isSliding = false;
        upperCollider.enabled = true;
        animator.SetBool("isSliding", false);
    }

    //private void FixedUpdate() {}
}
