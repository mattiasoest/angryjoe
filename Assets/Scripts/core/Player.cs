﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private const int DEFAULT_JUMPS = 3;
    private const int DIED_FORCE = 2;

    public Animator animator;
    public int jumps = 3;
    public bool isJumping;
    public bool isGrounded;
    public float speed;
    public Transform feetCollider;
    public float checkRaduis;
    public float jumpForce;
    public float downForce;
    public LayerMask whatIsGround;
    public float jumpTime;
    public BoxCollider2D upperCollider;

    public bool isAlive = true;

    private Rigidbody2D rb;
    private Vector2 velVector = new Vector2();
    private bool isSliding;
    private float counter;
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
        if (isAlive) {

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

            animator.SetBool("isGrounded", isGrounded);
        }
    }


    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Obstacle" && isAlive) {
            isAlive = false;
            //collision.gameObject.SendMessage("ApplyDamage", 10);
            //animator.SetBool("isAlive", true);
            //rb.velocity = Vector2.up * DIED_FORCE;
            animator.SetTrigger("diedTrigger");
            GameEventManager.instance.OnPlayerDied();
        }
    }

    private void OnReset() {
        isAlive = true;
        animator.SetTrigger("resetTrigger");
    }

    private void ResetSliding() {
        isSliding = false;
        upperCollider.enabled = true;
        animator.SetBool("isSliding", false);
    }

    //private void FixedUpdate() {}
}