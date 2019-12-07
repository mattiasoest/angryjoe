using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelUI : MonoBehaviour {

    public Button moveButton;

    private bool moveDivider;

    private Vector3 updatedControlDividerPos;
    private Vector3 updateButtonPos;

    private Color32 dividerColor;


    void Update() {
        if (moveDivider) {
            Vector3 mvDiv = StageController.instance.controlDivider.transform.position;
            Vector3 pointerPos = Input.mousePosition;

            // Cap it
            if (pointerPos.y > Screen.height * 0.57f) {
                pointerPos.y = Screen.height * 0.57f;
            } else if (pointerPos.y < Screen.height * 0.06f) {
                pointerPos.y = Screen.height * 0.06f;
            }
            Vector3 pointerPosTransformed = StageController.instance.mainCamera.ScreenToWorldPoint(pointerPos);
            updatedControlDividerPos.x = mvDiv.x;
            updatedControlDividerPos.y = pointerPosTransformed.y;
            updatedControlDividerPos.z = mvDiv.z;
            updateButtonPos = updatedControlDividerPos;
            // Keep x value for button
            updateButtonPos.x = moveButton.transform.position.x;
            StageController.instance.controlDivider.transform.position = updatedControlDividerPos;
            moveButton.transform.position = updateButtonPos;
            if (Input.GetMouseButtonUp(0)) {
                moveDivider = false;
                StageController.instance.controlDivider.GetComponent<Image>().color = new Color32(255, 0, 0, 100);
            }
        } else {
            // Cant have this in start without a coroutine and some delay, keep it here 
            // for now
            Vector3 mvDiv2 = StageController.instance.controlDivider.transform.position;
            updateButtonPos.y = mvDiv2.y;
            updateButtonPos.x = moveButton.transform.position.x;
            moveButton.transform.position = updateButtonPos;
        }
    }

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        StageController.instance.controlDivider.SetActive(false);
        PopupManager.instance.CloseAction(gameObject, () => {
            StageController.instance.controlDivider.GetComponent<Animator>().enabled = true;
            gameObject.SetActive(false);
            PopupManager.instance.ShowPopup(PopupManager.POPUP.MAIN, false);
        });
    }

    public void SaveButton() {
        // TODO save controls locally
        AudioManager.instance.PlayNormalButton();
        CloseButton();
    }

    public void MoveButton() {
        StageController.instance.controlDivider.GetComponent<Animator>().enabled = false;
        // LeanTween.color(StageController.instance.controlDivider, new Color(1f, 0f, 0f, 1f), 0.3f);
        StageController.instance.controlDivider.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
        moveDivider = true;
    }
}