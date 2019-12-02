using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour {

    public Image trophy;
    public Text username;
    public Text score;
    public Text placementLabel;

    // gold FFE423
    // silver E5E5E5
    // bronze EC8900
    // rest FFFFFF, 66 opacity

    public void Init(int placement, string name, int highscore) {
        switch (placement) {
            case 1:
                Debug.Log("ONE");
                trophy.color = new Color32(255, 228, 35, 255);
                break;
            case 2:
                trophy.color = new Color32(229, 229, 229, 255);
                break;
            case 3:
                trophy.color = new Color32(236, 137, 0, 255);
                break;
            default:
                trophy.color = new Color32(255, 255, 255, 66);
                break;
        }
        username.text = name;
        score.text = $"Score: {highscore}";
        placementLabel.text = placement.ToString();
    }
}
