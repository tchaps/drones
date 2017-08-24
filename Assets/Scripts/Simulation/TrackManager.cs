/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
using UnityEngine;
using System.Collections.Generic;
#endregion

/// <summary>
/// Singleton class managing the current track and all cars racing on it, evaluating each individual.
/// </summary>
public class TrackManager : MonoBehaviour
{
    #region Members
    public static TrackManager Instance
    {
        get;
        private set;
    }

    // The track camera, to be referenced in Unity Editor.
    [SerializeField]
    private FollowCamera TrackCamera;



    private Checkpoint[] checkpoints;
    public event System.Action<DroneController> DroneChanged;

    /// <summary>
    /// Event for when all agents have died.
    /// </summary>
    public event System.Action AllAgentsDied;

    /// <summary>
    /// Drone used to create new cars and to set start position.
    /// </summary>
    public DroneController PrototypeDrone;
    // Start position for cars
    private Vector3 startPosition;
    private Quaternion startRotation;
    private int currentAgentIndex;

    private String TrackStatus;


    // Struct for storing the current cars and their position on the track.
    private class RaceDrone
    {
        public RaceDrone(DroneController drone = null, uint checkpointIndex = 1)
        {
            this.Drone = drone;
            this.CheckpointIndex = checkpointIndex;
        }
        public DroneController Drone;
        public uint CheckpointIndex;
    }
    private List<RaceDrone> drones = new List<RaceDrone>();

    private List<Agent> listAgents = new List<Agent>();

    /// <summary>
    /// The amount of agents that are currently alive.
    /// </summary>
    public int AgentsAliveCount
    {
        get;
        private set;
    }


    #region Best Drone
    private DroneController bestDrone = null;
    /// <summary>
    /// The current best drone 
    /// </summary>
    public DroneController BestDrone
    {
        get { return bestDrone; }
        private set
        {
            if (bestDrone != value)
            {
                bestDrone = value;
                if (DroneChanged != null)
                {

                    DroneChanged(bestDrone);
                    TrackCamera.SetTarget(bestDrone.gameObject);
                }
            }
        }
    }
    /// <summary>
    /// Event for when the best car has changed.
    /// </summary>
    #endregion


    /// <summary>
    /// The length of the current track in Unity units (accumulated distance between successive checkpoints).
    /// </summary>
    public float TrackLength
    {
        get;
        private set;
    }
    #endregion

