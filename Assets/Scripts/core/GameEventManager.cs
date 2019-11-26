using UnityEngine;
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

    public event Action onPlayerDied;
    public void OnPlayerDied() {
        onPlayerDied();
    }


    public event Action onReset;
    public void OnReset() {
        onReset();
    }

    public event Action<Obstacle> onObstacleRecycle;
    public void ObstacleRecycle(Obstacle obstacle) {
        if (onObstacleRecycle != null) {
            onObstacleRecycle(obstacle);
        }
    }

}
