using UnityEngine;
using System;

public class GameEventManager : MonoBehaviour {

    public static GameEventManager instance;

    public event Action onPlayerDied;
    public event Action onReset;
    public event Action<Obstacle> onObstacleRecycle;
    // Use this for initialization
    void Awake() {
        instance = this;
    }


    public void OnPlayerDied() {
        onPlayerDied();
    }



    public void OnReset() {
        onReset();
    }


    public void ObstacleRecycle(Obstacle obstacle) {
        if (onObstacleRecycle != null) {
            onObstacleRecycle(obstacle);
        }
    }

}
