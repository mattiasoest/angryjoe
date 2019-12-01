using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

public class PlayfabManager : MonoBehaviour {

    public static PlayfabManager instance;

    private const string SCORE_STATISTIC = "highscore";

    public void Awake() {
        instance = this;
    }

    public void Start() {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)) {
            PlayFabSettings.TitleId = "5B418";
        }
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, result => {
            Debug.Log("Logged in to PlayFab!");
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
}