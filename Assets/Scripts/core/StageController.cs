using System.Collections;
using System.Collections.Generic;
using MLAgents;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class StageController : Area {
    public static StageController instance;
    public Camera mainCamera;
    public Player player;
    public Sprite[] obsSprites;
    public Text jumpLabel;
    public Text scoreLabel;
    public Text RewardLabel;
    public GameObject dynamicTopBlocker;
    public GameObject obstacleFab;
    public GameObject mainMenu;
    public GameObject controlDivider;

    public GameObject rewardContainer;
    public LeaderboardUI leaderboardUI;

    public Button removeBannerBtn;
    public Button getExtraJumpBtn;

    public bool isAIPlaying = true;

    [HideInInspector]
    public int score;
    [HideInInspector]
    public enum GAME_STATE { GAMEPLAY, MENU }

    [HideInInspector]
    public GAME_STATE currentState = GAME_STATE.MENU;
    [HideInInspector]
    public bool playedClicked;

    private const float START_SPAWN_TIME = 1f;
    private const float DEFAULT_SPAWN_RATE = 1.26f;
    private const float LOWER_OBSTACLE_BOUND = -5.2f;
    private const float UPPER_OBSTACLE_BOUND = 4.3f;

    private readonly Stack<Obstacle> obstaclePool = new Stack<Obstacle>();
    [HideInInspector]
    public readonly List<Obstacle> obstacleList = new List<Obstacle>();

    private float spawnTimer = START_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;

    private int normalRandomCount;

    private int lastRandomSpriteIndex = -1;

    private bool usedRevived = false;
    private bool showedContinue = false;

    private Vector2 playerDefaultPos;

    void Awake() {
        instance = this;
        float controlY = PlayerPrefs.GetFloat("control_panel_y");
        if (controlY != 0) {
            Vector3 divPos = controlDivider.transform.position;
            divPos.y = controlY;
            controlDivider.transform.position = divPos;
        }
        for (int i = 0; i < 2; i++) {
            GameObject obs = Instantiate(obstacleFab);
            float randomY = GenerateRandomYpos();
            Obstacle obsObj = obs.GetComponent<Obstacle>();
            obsObj.Init(randomY);
            obs.gameObject.SetActive(false);
            obstaclePool.Push(obsObj);
            // Use the list for AI observations
            obstacleList.Add(obsObj);
        }

        playerDefaultPos = new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y);
    }
    // Start is called before the first frame update
    void Start() {
        CameraSetup();
        AdjustDynamicTopCollider();
        // Subscribe to the list of events
        GameEventManager.instance.onObstacleRecycle += OnObstacleRecycle;
        GameEventManager.instance.onPlayerDied += OnPlayerDied;
        GameEventManager.instance.onContinueGame += OnContinueGame;
        GameEventManager.instance.onFinishGame += OnFinishGame;
    }

    void FixedUpdate() {
        // void Update() {
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
                        spawnTimer = DEFAULT_SPAWN_RATE;
                    }
                }
                break;
            default:
                throw new System.Exception("Invalid state");
        }

        // RewardLabel.text = player.GetCumulativeReward().ToString("0.00");
    }
    public void PlayButton() {
        if (!playedClicked) {
            playedClicked = true;
            controlDivider.SetActive(true);
            AudioManager.instance.PlayStartButton();
            PopupManager.instance.MainMenuCloseAction(() => {
                PlayfabManager.instance.StartGame();
                FastStartGame();
            });
        }
    }

    public void FastStartGame() {
        Debug.Log("=== GAMEPLAY ===");
        currentState = GAME_STATE.GAMEPLAY;
        scoreLabel.enabled = true;
        if (Random.Range(0, 1f) > 0.8f) {
            StartCoroutine(DelayedBurp());
        }
    }

    public void LeaderBoardButton() {
        PopupManager.instance.ShowPopup(PopupManager.POPUP.LEADERBOARD);
    }

    public void ExtraJumpButton() {
        string id = "extra_jump";
        AudioManager.instance.PlayNormalButton();
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    trackAd("AD_WATCHED", id);
                    AudioManager.instance.PlayGrantItem();
                    StartCoroutine(GrantJumpReward());
                    break;
                case ShowResult.Skipped:
                    trackAd("AD_SKIPPED", id);
                    break;
                case ShowResult.Failed:
                    trackAd("AD_FAILED", id);
                    break;
            }
        });
    }

    public void RemoveBannerAdbutton() {
        string id = "remove_banner";
        AudioManager.instance.PlayNormalButton();
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    trackAd("AD_WATCHED", id);
                    AudioManager.instance.PlayGrantItem();
                    StartCoroutine(GrantBannerReward());
                    break;
                case ShowResult.Skipped:
                    trackAd("AD_SKIPPED", id);
                    break;
                case ShowResult.Failed:
                    trackAd("AD_FAILED", id);
                    break;
            }
        });
    }

    public void UsernameButton() {
        PopupManager.instance.ShowPopup(PopupManager.POPUP.USERNAME);
    }

    public void SettingsButton() {
        PopupManager.instance.ShowPopup(PopupManager.POPUP.SETTINGS);
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
        scoreLabel.text = $"{score}";
    }

    private IEnumerator DelayedBurp() {
        yield return new WaitForSeconds(0.8f);
        AudioManager.instance.PlayStartGame();
    }

    private IEnumerator GrantBannerReward() {
        rewardContainer.SetActive(true);
        rewardContainer.GetComponentInChildren<Text>().text = "REMOVED";
        AdManager.instance.DestroyBannerAd();
        removeBannerBtn.interactable = false;
        yield return new WaitForSeconds(1.233f);
        rewardContainer.SetActive(false);
    }

    private IEnumerator GrantJumpReward() {
        rewardContainer.SetActive(true);
        rewardContainer.GetComponentInChildren<Text>().text = "+1 JUMP (+GIFT)";
        getExtraJumpBtn.interactable = false;
        player.GrantJumpReward();
        yield return new WaitForSeconds(1.233f);
        rewardContainer.SetActive(false);
    }

    private void OnObstacleRecycle(Obstacle obs) {
        obs.gameObject.SetActive(false);
        obstaclePool.Push(obs);
    }

    private void OnPlayerDied() {
        AudioManager.instance.PlayDeath();
        jumpLabel.enabled = false;
        if (isAIPlaying) {
            player.gameObject.transform.position = playerDefaultPos;
            StartCoroutine(ResetGame(0f));
            return;
        }
        if (string.IsNullOrWhiteSpace(PlayfabManager.instance.playerName)) {
            StartCoroutine(PromptDelayedUsernamePopup());
        } else {
            if (showedContinue) {
                ActivateMainMenu();
            } else {
                showedContinue = true;
                StartCoroutine(PromptDelayedContinuePopup());
            }
        }
    }

    private void OnContinueGame() {
        Analytics.CustomEvent("CONTINUE_GAME", new Dictionary<string, object> { { "score", score },
        });
        string id = "continue";
        long adStartTimeStamp = System.DateTime.Now.Ticks;
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    // Dont waste a players time if the backend will reject it
                    // later if they kept stalling in the ad watching state
                    // Skip extra life.
                    trackAd("AD_WATCHED", id);
                    long nowSecond = System.DateTime.Now.Ticks;
                    double timeElapsed = System.TimeSpan.FromTicks((nowSecond - adStartTimeStamp)).TotalSeconds;
                    if (timeElapsed < 100) {
                        AudioManager.instance.PlayContinueGame();
                        GameEventManager.instance.OnRevive(1.2f);
                        usedRevived = true;
                    } else {
                        Analytics.CustomEvent("SKIPPED_CONTINUE", new Dictionary<string, object> { { "time_elapsed", timeElapsed },
                        });
                        ActivateMainMenu(0f);
                    }
                    break;
                case ShowResult.Skipped:
                    trackAd("AD_SKIPPED", id);
                    ActivateMainMenu(0f);
                    break;
                case ShowResult.Failed:
                    trackAd("AD_FAILED", id);
                    ActivateMainMenu(0f);
                    break;
            }
        });
    }

    private void OnFinishGame() {
        if (currentState == GAME_STATE.GAMEPLAY) {
            // No delay if we're coming from a popup
            ActivateMainMenu(0f);
        }
    }

    private void ActivateMainMenu(float delay = 1.2f) {
        controlDivider.SetActive(false);
        Debug.Log("=== MAIN MENU ===");
        currentState = GAME_STATE.MENU;
        StartCoroutine(ResetGame(delay));
    }

    private IEnumerator ResetGame(float delayTime) {
        if (!isAIPlaying) {

            if (PlayfabManager.instance.hasUsername) {
                int scoreBeforeReset = score;
                bool reviveStatusBeforeReset = usedRevived;
                PlayfabManager.instance.SendHighScore(score, result => {
                    JsonObject jsonResult = (JsonObject)result.FunctionResult;
                    object messageValue;
                    jsonResult.TryGetValue("messageValue", out messageValue);
                    Analytics.CustomEvent("FINISH_GAME", new Dictionary<string, object> { { "score", scoreBeforeReset },
                        { "revived", reviveStatusBeforeReset },
                        { "message", messageValue },
                    });
                }, error => {
                    Analytics.CustomEvent("FINISH_GAME_ERROR", new Dictionary<string, object> { { "score", scoreBeforeReset },
                        { "revived", reviveStatusBeforeReset },
                        { "error_message", error.GenerateErrorReport() },
                    });
                });
            }
        }
        yield return new WaitForSeconds(delayTime);
        GameEventManager.instance.OnReset();
        spawnTimer = START_SPAWN_TIME;
        score = 0;
        usedRevived = false;
        showedContinue = false;
        scoreLabel.text = $"{score}";
        // scoreLabel.enabled = false;
        if (!isAIPlaying) {
            PopupManager.instance.ShowPopup(PopupManager.POPUP.MAIN, false);
        } else {
            currentState = GAME_STATE.GAMEPLAY;
        }
    }

    private IEnumerator PromptDelayedUsernamePopup() {
        yield return new WaitForSeconds(0.7f);
        PopupManager.instance.ShowPopup(PopupManager.POPUP.USERNAME);
    }
    private IEnumerator PromptDelayedContinuePopup() {
        yield return new WaitForSeconds(0.7f);
        PopupManager.instance.ShowPopup(PopupManager.POPUP.CONTINUE);
    }

    private float GenerateRandomYpos() {
        float randomY = Random.Range(LOWER_OBSTACLE_BOUND, UPPER_OBSTACLE_BOUND);
        normalRandomCount++;
        if (lastYpos > 3.3f && randomY > 3.3f) {
            normalRandomCount = 0;
            lastLowPos = true;
            randomY = LOWER_OBSTACLE_BOUND;
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
        float cameraWidth = 4.8f; // 480 / 100
        float desiredAspect = 0.72f;
        float ratio = desiredAspect / mainCamera.aspect;
        mainCamera.orthographicSize = cameraWidth * ratio;
    }

    // Used to have an extra top blocker for ppl witb basically squared screens
    private void AdjustDynamicTopCollider() {
        Vector3 cameraPosition = mainCamera.ScreenToWorldPoint(new Vector3(0, mainCamera.pixelHeight + 5, 0));
        Vector2 newPos = new Vector3(0f, cameraPosition.y, 0);
        dynamicTopBlocker.transform.position = newPos;
    }

    private void trackAd(string eventName, string typeID) {
        Analytics.CustomEvent(eventName, new Dictionary<string, object> { { "type", typeID },
        });
    }

    // ================ AI

    public override void ResetArea() {
        // StartCoroutine(ResetGame(1.2f));
        StartCoroutine(ResetGame(0f));
    }
}