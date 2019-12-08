using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class PopupManager : MonoBehaviour {
    public enum POPUP {
        MAIN,
        LEADERBOARD,
        USERNAME,
        CONTINUE,
        SETTINGS,
        CONTROLS,
    }

    public static PopupManager instance;

    public LeaderboardUI leaderboardUI;
    public UsernameUI usernameUI;
    public ContinueUI continueUI;
    public SettingsUI settingsUI;
    public ControlPanelUI controlPanelUI;
    public GameObject overlay; // TODO

    public GameObject mainMenu; // TODO
    private Vector3 zeroScaleVec = new Vector3(0, 0, 0);
    private Vector3 normalScaleVec = new Vector3(1, 1, 1);
    private const float CLOSE_TIME = 0.09f;
    private const float OPEN_TIME = 0.14f;

    private const float MAIN_MENU_TIME = 0.2f;

    private Animator overlayAnimator;

    void start() { }
    public void ShowPopup(POPUP selectedPopup, bool fadeIn = true) {
        if (fadeIn) {
            overlay.SetActive(true);
            if (overlayAnimator == null) {
                overlayAnimator = overlay.GetComponent<Animator>();
            }

        }
        switch (selectedPopup) {
            case POPUP.MAIN:
                ShowMainMenuUI();
                break;
            case POPUP.LEADERBOARD:
                AudioManager.instance.PlayLeaderboard();
                ShowLeaderboardUI();
                break;
            case POPUP.USERNAME:
                AudioManager.instance.PlayPopup();
                ShowUsernameUI();
                break;
            case POPUP.CONTINUE:
                AudioManager.instance.PlayPopup();
                ShowContinueUI();
                break;

            case POPUP.SETTINGS:
                AudioManager.instance.PlayPopup();
                ShowSettingsUI();
                break;

            case POPUP.CONTROLS:
                AudioManager.instance.PlayPopup();
                ShowControlUI();
                break;
            default:
                throw new UnityException($"Invalid popup: {selectedPopup}");
        }
    }

    public void CloseAction(GameObject toBeClosed, Action onCloseFinish, bool fadeOut = true) {
        LeanTween.scale(toBeClosed, zeroScaleVec, CLOSE_TIME).setEaseInSine().setOnComplete(() => {
            onCloseFinish();
            if (fadeOut) {
                StartCoroutine(FadeOut());
            }
        });
    }

    public void MainMenuCloseAction(Action onCloseFinish = null) {
        LeanTween.moveX(mainMenu, -8f, MAIN_MENU_TIME).setEaseInBack().setOnComplete(() => {
            mainMenu.SetActive(false);
            if (onCloseFinish != null) {
                onCloseFinish();
            }
        });
    }

    void Awake() {
        instance = this;
    }

    private IEnumerator FadeOut() {
        overlayAnimator.SetTrigger("fadeOut");
        yield return new WaitForSeconds(0.15F);
        overlay.SetActive(false);
    }

    private void ShowLeaderboardUI() {
        leaderboardUI.gameObject.transform.localScale = zeroScaleVec;
        leaderboardUI.gameObject.SetActive(true);
        LeanTween.scale(leaderboardUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
        leaderboardUI.RefreshLeaderboard();
    }

    private void ShowSettingsUI() {
        settingsUI.gameObject.transform.localScale = zeroScaleVec;
        settingsUI.gameObject.SetActive(true);
        LeanTween.scale(settingsUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
    }

    private void ShowControlUI() {
        controlPanelUI.gameObject.transform.localScale = zeroScaleVec;
        controlPanelUI.gameObject.SetActive(true);
        controlPanelUI.Init();
        LeanTween.scale(controlPanelUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
    }

    private void ShowMainMenuUI() {
        // Reset to allow main menu click
        StageController.instance.playedClicked = false;
        mainMenu.SetActive(true);
        LeanTween.moveX(mainMenu, 0f, MAIN_MENU_TIME).setEaseOutBack();
    }

    private void ShowUsernameUI() {
        if (!PlayfabManager.instance.loggedIn) {
            // Not logged in? Retry here
            PlayfabManager.instance.PlayfabLogin();
        }
        Analytics.CustomEvent("USERNAME_POPUP");
        usernameUI.gameObject.transform.localScale = zeroScaleVec;
        usernameUI.gameObject.SetActive(true);
        LeanTween.scale(usernameUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
    }

    private void ShowContinueUI() {
        continueUI.gameObject.transform.localScale = zeroScaleVec;
        continueUI.gameObject.SetActive(true);
        LeanTween.scale(continueUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
    }
}