using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour {
    public Sprite[] obsSprites;

    public static StageController instance;
    public GameObject obstacleFab;

    private const float DEFAULT_SPAWN_TIME = 1.45f;
    private const float LOWER_OBSTACLE_BOUND = -5.2f;
    private const float UPPER_OBSTACLE_BOUND = 4f;

    private enum GAME_STATE { PLAYING, MENU };

    private Stack<Obstacle> obstaclePool = new Stack<Obstacle>();

    private float spawnTimer = DEFAULT_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;

    private int normalRandomCount = 0;

    private int lastRandomSpriteIndex = -1;


    public Sprite getRandomObstacleSprite() {
        int randomIndex = Random.Range(0, 7);

        // Do one more random just in case
        if (randomIndex == lastRandomSpriteIndex) {
            randomIndex = Random.Range(0, 7);
        }
        return obsSprites[randomIndex];
    }


    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start() {
        // Subscribe to the list of events
        GameEventManager.instance.onObstacleRecycle += OnObstacleRecycle;
    }

    // Update is called once per frame
    void Update() {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer < 0) {
            if (obstaclePool.Count > 0) {
                Obstacle pooled = obstaclePool.Pop();
                pooled.gameObject.SetActive(true);
                float randomY = GenerateRandomYpos();
                pooled.Init(randomY);
            } else {
                GameObject obs = Instantiate(obstacleFab);
                float randomY = GenerateRandomYpos();
                obs.GetComponent<Obstacle>().Init(randomY);
            }
            spawnTimer = DEFAULT_SPAWN_TIME;
        }

    }

    private void OnObstacleRecycle(Obstacle obs) {
        obs.gameObject.SetActive(false);
        obstaclePool.Push(obs);
    }

    private float GenerateRandomYpos() {
        float randomY = Random.Range(LOWER_OBSTACLE_BOUND, UPPER_OBSTACLE_BOUND);
        normalRandomCount++;
        if (lastYpos > 3.3f && randomY > 3.3f) {
            normalRandomCount = 0;
            lastLowPos = true;
            randomY = LOWER_OBSTACLE_BOUND;
            //randomY -= Random.Range(1.5f, 3.5f);
        } else if (randomY < -1.94f && randomY > LOWER_OBSTACLE_BOUND) {
            normalRandomCount = 0;
            // Dont have 2 low points in a row
            if (lastLowPos) {
                randomY = Random.Range(-1.85f, 5.2f);
                lastLowPos = false;
            } else {
                // Set lowest point
                lastLowPos = true;
                randomY = LOWER_OBSTACLE_BOUND;
            }
        }

        if (normalRandomCount > 2) {
            normalRandomCount = 0;
            // Set lowest point or highest point
            randomY = Random.Range(0f, 1f) > 0.5f ? LOWER_OBSTACLE_BOUND : UPPER_OBSTACLE_BOUND;

            lastLowPos = randomY.Equals(LOWER_OBSTACLE_BOUND);
        }
        lastYpos = randomY;
        return randomY;
    }
}