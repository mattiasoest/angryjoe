using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour {

    public static StageController instance;
    public GameObject obstacleFab;
    private enum GAME_STATE { PLAYING, MENU };

    private List<GameObject> obstaclePool;

    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Test(string val) {
        Debug.Log($"test from player w value {val}!!");
    }
}