    #region Constructors
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Mulitple instance of TrackManager are not allowed in one Scene.");
            return;
        }

        Instance = this;

        //Get all checkpoints
        checkpoints = GetComponentsInChildren<Checkpoint>();
        //Set start position and hide prototype
        startPosition = PrototypeDrone.transform.position;
        startRotation = PrototypeDrone.transform.rotation;
        PrototypeDrone.gameObject.SetActive(false);

        CalculateCheckpointPercentages();
    }

    void Start()
    {
        //Hide checkpoints
        foreach (Checkpoint check in checkpoints)
            check.IsVisible = true;

        
    }
    #endregion

    #region Methods
    // Unity method for updating the simulation
    void Update()
    {
        //Update reward for each enabled car on the track
        UpdateAllDrones();

    }


    private void UpdateAllDrones()
    {
        for (int i = 0; i < drones.Count; i++)
        {
            RaceDrone drone = drones[i];
            if (drone.Drone.enabled)
            {
                drone.Drone.CurrentCompletionReward = GetCompletePerc(drone.Drone, ref drone.CheckpointIndex);

                //Update best Drone
                if (BestDrone == null || drone.Drone.CurrentCompletionReward >= BestDrone.CurrentCompletionReward)
                    BestDrone = drone.Drone;
            }
        }
    }

    internal void ClearDrones()
    {
        foreach (RaceDrone drone in drones)
        {
            if (drone.Drone != null)
            {
                Destroy(drone.Drone.gameObject);
            }
        }
        drones.Clear();
    }

    public void SetAgentList(List<Agent> listAgent)
    {
        this.listAgents = listAgent;
    }


    //Create a new RaceDrone from the prototype
    private RaceDrone CreateDrone(Agent agent)
    {
        GameObject droneCopy = Instantiate(PrototypeDrone.gameObject);
        droneCopy.transform.position = startPosition;
        droneCopy.transform.rotation = startRotation;
        DroneController controllerCopy = droneCopy.GetComponent<DroneController>();
        droneCopy.SetActive(true);
        RaceDrone raceDrone = new RaceDrone(controllerCopy, 1);
        raceDrone.Drone.Agent = agent;
        return raceDrone;
    }


    /// <summary>
    /// Restarts all cars and puts them at the track start.
    /// </summary>
    public void Restart()
    {
        AgentsAliveCount = 0;
        RestartAllAtATime();
    }

    public void PauseResumeSimulation()
    {
        TrackStatus = "Pause";
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else Time.timeScale = 1;
    }


    private void RestartAllAtATime()
    {
        foreach (Agent agent in listAgents)
        {
            agent.AgentDied += OnAgentDied;
            RaceDrone drone = CreateDrone(agent);
            drone.Drone.Restart();
            drone.CheckpointIndex = 1;
            AgentsAliveCount++;
            drones.Add(drone);
        }
    }

    public void StartRace()
    {
        currentAgentIndex = 0;
        Restart();
        TrackStatus = "Running";

    }

    public void OnAgentDied(Agent agent)
    {
        AgentsAliveCount--;

        if (AgentsAliveCount == 0 && AllAgentsDied != null)
        {
            AllAgentsDied();
            TrackStatus = "End";
        }
    }


    /// <summary>
    /// Calculates the percentage of the complete track a checkpoint accounts for. This method will
    /// also refresh the <see cref="TrackLength"/> property.
    /// </summary>
    private void CalculateCheckpointPercentages()
    {
        checkpoints[0].AccumulatedDistance = 0; //First checkpoint is start
        //Iterate over remaining checkpoints and set distance to previous and accumulated track distance.
        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].DistanceToPrevious = Vector3.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            checkpoints[i].AccumulatedDistance = checkpoints[i - 1].AccumulatedDistance + checkpoints[i].DistanceToPrevious;
        }

        //Set track length to accumulated distance of last checkpoint
        TrackLength = checkpoints[checkpoints.Length - 1].AccumulatedDistance;

        //Calculate reward value for each checkpoint
        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].RewardValue = (checkpoints[i].AccumulatedDistance / TrackLength) - checkpoints[i - 1].AccumulatedReward;
            checkpoints[i].AccumulatedReward = checkpoints[i - 1].AccumulatedReward + checkpoints[i].RewardValue;
        }
    }

    /* ajouté */

    // Calculates the completion percentage of given car with given completed last checkpoint.
    // This method will update the given checkpoint index accordingly to the current position.
    private float GetCompletePerc(DroneController drone, ref uint curCheckpointIndex)
    {
        //Already all checkpoints captured
        if (curCheckpointIndex >= checkpoints.Length)
            return 1;

        //Calculate distance to next checkpoint
        float checkPointDistance = Vector3.Distance(drone.transform.position, checkpoints[curCheckpointIndex].transform.position);
        /* a revoir */

        //Check if checkpoint can be captured
        if (checkpoints[curCheckpointIndex].GetComponent<Renderer>().bounds.Contains(drone.transform.position))
        {
            Debug.Log("Checkpoint completed !!!");
            curCheckpointIndex++;
            drone.CheckpointCaptured(); //Inform car that it captured a checkpoint
            return GetCompletePerc(drone, ref curCheckpointIndex); //Recursively check next checkpoint
        }
        else
        {
            //Return accumulated reward of last checkpoint + reward of distance to next checkpoint
            return checkpoints[curCheckpointIndex - 1].AccumulatedReward + checkpoints[curCheckpointIndex].GetRewardValue(checkPointDistance);
        }
    }
    #endregion

}
