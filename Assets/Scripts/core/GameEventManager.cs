using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameEventManager : MonoBehaviour {

    public static GameEventManager instance;

    public event Action onPlayerDied;
    public event Action onReset;
    public event Action<float> onRevive;
    public event Action<Obstacle> onObstacleRecycle;
    public event Action onFinishGame;
    public event Action onContinueGame;

    void Awake() {
        instance = this;
    }

    public void OnPlayerDied() {
        onPlayerDied();
    }

    public void OnReset() {
        onReset();
    }

    public void OnRevive(float delay) {
        onRevive(delay);
    }

    public void OnFinishGame() {
        onFinishGame();
    }

    public void OnContinueGame() {
        onContinueGame();
    }

    public void ObstacleRecycle(Obstacle obstacle) {
        if (onObstacleRecycle != null) {
            onObstacleRecycle(obstacle);
        }
    }

}