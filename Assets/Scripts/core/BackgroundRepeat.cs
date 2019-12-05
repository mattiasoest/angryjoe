using System.Collections;
using UnityEngine;

public class BackgroundRepeat : MonoBehaviour {
    public float speedEffect;

    private readonly Vector2 ZERO_VEC = new Vector2(0, 0);

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
        GameEventManager.instance.onRevive += OnRevive;
    }

    void Update() {
        if (body.transform.position.x <= -imageWidth) {
            posVector.x = transform.position.x + imageWidth * 2f;
            body.transform.position = posVector;
        }
    }

    private void StopRepeat() {
        body.velocity = ZERO_VEC;
    }

    private void OnReset() {
        body.velocity = velVector;
    }

    private void OnRevive(float delay) {
        StartCoroutine(DelayedResume(delay));
    }

    private IEnumerator DelayedResume(float delay) {
        yield return new WaitForSeconds(delay);
        body.velocity = velVector;
    }
}