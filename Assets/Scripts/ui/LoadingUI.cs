using UnityEngine;

public class LoadingUI : MonoBehaviour {

    public static LoadingUI instance;

    void Awake() {
        instance = this;
    }
}
