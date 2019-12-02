using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class LeaderboardUI : MonoBehaviour {

    private const string WEEKLY_NAME = "ToggleWeekly";
    private const string GLOBAL_NAME = "ToggleGlobal";


    public ToggleGroup toggleGroup;

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
}
