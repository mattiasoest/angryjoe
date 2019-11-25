using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRepeat : MonoBehaviour {
    private float imageWidth;
    private Rigidbody2D body;
    private Vector2 posVector = new Vector2();
    public float speedEffect;

    void Start() {
        imageWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(speedEffect, 0);
        posVector.x = body.transform.position.x;
        posVector.y = body.transform.position.y;
        GameEventManager.instance.onPlayerdied += StopRepeat;

    }


    private void StopRepeat() {
        body.velocity = new Vector2(0, 0);
    }

    void Update() {
        if (StageController.instance.isPlayerAlive && body.transform.position.x <= -imageWidth) {
            posVector.x = transform.position.x + imageWidth * 2f;
            body.transform.position = posVector;
        }
    }

    private void FixedUpdate() {
    }
}
