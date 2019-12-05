using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UsernameUI : MonoBehaviour {

    public InputField inputField;
    public Text invalidText;

    private readonly Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);

    void Awake() {

    }

    public void CloseButton() {
        AudioManager.instance.PlayCloseButton();
        PopupManager.instance.CloseAction(gameObject, () => {
            invalidText.enabled = false;
            gameObject.SetActive(false);
            inputField.text = "";
            GameEventManager.instance.OnFinishGame();
        });
    }

    public void ConfirmButton() {
        AudioManager.instance.PlayNormalButton();

        // Not logged in, dont do anything
        if (!PlayfabManager.instance.loggedIn) {
            invalidText.text = "Failed to update!";
            invalidText.enabled = true;
            return;
        }

        string newName = inputField.text.Trim();
        newName = regex.Replace(newName, @" ");

        if (string.IsNullOrWhiteSpace(newName)) {
            invalidText.text = "Invalid username!";
            invalidText.enabled = true;
        } else if (newName.Length > 22) {
            invalidText.text = "Max 22 characters!";
            invalidText.enabled = true;

        } else if (newName.Length < 3) {
            invalidText.text = "Min 3 characters!";
            invalidText.enabled = true;
        } else {

            PlayfabManager.instance.SetDisplayName(newName,
                result => {
                    CloseButton();
                }, error => {
                    invalidText.text = "Failed to update!";
                    invalidText.enabled = true;
                });
        }
    }
}