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

    private float defaultPos;
    private Vector3 prevPos;

    public void Init() {
        defaultPos = StageController.instance.mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height * 0.21f, 0)).y;
        prevPos = StageController.instance.controlDivider.transform.position;
    }

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
                StageController.instance.controlDivider.GetComponent<Image>().color = new Color32(20, 255, 0, 125);
            }
        } else {
            Vector3 mvDiv2 = StageController.instance.controlDivider.transform.position;
            updateButtonPos.y = mvDiv2.y;
            updateButtonPos.x = moveButton.transform.position.x;
            moveButton.transform.position = updateButtonPos;
        }
    }

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        DiscardChanges();
        Close();
    }

    public void SaveButton() {
        AudioManager.instance.PlayNormalButton();
        SaveChanges();
        Close();
    }

    public void MoveButton() {
        StageController.instance.controlDivider.GetComponent<Animator>().enabled = false;
        StageController.instance.controlDivider.GetComponent<Image>().color = new Color32(20, 255, 0, 255);
        moveDivider = true;
    }

    public void ResetButton() {
        updateButtonPos.x = moveButton.transform.position.x;
        // StageController.instance.controlDivider.transform.position = prevPos;
        Vector3 temp = StageController.instance.controlDivider.transform.position;
        temp.y = defaultPos;
        StageController.instance.controlDivider.transform.position = temp;
        moveButton.transform.position = updateButtonPos;
    }

    private void DiscardChanges() {
        updateButtonPos.x = moveButton.transform.position.x;
        StageController.instance.controlDivider.transform.position = prevPos;
        moveButton.transform.position = updateButtonPos;
    }

    private void SaveChanges() {
        PlayerPrefs.SetFloat("control_panel_y", StageController.instance.controlDivider.transform.position.y);
    }

    private void Close() {
        StageController.instance.controlDivider.SetActive(false);
        PopupManager.instance.CloseAction(gameObject, () => {
            StageController.instance.controlDivider.GetComponent<Animator>().enabled = true;
            gameObject.SetActive(false);
            PopupManager.instance.ShowPopup(PopupManager.POPUP.MAIN, false);
        });
    }
}