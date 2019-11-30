using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageController;

public class Player : MonoBehaviour {

    private const int DEFAULT_JUMPS = 3;
    private const int DIED_FORCE = 2;


    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public LayerMask whatIsGround;
    public float jumpTime;
    public BoxCollider2D upperCollider;

    public bool isAlive = true;

    public Animator animator;
    [HideInInspector]
    public int jumps = 3;
    [HideInInspector]
    public bool isJumping;
    [HideInInspector]
    public bool isGrounded;
    private Rigidbody2D rb;
    private Vector2 velVector = new Vector2();
    private bool isSliding;
    private float counter;
    private bool isDoubleJumpPlaying;
    private float scoreTimer = 0.9f;
    private float jumpYScreenPos = -2.9f;
    private float jumpForce = 10f;
    private float downForce = 100f;
    private float jumpForceTouch = 10f;
    private float downForceTouch = 100f;

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
        isGrounded = Physics2D.OverlapCircle(feetCollider.position, 0.1f, whatIsGround);
        if (isAlive && StageController.instance.currentState == GAME_STATE.GAMEPLAY) {
            // TODO IOS!!
            if (Application.platform == RuntimePlatform.Android) {
                TouchInput();
            } else {
                KeyBoardInput();
            }
        }
        scoreTimer -= Time.deltaTime;
        animator.SetBool("isGrounded", isGrounded);
    }


    void OnTriggerEnter2D(Collider2D collision) {
        if (isAlive) {
            if (collision.gameObject.tag == "Obstacle") {
                isAlive = false;
                //collision.gameObject.SendMessage("ApplyDamage", 10);
                //animator.SetBool("isAlive", true);
                //rb.velocity = Vector2.up * DIED_FORCE;
                animator.SetTrigger("diedTrigger");
                GameEventManager.instance.OnPlayerDied();
            } else if (collision.gameObject.tag == "ScoreTrigger" && scoreTimer < 0) {
                // Just use the timer to avoid both player colliders getting a point
                scoreTimer = 0.9f;
                StageController.instance.UpdateScore();
            }
        }
    }

    private void TouchInput() {
        Vector3 pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(pointerPos.y);
        if ((jumps > 0 || isGrounded) && Input.GetMouseButtonDown(0) && pointerPos.y > jumpYScreenPos) {
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
            rb.velocity = Vector2.up * jumpForceTouch;
            counter = jumpTime;
            isJumping = true;

            //AudioManager.instance.Play("jump2");
            AudioManager.instance.PlayJump();
        }

        if (Input.GetMouseButton(0) && pointerPos.y > jumpYScreenPos && isJumping) {
            if (counter > 0f) {
                rb.velocity = Vector2.up * jumpForceTouch;
                counter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        } else if (Input.GetMouseButton(0) && pointerPos.y <= jumpYScreenPos && !isGrounded) {
            rb.velocity += Vector2.down * downForceTouch * Time.deltaTime;
            if (rb.velocity.y <= -30) {
                velVector.y = -30;
                rb.velocity = velVector;
            }

        } else if (Input.GetMouseButton(0) && pointerPos.y <= jumpYScreenPos && isGrounded && !isSliding) {
            //AudioManager.instance.PlaySlide();
            isSliding = true;
            animator.SetBool("isSliding", true);
            upperCollider.enabled = false;
        }

        if (Input.GetMouseButtonDown(0) && pointerPos.y <= jumpYScreenPos) {
            AudioManager.instance.PlayDownForce();
        }

        if (Input.GetMouseButtonUp(0)) {
            isJumping = false;
            ResetSliding();
        }

    }

    private void KeyBoardInput() {
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

            //AudioManager.instance.Play("jump2");
            AudioManager.instance.PlayJump();
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
            //AudioManager.instance.PlaySlide();
            isSliding = true;
            animator.SetBool("isSliding", true);
            upperCollider.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            AudioManager.instance.PlayDownForce();
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            isJumping = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            ResetSliding();
        }
    }

    private void OnReset() {
        ResetSliding();
        isAlive = true;
        scoreTimer = 0.9f;
        animator.SetTrigger("resetTrigger");
    }

    private void ResetSliding() {
        isSliding = false;
        upperCollider.enabled = true;
        animator.SetBool("isSliding", false);
    }

    //private void FixedUpdate() {}
}
