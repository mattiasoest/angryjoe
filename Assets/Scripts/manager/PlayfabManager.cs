using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour {

    public static PlayfabManager instance;

    public Text nameText;

    [HideInInspector]
    public string playerName;

    [HideInInspector]
    public bool hasUsername;

    [HideInInspector]
    public readonly string SCORE_GLOBAL = "highscore";
    [HideInInspector]
    public readonly string SCORE_WEEKLY = "highscore_weekly";

    private bool debug;

    public void Awake() {
        instance = this;
    }

    public void Start() {
#if UNITY_EDITOR
        Debug.Log("Unity Editor");
        debug = true;
#endif
        if (string.IsNullOrWhiteSpace(PlayFabSettings.TitleId)) {
            PlayFabSettings.TitleId = "5B418";
        }

        PlayfabLogin();
    }

    public void SetDisplayName(string name) {
        UpdateUserTitleDisplayNameRequest req = new UpdateUserTitleDisplayNameRequest { DisplayName = name };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, result => {
            Debug.Log($"Name updated to -> {name}");
        }, error => {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void GetLeaderboard(string leaderboardId, Action<GetLeaderboardResult> onSucess, Action<PlayFabError> onError) {
        if (leaderboardId != SCORE_GLOBAL && leaderboardId != SCORE_WEEKLY) {
            throw new Exception($"Invalid leaderboard: {leaderboardId}");
        }

        GetLeaderboardRequest requestLeaderBoard = new GetLeaderboardRequest {
            StartPosition = 0,
            StatisticName = leaderboardId,
            MaxResultsCount = leaderboardId == SCORE_GLOBAL ? 50 : 20
        };

        PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, result => {
            onSucess(result);
        }, error => {
            onError(error);
        });
    }

    public void SendHighScore(int score, Action<ExecuteCloudScriptResult> onSucess, Action<PlayFabError> onError) {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "sendHighScore",
            FunctionParameter = new { score },
            GeneratePlayStreamEvent = true,
        }, result => {
            onSucess(result);
        }, error => {
            onError(error);
        });
    }

    private void PlayfabLogin() {
        if (!debug) {
#if UNITY_ANDROID
            LoginWithAndroidDeviceIDRequest requestAndroid = new LoginWithAndroidDeviceIDRequest {
                AndroidDeviceId = GetMobileId(),
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                    GetPlayerProfile = true,
                    ProfileConstraints = new PlayerProfileViewConstraints() {
                        ShowDisplayName = true,
                    }
                }
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
#if UNITY_IOS
            LoginWithIOSDeviceIDRequest requestIOS = new LoginWithIOSDeviceIDRequest {
            DeviceId = ReturnMobileID(),
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                GetPlayerProfile = true,
                ProfileConstraints = new PlayerProfileViewConstraints() {
                    ShowDisplayName = true,
                }
            }
            };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
        } else {
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest {
                CustomId = "GettingStartedGuide",
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() {
                    GetPlayerProfile = true,
                    ProfileConstraints = new PlayerProfileViewConstraints() {
                        ShowDisplayName = true,
                    }
                }
            };
            PlayFabClientAPI.LoginWithCustomID(request, result => {
                string username = result.InfoResultPayload.PlayerProfile.DisplayName;
                if (string.IsNullOrEmpty(username)) {
                    hasUsername = false;
                } else {
                    hasUsername = true;
                    playerName = username;
                    nameText.enabled = true;
                    nameText.text = $"Logged in as: {playerName}";
                }
                LoadingUI.instance.gameObject.SetActive(false);
            }, error => {
                Debug.LogError(error.GenerateErrorReport());
                LoadingUI.instance.gameObject.SetActive(false);
            });
        }
    }

    private void OnLoginMobileSuccess(LoginResult result) {
        string username = result.InfoResultPayload.PlayerProfile.DisplayName;
        if (string.IsNullOrEmpty(username)) {
            hasUsername = false;
        } else {
            hasUsername = true;
            playerName = username;
            nameText.enabled = true;
            nameText.text = $"Logged in as: {playerName}";
        }

        LoadingUI.instance.gameObject.SetActive(false);
    }

    private void OnLoginMobileFailure(PlayFabError error) {
        LoadingUI.instance.gameObject.SetActive(false);
        Debug.LogError(error.GenerateErrorReport());
    }

    private string GetMobileId() {
        return SystemInfo.deviceUniqueIdentifier;
    }
}