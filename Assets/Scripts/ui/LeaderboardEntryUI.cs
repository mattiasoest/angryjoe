using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour {

    public Image trophy;
    public Text placementLabel;
    public Text username;
    public Text score;

    // gold FFE423
    // silver E5E5E5
    // bronze EC8900
    // rest FFFFFF, 66 opacity

    public void Init(int placement, string name, int highscore, Sprite trophySprite) {
        trophy.sprite = trophySprite;
        switch (placement) {
            case 1:
            case 2:
            case 3:
                // 1,2,3 100% color
                placementLabel.enabled = false;
                trophy.color = new Color32(255, 255, 255, 255);
                break;
            default:
                placementLabel.enabled = true;
                placementLabel.text = placement.ToString();
                trophy.color = new Color32(0, 0, 0, 100);
                break;

                // case 1:
                //     trophy.color = new Color32(255, 228, 35, 255);
                //     break;
                // case 2:
                //     trophy.color = new Color32(229, 229, 229, 255);
                //     break;
                // case 3:
                //     trophy.color = new Color32(236, 137, 0, 255);
                //     break;
                // default:
                //     trophy.color = new Color32(255, 255, 255, 66);
                //     break;
        }
        username.text = name;
        score.text = $"{highscore}";
    }
}