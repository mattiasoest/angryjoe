using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {

    public GameObject allSfxToggle;
    public GameObject scoreSfxToggle;


    void Start() {
        scoreSfxToggle.GetComponent<Toggle>().isOn = AudioManager.instance.isScoreSfxOn;
        allSfxToggle.GetComponent<Toggle>().isOn = AudioManager.instance.isSfxOn;
    }
    public void CloseButton() {
        PlayerPrefs.SetInt("all_sfx", AudioManager.instance.isSfxOn ? 1 : -1);
        PlayerPrefs.SetInt("score_sfx", AudioManager.instance.isScoreSfxOn ? 1 : -1);
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            gameObject.SetActive(false);
        });
    }

    public void AdjustButton() {
        PopupManager.instance.MainMenuCloseAction();
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            StageController.instance.controlDivider.SetActive(true);
            PopupManager.instance.ShowPopup(PopupManager.POPUP.CONTROLS);
        });
    }

    public void ConfirmButton() {
        AudioManager.instance.PlayNormalButton();
        CloseButton();
    }

    public void AllSFXToggle() {
        AudioManager.instance.PlayToggle();
        AudioManager.instance.isSfxOn = allSfxToggle.GetComponent<Toggle>().isOn;
    }

    public void ScoreSFXToggle() {
        AudioManager.instance.PlayToggle();
        AudioManager.instance.isScoreSfxOn = scoreSfxToggle.GetComponent<Toggle>().isOn;
    }
}