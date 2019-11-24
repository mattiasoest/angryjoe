﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	private Rigidbody2D body;
	private Vector2 posVector = new Vector2();
    private Vector2 camPos;

    public float speedEffect;

    public void Init(float yPosition) {
        body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(speedEffect, 0);
        GeneratePosition(yPosition);
    }

	void Start() {
        camPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.15f, 0.5f));
       }


	void Update(){ }

	private void FixedUpdate() {

		if (body.transform.position.x <= camPos.x) {
            GameEventManager.instance.ObstacleRecycle(this);
		}
	}

    private void GeneratePosition(float yPosition) {
        posVector.x = 2.1f;
        //posVector.y = body.position.y;
        posVector.y = yPosition;
        body.transform.position = posVector;
    }
}
