using System.Collections;
using System.Collections.Generic;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour {
    public static StageController instance;
    public Player player;
    public Sprite[] obsSprites;
    public Text jumpLabel;
    public Text scoreLabel;
    public GameObject dynamicTopBlocker;
    public GameObject obstacleFab;
    public GameObject mainMenu;
    public GameObject userNamerPopup;
    public LeaderboardUI leaderboardUI;

    [HideInInspector]
    public int score;
    [HideInInspector]
    public enum GAME_STATE { GAMEPLAY, MENU };
    [HideInInspector]
    public GAME_STATE currentState = GAME_STATE.MENU;

    private const float DEFAULT_SPAWN_TIME = 1.32f;
    private const float LOWER_OBSTACLE_BOUND = -5.2f;
    private const float UPPER_OBSTACLE_BOUND = 4.3f;

    private readonly Stack<Obstacle> obstaclePool = new Stack<Obstacle>();

    private float spawnTimer = DEFAULT_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;

    private int normalRandomCount;

    private int lastRandomSpriteIndex = -1;

    //MENU HANDLING

    public void PlayButton() {
        mainMenu.SetActive(false);
        scoreLabel.enabled = true;
        // Only play it in some cases
        if (Random.Range(0, 1f) > 0.65f) {
            AudioManager.instance.PlayStartGame();
        }
        Debug.Log("=== GAMEPLAY ===");
        currentState = GAME_STATE.GAMEPLAY;
    }

    public void LeaderBoardButton() {
        leaderboardUI.gameObject.SetActive(true);
        leaderboardUI.RefreshLeaderboard();
    }


    public Sprite GetRandomObstacleSprite() {
        int randomIndex = Random.Range(0, 8);

        // Do one more random just in case
        for (int i = 0; i < 3; i++) {
            if (randomIndex != lastRandomSpriteIndex) {
                break;
            }
            randomIndex = Random.Range(0, 8);
        }
        lastRandomSpriteIndex = randomIndex;
        return obsSprites[randomIndex];
    }

    public void UpdateScore() {
        AudioManager.instance.PlayScore();
        score++;
        scoreLabel.text = $"Score: {score}";
    }


    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start() {
        CameraSetup();
        AdjustDynamicTopCollider();
        // Subscribe to the list of events
        GameEventManager.instance.onObstacleRecycle += OnObstacleRecycle;
        GameEventManager.instance.onPlayerDied += OnPlayerDied;
    }

    // Update is called once per frame
    void Update() {
        switch (currentState) {
            case GAME_STATE.MENU:
                break;
            case GAME_STATE.GAMEPLAY:
                if (player.isAlive) {
                    jumpLabel.enabled = !player.isGrounded;
                    if (player.isJumping) {
                        jumpLabel.text = player.jumps.ToString();
                    }

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
                break;
            default:
                throw new System.Exception("Invalid state");
        }
    }

    private void OnObstacleRecycle(Obstacle obs) {
        obs.gameObject.SetActive(false);
        obstaclePool.Push(obs);
    }

    private void OnPlayerDied() {
        AudioManager.instance.PlayDeath();
        jumpLabel.enabled = false;
        if (string.IsNullOrWhiteSpace(PlayfabManager.instance.playerName)) {
            userNamerPopup.SetActive(true);
        } else {
            ActivateMainMenu();
        }
    }

    private void ActivateMainMenu() {
        Debug.Log("=== MAIN MENU ===");
        currentState = GAME_STATE.MENU;
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame() {
        PlayfabManager.instance.SendHighScore(score, result => {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object messageValue;
            jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
            Debug.Log((string)messageValue);
        }, error => {
            // TODO Retry?
            Debug.Log(error.GenerateErrorReport());
        });
        yield return new WaitForSeconds(1.5f);
        //AdManager.instance.PlayVideoAd();
        GameEventManager.instance.OnReset();
        spawnTimer = DEFAULT_SPAWN_TIME;
        score = 0;
        scoreLabel.text = $"Score: {score}";
        scoreLabel.enabled = false;
        mainMenu.SetActive(true);
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
                randomY = Random.Range(-1.85f, UPPER_OBSTACLE_BOUND);
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

    private void CameraSetup() {
        //float cameraHeight = 8; //800 / 100
        float cameraWidth = 4.8f; //800 / 100
        float desiredAspect = 0.72f; // 480/800
        float ratio = desiredAspect / Camera.main.aspect;
        Camera.main.orthographicSize = cameraWidth * ratio;

        //float defaultWidth = Camera.main.orthographicSize * Camera.main.aspect;
        //Camera.main.orthographicSize = defaultWidth / Camera.main.aspect;
    }


    // Used to have an extra top blocker for ppl witb basically squared screens
    private void AdjustDynamicTopCollider() {
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight + 5, 0));
        Vector2 newPos = new Vector3(0f, cameraPosition.y, 0);
        dynamicTopBlocker.transform.position = newPos;
    }
}