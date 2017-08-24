/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using UnityEngine.SceneManagement;
using Drone.Utils.IO;
using Drone.AI.Evolution;
#endregion

/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    #region Members
    // The camera object, to be referenced in Unity Editor.
    [SerializeField]
    private CameraMovement Camera;

    public ApplicationSettings appSettings;
    

    /// <summary>
    /// The UIController object.
    /// </summary>
    public UIController UIController
    {
        get;
        set;
    }

    public static GameStateManager Instance
    {
        get;
        private set;
    }

    #endregion

    #region Constructors
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameStateManagers in the Scene.");
            return;
        }
        Instance = this;
        appSettings = IOUtils.LoadJson("settings.json");
        //Load gui scene
        SceneManager.LoadScene("GUI", LoadSceneMode.Additive);

        //Load track
        SceneManager.LoadScene(appSettings.trackId, LoadSceneMode.Additive);
    }

    void Start ()
    {
        
        EvolutionManager.Instance.CanSaveBestGenotype = UIController.CanSaveBestGenotype();
        TrackManager.Instance.DroneChanged += OnDroneChanged;
	}
    #endregion

    //#region Methods
    //// Callback method for when the best car has changed.
    private void OnDroneChanged(DroneController newDrone)
    {
       
        Camera.SetTarget(newDrone.gameObject);
        if (UIController != null)
            UIController.SetDisplayTarget(newDrone);

    }
    //#endregion
}
