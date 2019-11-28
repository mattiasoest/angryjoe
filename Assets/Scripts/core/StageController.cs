using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour {
    public static StageController instance;
    public Player player;
    public Sprite[] obsSprites;
    public Text jumpLabel;
    public Text scoreLabel;
    public GameObject obstacleFab;
    public GameObject mainMenu;

    [HideInInspector]
    public int score;
    [HideInInspector]
    public enum GAME_STATE { GAMEPLAY, MENU };
    [HideInInspector]
    public GAME_STATE currentState = GAME_STATE.MENU;

    private const float DEFAULT_SPAWN_TIME = 1.37f;
    private const float LOWER_OBSTACLE_BOUND = -5.2f;
    private const float UPPER_OBSTACLE_BOUND = 4.3f;

    private readonly Stack<Obstacle> obstaclePool = new Stack<Obstacle>();

    private float spawnTimer = DEFAULT_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;

    private int normalRandomCount;

    private int lastRandomSpriteIndex = -1;

    //MENU HANDLING

    public void PlayButtonCB() {
        mainMenu.SetActive(false);
        scoreLabel.enabled = true;
        // Only play it in some cases
        if (Random.Range(0, 1f) > 0.65f) {
            AudioManager.instance.PlayStartGame();
        }
        Debug.Log("=== GAMEPLAY ===");
        currentState = GAME_STATE.GAMEPLAY;
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
        Debug.Log("=== MAIN MENU ===");
        currentState = GAME_STATE.MENU;
        AudioManager.instance.PlayDeath();
        jumpLabel.enabled = false;
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame() {
        yield return new WaitForSeconds(1.5f);
        AdManager.instance.PlayVideoAd();
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
}