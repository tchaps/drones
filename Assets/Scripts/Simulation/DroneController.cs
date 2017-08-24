/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
#endregion

/// <summary>
/// Class representing a controlling container for a 2D physical simulation
/// of a car with 5 front facing sensors, detecting the distance to obstacles.
/// </summary>
public class DroneController : MonoBehaviour
{
    #region Members
    #region IDGenerator
    // Used for unique ID generation
    private static int idGenerator = 0;
    /// <summary>
    /// Returns the next unique id in the sequence.
    /// </summary>
    private static int NextID
    {
        get { return idGenerator++; }
    }
    #endregion

    // Maximum delay in seconds between the collection of two checkpoints until this car dies.
    private const float MAX_CHECKPOINT_DELAY = 7;

    /// <summary>
    /// The underlying AI agent of this car.
    /// </summary>
    public Agent Agent
    {
        get;
        set;
    }

    public float CurrentCompletionReward
    {
        get { return Agent.Genotype.Evaluation; }
        set { Agent.Genotype.Evaluation = value; }
    }

    /// <summary>
    /// Whether this car is controllable by user input (keyboard).
    /// </summary>
    public bool UseUserInput = false;

    /// <summary>
    /// The movement component of this car.
    /// </summary>
    public DroneMovement Movement
    {
        get;
        private set;
    }

    /// <summary>
    /// The current inputs for controlling the CarMovement component.
    /// </summary>
    public double[] CurrentControlInputs
    {
        get { return Movement.CurrentInputs; }
    }

    /// <summary>
    /// The cached SpriteRenderer of this car.
    /// </summary>
    public SpriteRenderer SpriteRenderer
    {
        get;
        private set;
    }

    private Sensors sensors;
    private float timeSinceLastCheckpoint;
    #endregion
    
    #region Constructors
    void Awake()
    {
        //Cache components
        Movement = GetComponent<DroneMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        sensors = GetComponent<Sensors>();
    }
    void Start()
    {
        Movement.HitWall += Die;

        //Set name to be unique
        this.name = "Drone (" + NextID + ")";
    }
    #endregion

    #region Methods
    /// <summary>
    /// Restarts this car, making it movable again.
    /// </summary>
    public void Restart()
    {
        Movement.enabled = true;
        timeSinceLastCheckpoint = 0;
        Agent.Reset();
        this.enabled = true;
    }

    // Unity method for normal update
    void Update()
    {
        timeSinceLastCheckpoint += Time.deltaTime;
    }

    // Unity method for physics update
    void FixedUpdate()
    {
        //Get control inputs from Agent
        if (!UseUserInput)
        {
            //Get readings from sensors
            if(Agent != null)
            {
                double[] sensorOutput = new double[9];
            
                sensorOutput[0] = sensors.getFrontSensorDistance();
                sensorOutput[1] = sensors.getFrontSensorDistanceDroite();
                sensorOutput[2] = sensors.getFrontSensorDistanceGauche();
                sensorOutput[3] = sensors.getUpSensorDistance();
                sensorOutput[4] = sensors.getUpSensorDistanceDroite();
                sensorOutput[5] = sensors.getUpSensorDistanceGauche();
                sensorOutput[6] = sensors.getDownSensorDistance();
                sensorOutput[7] = sensors.getDownSensorDistanceDroite();
                sensorOutput[8] = sensors.getDownSensorDistanceGauche();
                

                double[] controlInputs = Agent.FNN.ProcessInputs(sensorOutput);

                if (controlInputs.Length == 4)
                {
                    Movement.SetInputs(controlInputs);
                }
                else
                {
                    Debug.Log("Il doit y avoir 4 outputs dans le RDN");
                }

            }
        }

        if (timeSinceLastCheckpoint > MAX_CHECKPOINT_DELAY)
        {
            Die();
        }
    }

    // Makes this car die (making it unmovable and stops the Agent from calculating the controls for the car).
    private void Die()
    {
        this.enabled = false;
        Movement.Stop();
        Movement.enabled = false;

        Agent.Kill();
    }

    public void CheckpointCaptured()
    {
        timeSinceLastCheckpoint = 0;
    }
    #endregion
}
