using UnityEngine;

public class BackgroundRepeat : MonoBehaviour {
    public float speedEffect;

    private float imageWidth;
    private Rigidbody2D body;
    private Vector2 posVector = new Vector2();
    private Vector2 velVector;

    void Start() {
        imageWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        body = GetComponent<Rigidbody2D>();
        velVector = new Vector2(speedEffect, 0);
        body.velocity = velVector;
        posVector.x = body.transform.position.x;
        posVector.y = body.transform.position.y;
        GameEventManager.instance.onPlayerDied += StopRepeat;
        GameEventManager.instance.onReset += OnReset;
    }

    private void StopRepeat() {
        body.velocity = new Vector2(0, 0);
    }

    private void OnReset() {
        body.velocity = velVector;
    }

    void Update() {
        if (body.transform.position.x <= -imageWidth) {
            posVector.x = transform.position.x + imageWidth * 2f;
            body.transform.position = posVector;
        }
    }
}