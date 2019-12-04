using System;
using UnityEngine;

public class GameEventManager : MonoBehaviour {

    public static GameEventManager instance;

    public event Action onPlayerDied;
    public event Action onReset;
    public event Action<Obstacle> onObstacleRecycle;
    public event Action onFinishGame;
    public event Action onContinueGame;
    public event Action onUsernameUIClose;
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

    public void OnFinishGame() {
        onFinishGame();
    }

    public void OnContinueGame() {
        onContinueGame();
    }

    public void OnUsernameUIClose() {
        onUsernameUIClose();
    }

    public void ObstacleRecycle(Obstacle obstacle) {
        if (onObstacleRecycle != null) {
            onObstacleRecycle(obstacle);
        }
    }

}