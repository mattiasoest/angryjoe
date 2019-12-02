using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour {

    public GameObject leaderBoardEntry;
    public ToggleGroup toggleGroup;
    public GameObject noEntriesText;
    public GameObject contentContainer;

    private readonly Stack<LeaderboardEntryUI> entryPool = new Stack<LeaderboardEntryUI>();

    private readonly Stack<LeaderboardEntryUI> activeEntries = new Stack<LeaderboardEntryUI>();


    private const string WEEKLY_NAME = "ToggleWeekly";
    private const string GLOBAL_NAME = "ToggleGlobal";


    private string prevToggleName;


    public Toggle currentSelected {
        get {
            return toggleGroup.ActiveToggles().FirstOrDefault();
        }
    }

    void Awake() {
        LoadingUI.instance.gameObject.SetActive(true);
        PopuplateEntries(PlayfabManager.instance.SCORE_GLOBAL);
        prevToggleName = GLOBAL_NAME;
    }

    public void CloseButton() {
        while (activeEntries.Count > 0) {
            LeaderboardEntryUI entry = activeEntries.Pop();
            entry.gameObject.SetActive(false);
            entryPool.Push(entry);
        }
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
        LoadingUI.instance.gameObject.SetActive(true);

        RecycleEntries();

        PopuplateEntries(PlayfabManager.instance.SCORE_WEEKLY);
    }


    public void WeeklyToggle() {
        if (prevToggleName == WEEKLY_NAME) {
            //Dont refresh leaderboard
            return;
        }
        Debug.Log("weekly");
        prevToggleName = WEEKLY_NAME;
        LoadingUI.instance.gameObject.SetActive(true);

        RecycleEntries();

        PopuplateEntries(PlayfabManager.instance.SCORE_WEEKLY);
    }

    private void RecycleEntries() {
        Debug.Log("==== RECYCLE ENTRIES ====");
        while (activeEntries.Count > 0) {
            LeaderboardEntryUI entry = activeEntries.Pop();
            entry.gameObject.SetActive(false);
            entryPool.Push(entry);
        }
    }


    private void PopuplateEntries(string id) {
        PlayfabManager.instance.GetLeaderboard(id,
            result => {
                int placement = 1;
                foreach (PlayerLeaderboardEntry player in result.Leaderboard) {
                    LeaderboardEntryUI newEntry;
                    if (entryPool.Count > 0) {
                        newEntry = entryPool.Pop();
                        newEntry.gameObject.SetActive(true);
                    } else {
                        newEntry =
                        Instantiate(leaderBoardEntry, contentContainer.transform).GetComponent<LeaderboardEntryUI>();
                    }
                    newEntry.Init(placement, player.DisplayName, player.StatValue);
                    placement++;
                    activeEntries.Push(newEntry);

                    LoadingUI.instance.gameObject.SetActive(false);
                }
            }, error => {
                noEntriesText.SetActive(true);
                LoadingUI.instance.gameObject.SetActive(false);
            });
    }
}
