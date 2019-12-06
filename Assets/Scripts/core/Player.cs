using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageController;

public class Player : MonoBehaviour {

    private const int DIED_FORCE = 2;

    private readonly Vector2 GLASSES_DEFAULT = new Vector2(0.32f, -0.18f);
    private readonly Vector2 GLASSES_SLIDING = new Vector2(1.79f, -3.35f);

    private int defaultJumps = 2;
    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public LayerMask whatIsGround;
    public float jumpTime;
    public BoxCollider2D upperCollider;
    public GameObject glasses;
    [HideInInspector]
    public bool glassesEquipped;
    public bool isAlive = true;

    public Animator animator;
    public Animator glassesAnimator;
    [HideInInspector]
    public int jumps = 2;
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
    private float downForce = 125f;
    private bool isImmune;

    private float immuneTimer = 2.5f;
    private SpriteRenderer playerRenderer;
    private SpriteRenderer glassRenderer;

    public void ResetJumpTrigger() {
        isDoubleJumpPlaying = false;
        animator.SetBool("shouldPlayDoubleJump", isDoubleJumpPlaying);
        animator.ResetTrigger("doubleJump");
    }

    public void GrantJumpReward() {
        defaultJumps = 3;
        jumps = 3;
        // plus bonus glasses for the lulz
        glasses.SetActive(true);
        glassesEquipped = true;
    }

    void Start() {
        playerRenderer = GetComponent<SpriteRenderer>();
        glassRenderer = glasses.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        GameEventManager.instance.onReset += OnReset;
        GameEventManager.instance.onRevive += OnRevive;
    }

    void Update() {
        if (isImmune) {
            immuneTimer -= Time.deltaTime;
            if (immuneTimer < 0) {
                setImmune(false);
            }
        }

        isGrounded = Physics2D.OverlapCircle(feetCollider.position, 0.1f, whatIsGround);
        if (isAlive && StageController.instance.currentState == GAME_STATE.GAMEPLAY) {
            // TODO IOS!!
            if (Application.platform == RuntimePlatform.Android) {
                TouchInput();
            } else {
                KeyBoardInput();
            }
            scoreTimer -= Time.deltaTime;
        }
        animator.SetBool("isGrounded", isGrounded);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (isAlive) {
            if (collision.gameObject.tag == "Obstacle") {
                // Player continued and is immune for
                if (isImmune) {
                    return;
                }
                AudioManager.instance.PlayHit();
                isAlive = false;
                //collision.gameObject.SendMessage("ApplyDamage", 10);
                //animator.SetBool("isAlive", true);
                //rb.velocity = Vector2.up * DIED_FORCE;
                animator.SetTrigger("diedTrigger");
                if (glassesEquipped) {
                    glassesAnimator.SetTrigger("dropTrigger");
                }
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
                jumps = defaultJumps;
            }
            if (jumps < defaultJumps) {
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

        if (Input.GetMouseButton(0) && pointerPos.y > jumpYScreenPos && isJumping) {
            if (counter > 0f) {
                rb.velocity = Vector2.up * jumpForce;
                counter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        } else if (Input.GetMouseButton(0) && pointerPos.y <= jumpYScreenPos && !isGrounded) {
            rb.velocity += Vector2.down * downForce * Time.deltaTime;
            if (rb.velocity.y <= -30) {
                velVector.y = -30;
                rb.velocity = velVector;
            }

        } else if (Input.GetMouseButton(0) && pointerPos.y <= jumpYScreenPos && isGrounded && !isSliding) {
            //AudioManager.instance.PlaySlide();
            isSliding = true;
            animator.SetBool("isSliding", true);
            upperCollider.enabled = false;
            if (glassesEquipped) {
                glassesAnimator.SetBool("isSlidingGlasses", true);
            }
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
                jumps = defaultJumps;
            }
            if (jumps < defaultJumps) {
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
            if (glassesEquipped) {
                glassesAnimator.SetBool("isSlidingGlasses", true);
            }
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
        if (glassesEquipped) {
            glassesAnimator.SetTrigger("resetGlassesTrigger");
        }
    }

    private void OnRevive(float delay) {
        StartCoroutine(RevivePlayer(delay));
    }

    private IEnumerator RevivePlayer(float delay) {
        Color32 color = new Color32(255, 255, 255, 0);
        playerRenderer.color = color;
        if (glassesEquipped) {
            glassRenderer.color = color;
        }
        animator.SetTrigger("resetTrigger");
        // Small jump
        rb.velocity = Vector2.up * jumpForce * 1.6f;
        // Special jump params for this case
        jumps = 0;
        isJumping = false;

        yield return new WaitForSeconds(delay / 2);
        color.a = 150;
        playerRenderer.color = color;
        if (glassesEquipped) {
            glassRenderer.color = color;
            glassesAnimator.SetTrigger("resetGlassesTrigger");
        }
        yield return new WaitForSeconds(delay / 2);
        setImmune(true);
        ResetSliding();
        isAlive = true;
        // Dont get score for the currrent collided obstacle
        scoreTimer = 1f;
    }

    private void setImmune(bool immune) {
        immuneTimer = 1.9f;
        isImmune = immune;
        if (glassesEquipped) {
            glassRenderer.color = immune ? new Color32(255, 255, 255, 155) : new Color32(255, 255, 255, 255);
        }
        playerRenderer.color = immune ? new Color32(255, 255, 255, 155) : new Color32(255, 255, 255, 255);
    }

    private void ResetSliding() {
        isSliding = false;
        upperCollider.enabled = true;
        animator.SetBool("isSliding", false);
        if (glassesEquipped) {
            glassesAnimator.SetBool("isSlidingGlasses", false);
        }
    }
}