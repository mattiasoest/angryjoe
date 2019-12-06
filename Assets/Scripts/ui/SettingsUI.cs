using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour {

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            gameObject.SetActive(false);
        });
    }

    public void AdjustButton() {
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            // TODO Open FULL UI WITHOUT BUTTONS AND GAMEPLAY
            gameObject.SetActive(false);
        });
    }

    public void ConfirmButton() {
        // TODO save settings locally
        AudioManager.instance.PlayNormalButton();
        CloseButton();
    }
}