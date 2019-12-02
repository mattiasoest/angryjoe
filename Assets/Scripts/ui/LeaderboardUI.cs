using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class LeaderboardUI : MonoBehaviour {

    public GameObject leaderBoardEntry;
    public ToggleGroup toggleGroup;

    private const string WEEKLY_NAME = "ToggleWeekly";
    private const string GLOBAL_NAME = "ToggleGlobal";


    private string prevToggleName;
    

    public Toggle currentSelected {
        get {
            return toggleGroup.ActiveToggles().FirstOrDefault();
        }
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
