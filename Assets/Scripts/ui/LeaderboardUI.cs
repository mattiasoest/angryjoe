using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using PlayFab.ClientModels;

public class LeaderboardUI : MonoBehaviour {

    public GameObject leaderBoardEntry;
    public ToggleGroup toggleGroup;
    public GameObject noEntriesText;
    public GameObject contentContainer;


    private const string WEEKLY_NAME = "ToggleWeekly";
    private const string GLOBAL_NAME = "ToggleGlobal";


    private string prevToggleName;
    

    public Toggle currentSelected {
        get {
            return toggleGroup.ActiveToggles().FirstOrDefault();
        }
    }

    void Awake() {
        PlayfabManager.instance.GetLeaderboard(PlayfabManager.instance.SCORE_GLOBAL,
            result => {

                int placement = 1;
                foreach (PlayerLeaderboardEntry player in result.Leaderboard) {
                    Debug.Log($"Name: {player.DisplayName} Score: {player.StatValue}");
                    GameObject newEntry = Instantiate(leaderBoardEntry, contentContainer.transform);
                    newEntry.GetComponent<LeaderboardEntryUI>().Init(placement, player.DisplayName, player.StatValue);
                    placement++;
                }
            }, error => {
                noEntriesText.SetActive(true);
            }); 
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseButton() {
        gameObject.SetActive(false);
    }

    public void SelectToggle(int id) {
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        toggles[id].isOn = true;
    }

    public void Toggle() {
        switch (currentSelected.name) {
            case GLOBAL_NAME:
                Debug.Log("global");
                break;
            case WEEKLY_NAME:
                Debug.Log("weekly");
                break;
            default:
                throw new Exception($"Invalid toogle name {currentSelected.name}");
        }
    }

    public void GlobalToggle() {
        if (prevToggleName == GLOBAL_NAME) {
            //Dont refresh leaderboard
            return;
        }
        Debug.Log("global");
        prevToggleName = GLOBAL_NAME;
    }


    public void WeeklyToggle() {
        if (prevToggleName == WEEKLY_NAME) {
            //Dont refresh leaderboard
            return;
        }
        Debug.Log("weekly");
        prevToggleName = WEEKLY_NAME;
    }
}
