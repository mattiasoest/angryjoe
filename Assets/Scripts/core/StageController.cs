using System.Collections;
using System.Collections.Generic;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Monetization;
using UnityEngine.UI;

public class StageController : MonoBehaviour {
    public static StageController instance;
    public Camera mainCamera;
    public Player player;
    public Sprite[] obsSprites;
    public Text jumpLabel;
    public Text scoreLabel;
    public GameObject dynamicTopBlocker;
    public GameObject obstacleFab;
    public GameObject mainMenu;
    public GameObject controlDivider;

    public GameObject rewardContainer;
    public LeaderboardUI leaderboardUI;

    public Button removeBannerBtn;
    public Button getExtraJumpBtn;

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

    private float spawnTimer = START_SPAWN_TIME;

    private float lastYpos;
    private bool lastLowPos;

    private int normalRandomCount;

    private int lastRandomSpriteIndex = -1;

    private bool usedRevived;

    void Awake() {
        instance = this;
        float controlY = PlayerPrefs.GetFloat("control_panel_y");
        if (controlY != 0) {
            Vector3 divPos = controlDivider.transform.position;
            divPos.y = controlY;
            controlDivider.transform.position = divPos;
        }
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
                        spawnTimer = DEFAULT_SPAWN_RATE;
                    }
                }
                break;
            default:
                throw new System.Exception("Invalid state");
        }
    }
    public void PlayButton() {
        if (!playedClicked) {
            playedClicked = true;
            controlDivider.SetActive(true);
            AudioManager.instance.PlayStartButton();
            PopupManager.instance.MainMenuCloseAction(() => {
                PlayfabManager.instance.StartGame();
                Debug.Log("=== GAMEPLAY ===");
                currentState = GAME_STATE.GAMEPLAY;
                scoreLabel.enabled = true;
                if (Random.Range(0, 1f) > 0.8f) {
                    StartCoroutine(DelayedBurp());
                }
            });
        }
    }

    public void LeaderBoardButton() {
        PopupManager.instance.ShowPopup(PopupManager.POPUP.LEADERBOARD);
    }

    public void ExtraJumpButton() {
        AudioManager.instance.PlayNormalButton();
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    // TODO animation?
                    AudioManager.instance.PlayGrantItem();
                    StartCoroutine(GrantJumpReward());
                    break;
                case ShowResult.Skipped:
                    Debug.Log("SKIPPED JUMP");
                    break;
                case ShowResult.Failed:
                    Debug.Log("FAILED JUMP");
                    break;
            }
        });
    }

    public void RemoveBannerAdbutton() {
        AudioManager.instance.PlayNormalButton();
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    // TODO animation?
                    AudioManager.instance.PlayGrantItem();
                    StartCoroutine(GrantBannerReward());
                    break;
                case ShowResult.Skipped:
                    Debug.Log("SKIPPED BANNER");
                    break;
                case ShowResult.Failed:
                    Debug.Log("FAILED BANNER");
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
        if (string.IsNullOrWhiteSpace(PlayfabManager.instance.playerName)) {
            StartCoroutine(PromptDelayedUsernamePopup());
        } else {
            if (usedRevived) {
                ActivateMainMenu();
            } else {
                usedRevived = true;
                StartCoroutine(PromptDelayedContinuePopup());
            }
        }
    }

    private void OnContinueGame() {
        long adStartTimeStamp = System.DateTime.Now.Ticks;
        AdManager.instance.PlayRewardedAd(adResult => {
            switch (adResult) {
                case ShowResult.Finished:
                    // Dont waste a players time if the backend will reject it
                    // later if they kept stalling in the ad watching state
                    // Skip extra life.
                    long nowSecond = System.DateTime.Now.Ticks;
                    double timeElapsed = System.TimeSpan.FromTicks((nowSecond - adStartTimeStamp)).TotalSeconds;
                    if (timeElapsed < 110) {
                        AudioManager.instance.PlayContinueGame();
                        GameEventManager.instance.OnRevive(1.2f);
                    } else {
                        ActivateMainMenu(0f);
                    }
                    break;
                case ShowResult.Skipped:
                    ActivateMainMenu(0f);
                    break;
                case ShowResult.Failed:
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
        if (PlayfabManager.instance.hasUsername) {

            PlayfabManager.instance.SendHighScore(score, result => {
                JsonObject jsonResult = (JsonObject)result.FunctionResult;
                object messageValue;
                jsonResult.TryGetValue("messageValue", out messageValue);
                Debug.Log(messageValue);
            }, error => {
                // TODO Retry?
                Debug.Log(error.GenerateErrorReport());
            });
        }
        yield return new WaitForSeconds(delayTime);
        GameEventManager.instance.OnReset();
        spawnTimer = START_SPAWN_TIME;
        score = 0;
        usedRevived = false;
        scoreLabel.text = $"{score}";
        // scoreLabel.enabled = false;
        PopupManager.instance.ShowPopup(PopupManager.POPUP.MAIN, false);
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
}