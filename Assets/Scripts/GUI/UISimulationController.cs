/// Author: Samuel Arzt
/// Date: March 2017


#region Includes
using UnityEngine.UI;
using UnityEngine;
using System;
using Drone.AI.Evolution;
#endregion

/// <summary>
/// Class for controlling the various ui elements of the simulation
/// </summary>
public class UISimulationController : MonoBehaviour
{
    #region Members
    private DroneController target;
    /// <summary>
    /// The Drone to fill the GUI data with.
    /// </summary>
    /// 
    private UINeuralNetworkConnectionPanel SelectedNode
    {
        get;
        set;
    }
    public DroneController Target
    {
        get { return target; }
        set
        {
            if (target != value)
            {
                target = value;

                if (target != null)
                    NeuralNetPanel.Display(target.Agent.FNN);
            }
        }
    }


    // GUI element references to be set in Unity Editor.
    [SerializeField]
    private Text[] InputTexts;
    [SerializeField]
    private Text Evaluation;
    [SerializeField]
    private Text GenerationCount;
    [SerializeField]
    private UINeuralNetworkPanel NeuralNetPanel;
    [SerializeField]
    private Toggle SaveBestGenotype;
   
    #endregion

    #region Constructors
    void Awake()
    {
        NeuralNetPanel.SelectedNodeChanged += SelectedNodeChanged;
    }
    #endregion

    #region Methods

    public void SelectedNodeChanged(UINeuralNetworkConnectionPanel newNode)
    {
        SelectedNode = newNode;
    }

    void Update()
    {
        if (Target != null)
        {
            //Display controls
            if (Target.CurrentControlInputs != null)
            {
                for (int i = 0; i < InputTexts.Length; i++)
                    InputTexts[i].text = Target.CurrentControlInputs[i].ToString();
            }

            //Display evaluation and generation count
            Evaluation.text = Target.Agent.Genotype.Evaluation.ToString();
            GenerationCount.text = EvolutionManager.Instance.GenerationCount.ToString();
        }
    }

    /// <summary>
    /// Starts to display the gui elements.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Stops displaying the gui elements.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Boolean CanSaveBestGenotype()
    {
        return SaveBestGenotype.isOn;
    }
    #endregion
}
