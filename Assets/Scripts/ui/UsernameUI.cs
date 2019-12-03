using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UsernameUI : MonoBehaviour {

    public InputField inputField;
    public Text invalidText;

    private readonly Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);

    public void CloseButton() {
        invalidText.enabled = false;
        gameObject.SetActive(false);
        GameEventManager.instance.OnFinishGame();
    }

    public void ConfirmButton() {
        LoadingUI.instance.gameObject.SetActive(true);
        Debug.Log("Confirm");
        string newName = inputField.text.Trim();
        newName = regex.Replace(newName, @" ");
        if (string.IsNullOrWhiteSpace(newName)) {
            invalidText.text = "Invalid username!";
            invalidText.enabled = true;
        } else if (newName.Length > 22) {
            invalidText.text = "Max 22 characters!";
            invalidText.enabled = true;
        } else {

            PlayfabManager.instance.SetDisplayName(newName,
            result => {
                invalidText.enabled = false;
                gameObject.SetActive(false);
                GameEventManager.instance.OnFinishGame();
            }, error => {
                invalidText.text = "Failed to update!";
                invalidText.enabled = true;
            });

        }
    }
}
