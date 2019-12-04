using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour {
    public enum POPUP {
        MAIN,
        LEADERBOARD,
        USERNAME
    }

    public static PopupManager instance;

    public LeaderboardUI leaderboardUI;
    public UsernameUI usernameUI;
    public GameObject overlay; // TODO
    private Vector3 zeroScaleVec = new Vector3(0, 0, 0);
    private Vector3 normalScaleVec = new Vector3(1, 1, 1);
    private const float CLOSE_TIME = 0.09f;
    private const float OPEN_TIME = 0.14f;

    private Animator overlayAnimator;

    void start() { }
    public void ShowPopup(POPUP selectedPopup) {
        overlay.SetActive(true);
        if (overlayAnimator == null) {
            overlayAnimator = overlay.GetComponent<Animator>();
        }

        switch (selectedPopup) {
            case POPUP.MAIN:
                // TODO ?
                break;
            case POPUP.LEADERBOARD:
                AudioManager.instance.PlayLeaderboard();
                ShowLeaderboardUI();
                break;
            case POPUP.USERNAME:
                AudioManager.instance.PlayPopup();
                ShowUsernameUI();
                break;
            default:
                throw new UnityException($"Invalid popup: {selectedPopup}");
        }
    }

    public void CloseAction(GameObject toBeClosed, Action onCloseFinish) {
        LeanTween.scale(toBeClosed, zeroScaleVec, CLOSE_TIME).setEaseInSine().setOnComplete(() => {
            onCloseFinish();
            StartCoroutine(FadeOut());
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

    private void ShowUsernameUI() {
        usernameUI.gameObject.transform.localScale = zeroScaleVec;
        usernameUI.gameObject.SetActive(true);
        LeanTween.scale(usernameUI.gameObject, normalScaleVec, OPEN_TIME).setEaseOutBack();
    }
}