using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {

    public GameObject generalSoundToggle;
    // public Toogle jumpToggle;
    // public Toogle scoreToggle;

    public void CloseButton() {
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
            // TODO Open FULL UI WITHOUT BUTTONS AND GAMEPLAY
            // TODO FIX WITH TOUCH INPUT ETC
            // Debug.Log(StageController.instance.controlDivider.transform.position.y);
            // StageController.instance.controlDivider.transform.position = new Vector2(StageController.instance.controlDivider.transform.position.x, StageController.instance.controlDivider.transform.position.y * -1.5f);
            // gameObject.SetActive(false);
            // Debug.Log(StageController.instance.controlDivider.transform.position.y);
            PopupManager.instance.ShowPopup(PopupManager.POPUP.CONTROLS);
        });
    }

    public void ConfirmButton() {
        // TODO save settings locally
        AudioManager.instance.PlayNormalButton();
        CloseButton();
    }

    public void AllSFXToggle() {
        Debug.Log("SOUND - " + generalSoundToggle.GetComponent<Toggle>().isOn);
    }

        public void ScoreSFXToggle() {
        Debug.Log("SCORE - " + generalSoundToggle.GetComponent<Toggle>().isOn);
    }
}