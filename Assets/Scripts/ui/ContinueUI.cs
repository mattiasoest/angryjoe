using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueUI : MonoBehaviour {

    private const float DEFAULT_COUNTER_VAL = 6;
    public Text counterText;

    private float counter = DEFAULT_COUNTER_VAL;

    private int toShowVal = (int)DEFAULT_COUNTER_VAL;
    private int prevIntVal = (int)DEFAULT_COUNTER_VAL;

    private bool isOpen = true;
    // Update is called once per frame
    void Update() {
        if (isOpen) {
            counter -= Time.deltaTime;
            if (counter > 0.7f) {
                toShowVal = Mathf.FloorToInt(counter);
                // Only update text once every second
                if (toShowVal != prevIntVal) {
                    prevIntVal = toShowVal;
                    counterText.text = toShowVal.ToString();
                }
            } else {
                isOpen = false;
                CloseButton();
            }
        }
    }

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            ResetCounter();
            ResetOpenState();
            counterText.text = counter.ToString();
            gameObject.SetActive(false);
            GameEventManager.instance.OnFinishGame();
        });
    }

    public void ConfirmButton() {
        AudioManager.instance.PlayNormalButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            ResetCounter();
            ResetOpenState();
            counterText.text = counter.ToString();
            gameObject.SetActive(false);
            GameEventManager.instance.OnContinueGame();
        });
    }

    private void ResetOpenState() {
        isOpen = true;
    }

    private void ResetCounter() {
        counter = DEFAULT_COUNTER_VAL;
        toShowVal = (int)DEFAULT_COUNTER_VAL;
        prevIntVal = (int)DEFAULT_COUNTER_VAL;
    }
}