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
                pooled.init();
            } else {
                Instantiate(obstacleFab);
            }
            spawnTimer = DEFAULT_SPAWN_TIME;
        }

    }

    private void OnObstacleRecycle(Obstacle obs) {
        obs.gameObject.SetActive(false);
        obstaclePool.Push(obs);
    }
}