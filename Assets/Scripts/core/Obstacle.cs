using UnityEngine;

public class Obstacle : MonoBehaviour {
    public SpriteRenderer topRenderer, bottomRenderer;
    private Rigidbody2D body;
	private Vector2 posVector;
    private Vector2 camPos;

    public float speedEffect;

    public void Init(float yPosition) {
        topRenderer.sprite = StageController.instance.getRandomObstacleSprite();
        bottomRenderer.sprite = StageController.instance.getRandomObstacleSprite();
        body = GetComponent<Rigidbody2D>();
        body.velocity = new Vector2(speedEffect, 0);
        GeneratePosition(yPosition);
    }

	void Start() {
        camPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.15f, 0.5f));
        GameEventManager.instance.onPlayerDied += StopMovement;
        GameEventManager.instance.onReset += OnReset;
    }

    void Update() {
        if (body.transform.position.x <= camPos.x) {
            GameEventManager.instance.ObstacleRecycle(this);
        }
    }

    private void StopMovement() {
        body.velocity = new Vector2(0, 0);
    }

    private void OnReset() {
        GameEventManager.instance.ObstacleRecycle(this);
    }

    private void GeneratePosition(float yPosition) {
        posVector.x = 2.1f;
        posVector.y = yPosition;
        body.transform.position = posVector;
    }
}
