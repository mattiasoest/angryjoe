using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour {

    public static PlayfabManager instance;

    public Text nameText;

    [HideInInspector]
    public string playerName;

    [HideInInspector]
    public bool hasUsername;

    private const string SCORE_STATISTIC = "highscore";

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

    public void GetLeaderboard() {
        GetLeaderboardRequest requestLeaderBoard = new GetLeaderboardRequest {
            StartPosition = 0,
            StatisticName = SCORE_STATISTIC,
            MaxResultsCount = 15
        };

        PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, result => {
            foreach (PlayerLeaderboardEntry player in result.Leaderboard) {
                Debug.Log($"Name: {player.DisplayName} Score: {player.StatValue}");
            }
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SendHighScore(int score) {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() {
            FunctionName = "sendHighScore",
            FunctionParameter = new { score },
            GeneratePlayStreamEvent = true,
        }, result => {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object messageValue;
            jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
            Debug.Log((string)messageValue);
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void PlayfabLogin() {
        if (!debug) {
            #if UNITY_ANDROID
                LoginWithAndroidDeviceIDRequest requestAndroid = new LoginWithAndroidDeviceIDRequest
                { AndroidDeviceId = GetMobileId(),
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
                    //nameText.enabled = true;
                    //nameText.text = $"Logged in as: {playerName}";
                }
            }, error => {
                Debug.LogError(error.GenerateErrorReport());
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
    }

    private void OnLoginMobileFailure(PlayFabError error) {
        Debug.LogError(error.GenerateErrorReport());
    }

    private string GetMobileId() {
        return SystemInfo.deviceUniqueIdentifier;
    }
}