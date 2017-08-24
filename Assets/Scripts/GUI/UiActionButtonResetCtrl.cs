using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiActionButtonCtrl : MonoBehaviour {

# region Members
    Button myButton;

# endregion
    private void Awake()
    {
        myButton = GetComponent<Button>(); // <-- you get access to the button component here

        myButton.onClick.AddListener(Reset);
    }

    

    // Use this for initialization
    void Reset () {
        TrackManager.Instance.Restart();
    }
}
