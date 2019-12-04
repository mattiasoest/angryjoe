using System;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour {

    public Sprite[] trophies;
    public GameObject leaderBoardEntry;
    public ToggleGroup toggleGroup;
    public GameObject noEntriesText;
    public GameObject contentContainer;

    public Text subtitle;

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

    public void RefreshLeaderboard() {
        // Dont try to send a request not logged in
        if (!PlayfabManager.instance.loggedIn) {
            noEntriesText.SetActive(true);
            return;
        }

        noEntriesText.SetActive(false);
        LoadingUI.instance.gameObject.SetActive(true);
        if (entryPool.Count == 0) {
            //First time, populate with the global
            PopuplateEntries(PlayfabManager.instance.SCORE_GLOBAL);
            prevToggleName = GLOBAL_NAME;
            subtitle.text = "GLOBAL";
        } else {
            switch (currentSelected.name) {
                case GLOBAL_NAME:
                    PopuplateEntries(PlayfabManager.instance.SCORE_GLOBAL);
                    prevToggleName = GLOBAL_NAME;
                    break;
                case WEEKLY_NAME:
                    PopuplateEntries(PlayfabManager.instance.SCORE_WEEKLY);
                    prevToggleName = WEEKLY_NAME;
                    break;
                default:
                    throw new Exception($"Invalid toogle name {currentSelected.name}");
            }
        }

    }

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            while (activeEntries.Count > 0) {
                LeaderboardEntryUI entry = activeEntries.Pop();
                entry.gameObject.SetActive(false);
                entryPool.Push(entry);
            }
            gameObject.SetActive(false);
        });
    }

    public void SelectToggle(int id) {
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
        toggles[id].isOn = true;
    }

    public void GlobalToggle() {
        if (prevToggleName == GLOBAL_NAME) {
            //Dont refresh leaderboard
            return;
        }
        subtitle.text = "GLOBAL";
        AudioManager.instance.PlayToggle();
        Debug.Log("global");
        prevToggleName = GLOBAL_NAME;
        LoadingUI.instance.gameObject.SetActive(true);

        RecycleEntries();
        if (!PlayfabManager.instance.loggedIn) {
            PlayfabManager.instance.PlayfabLogin();
            return;
        }

        PopuplateEntries(PlayfabManager.instance.SCORE_WEEKLY);
    }

    public void WeeklyToggle() {
        if (prevToggleName == WEEKLY_NAME) {
            //Dont refresh leaderboard
            return;
        }
        subtitle.text = "WEEKLY";
        AudioManager.instance.PlayToggle();
        Debug.Log("weekly");
        prevToggleName = WEEKLY_NAME;
        LoadingUI.instance.gameObject.SetActive(true);

        RecycleEntries();
        if (!PlayfabManager.instance.loggedIn) {
            PlayfabManager.instance.PlayfabLogin();
            return;
        }

        PopuplateEntries(PlayfabManager.instance.SCORE_WEEKLY);
    }

    private void RecycleEntries() {
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
                    int trophyIndex = placement < 4 ? placement - 1 : 1;
                    LeaderboardEntryUI newEntry;
                    if (entryPool.Count > 0) {
                        newEntry = entryPool.Pop();
                        newEntry.gameObject.SetActive(true);
                    } else {
                        newEntry =
                            Instantiate(leaderBoardEntry, contentContainer.transform).GetComponent<LeaderboardEntryUI>();
                    }
                    newEntry.Init(placement, player.DisplayName, player.StatValue, trophies[trophyIndex]);
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