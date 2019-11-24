using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour {
    public static StageController instance;
    public GameObject obstacleFab;

    private static float DEFAULT_SPAWN_TIME = 1.5f;

    private enum GAME_STATE { PLAYING, MENU };

    private Stack<Obstacle> obstaclePool = new Stack<Obstacle>();

    private float spawnTimer = DEFAULT_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;


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
        float randomY = Random.Range(-5.05f, 5.2f);
        if (lastYpos > 4f && randomY > 4f) {
            print("Adjusted Y POS!!!");
            randomY -= Random.Range(1f, 2.5f);
        } else if (randomY < -1.9f && randomY > -5.05f) {
            // Dont have 2 low points in a row
            if (lastLowPos) {
                print("Not Allowed!!!! genereated new higher pos!");
                randomY = Random.Range(-1.8f, 5.2f);
                lastLowPos = false;
            } else {
                print("LOW VAL ADJUSTED");
                // Set lowest point
                lastLowPos = true;
                randomY = -5.05f;
            }
        }
        lastYpos = randomY;
        return randomY;
    }
}