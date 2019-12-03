using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseButton() {
        gameObject.SetActive(false);
        GameEventManager.instance.OnFinishGame();
    }

    public void ConfirmButton() {
        // TODO parse name
        Debug.Log("Confirm");
        gameObject.SetActive(false);
        GameEventManager.instance.OnFinishGame();
    }
}
