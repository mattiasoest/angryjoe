using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public SpriteRenderer topRenderer, bottomRenderer;

    [HideInInspector]
    public int point = 1;
    private const float SPEED = -4.6f;
    private Rigidbody2D body;
    private Vector2 posVector;
    private Vector2 velVector = new Vector2(SPEED, 0);
    private readonly Vector2 ZERO_VEC = new Vector2(0, 0);
    private Vector2 camPos;
    

    void Start() {
        camPos = StageController.instance.mainCamera.ViewportToWorldPoint(new Vector2(-0.15f, 0.5f));
        GameEventManager.instance.onPlayerDied += StopMovement;
        GameEventManager.instance.onReset += OnReset;
        GameEventManager.instance.onRevive += OnRevive;
    }

    void Update() {
        if (body.transform.position.x <= camPos.x) {
            GameEventManager.instance.ObstacleRecycle(this);
        }
    }

    public void Init(float yPosition) {
        topRenderer.sprite = StageController.instance.GetRandomObstacleSprite();
        bottomRenderer.sprite = StageController.instance.GetRandomObstacleSprite();
        body = GetComponent<Rigidbody2D>();
        body.velocity = velVector;
        GeneratePosition(yPosition);
        point = 1;
    }

    private void StopMovement() {
        body.velocity = ZERO_VEC;
    }

    private void OnReset() {
        GameEventManager.instance.ObstacleRecycle(this);
    }

    // Player revived continue current state
    private void OnRevive(float delay) {
        StartCoroutine(DelayedResume(delay));
    }

    private IEnumerator DelayedResume(float delay) {
        yield return new WaitForSeconds(delay);
        body.velocity = velVector;
    }

    private void GeneratePosition(float yPosition) {
        posVector.x = 4.25f;
        posVector.y = yPosition;
        body.transform.position = posVector;
    }
}