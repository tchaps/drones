using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Drone.AI.Evolution;

public class UIButtonReset : MonoBehaviour
{

    [SerializeField]
    private Button StartButton;

    [SerializeField]
    private Button PauseButton;

    [SerializeField]
    private Button SelectDialogButton;

    [SerializeField]
    private Text PauseButtonText;

    // Use this for initialization

    private void Awake()
    {
        //myButton = GetComponent<Button>(); // <-- you get access to the button component here

        StartButton.onClick.AddListener(Reset);
        PauseButton.onClick.AddListener(PauseResume);
        SelectDialogButton.onClick.AddListener(OpenEditor);
    }



    // Use this for initialization
    void Reset()
    {
        EvolutionManager.Instance.LaunchActivity();
    }

    // Use this for initialization
    void PauseResume()
    {
        EvolutionManager.Instance.PauseResumeActivity();
        if (PauseButtonText.text == "PAUSE")
        {
            PauseButtonText.text = "RESUME";
        }
        else
        {
            PauseButtonText.text = "PAUSE";
        }

    }

    void OpenEditor()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Select Genotype", "", "txt");
        if (path != null)
        {
            EvolutionManager.Instance.PathToInitGenotypeFrom = path;
        }
#endif
    }

}
