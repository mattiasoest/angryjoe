using UnityEngine;
using System.Collections;
using System;

public class GameEventManager : MonoBehaviour {

    public static GameEventManager instance;
    // Use this for initialization
    void Awake() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {

    }


    public event Action<Obstacle> onObstacleRecycle;
    public void ObstacleRecycle(Obstacle obstacle) {
        if (onObstacleRecycle != null) {
            onObstacleRecycle(obstacle);
        }
    }

    public event Action onPlayerdied;
    public void OnPlayerDied() {
        onPlayerdied();
    }
}
