/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using System.Collections;
#endregion

/// <summary>
/// Component for car movement and collision detection.
/// </summary>
public class DroneMovement : MonoBehaviour
{
    #region Members
    /// <summary>
    /// Event for when the car hit a wall.
    /// </summary>
    public event System.Action HitWall;

    //Movement constants
    /*private const float MAX_VEL = 20f;
    private const float ACCELERATION = 8f;
    private const float VEL_FRICT = 2f;*/
    private const float TURN_SPEED = 100;

    private DroneController controller;

    /// <summary>
    /// The current velocity of the car.
    /// </summary>
    public float Velocity
    {
        get;
        private set;
    }

    /// <summary>
    /// The current rotation of the car.
    /// </summary>
    public Quaternion Rotation
    {
        get;
        private set;
    }

    //private double horizontalInput, frontSpeedInput, verticalInput; //Horizontal = engine force, Vertical = turning force
    private double leftInput, rightInput, downInput, upInput;
        /// <summary>
    /// The current inputs for turning and engine force in this order.
    /// </summary>
    public double[] CurrentInputs
    {
        //get { return new double[] { horizontalInput, frontSpeedInput, verticalInput }; }
        get { return new double[] { leftInput, rightInput, downInput, upInput }; }
    }
    #endregion

    #region Constructors
    void Start()
    {
        controller = GetComponent<DroneController>();

        // 
        Velocity = 3f;
    }
    #endregion

    #region Methods
    // Unity method for physics updates
    void FixedUpdate ()
    {

        ApplyInput();

        ApplyVelocity();

        //ApplyFriction();
	}

    // Applies the currently set input
    private void ApplyInput()
    {
        //Cap input 
        /*if (frontSpeedInput > 1)
            frontSpeedInput = 1;
        else if (frontSpeedInput < -1)
            frontSpeedInput = -1;

        if (horizontalInput > 1)
            horizontalInput = 1;
        else if (horizontalInput < -1)
            horizontalInput = -1;

        if (verticalInput > 1)
            verticalInput = 1;
        else if (verticalInput < -1)
            verticalInput = -1;*/


        //Car can only accelerate further if velocity is lower than engineForce * MAX_VEL
        /*bool canAccelerate = false;
        if (frontSpeedInput < 0)
            canAccelerate = Velocity > frontSpeedInput * MAX_VEL;
        else if (frontSpeedInput > 0)
            canAccelerate = Velocity < frontSpeedInput * MAX_VEL;

        //Set velocity
        if (canAccelerate)
        {
            Velocity += (float)frontSpeedInput * ACCELERATION * Time.deltaTime;

            //Cap velocity
            if (Velocity > MAX_VEL)
                Velocity = MAX_VEL;
            else if (Velocity < -MAX_VEL)
                Velocity = -MAX_VEL;
        }*/



        //Set rotation
        Rotation = transform.rotation;
        float maxAngle = TURN_SPEED * Time.deltaTime;
        float Hangle = (float)(leftInput-rightInput) * maxAngle * 2 - maxAngle;
        float Vangle = (float)(downInput-upInput) * maxAngle * 2 - maxAngle;
        //Debug.Log("hangle = " + angle);
        Quaternion newHRot = Quaternion.AngleAxis(Hangle, Vector3.up);
        Quaternion newVRot = Quaternion.AngleAxis(Vangle, Vector3.left);

        Rotation *= newHRot * newVRot;

    }
    

    /// <summary>
    /// Sets the engine and turning input according to the given values.
    /// </summary>
    /// <param name="input">The inputs for turning and engine force in this order.</param>
    public void SetInputs(double[] input)
    {
        //horizontalInput = input[0];
        //frontSpeedInput = input[1];
        //verticalInput = input[2];
        leftInput = input[0];
        rightInput = input[1];
        downInput = input[2];
        upInput = input[3];
    }

    // Applies the current velocity to the position of the car.
    private void ApplyVelocity()
    {
        //Vector3 direction = new Vector3(0, 1, 0);
        Vector3 direction = Vector3.forward;
        transform.rotation = Rotation;
        direction = Rotation * direction;

        this.transform.position += direction * Velocity * Time.deltaTime;
    }

    // Applies some friction to velocity
    /*private void ApplyFriction()
    {
        if (frontSpeedInput == 0)
        {
            if (Velocity > 0)
            {
                Velocity -= VEL_FRICT * Time.deltaTime;
                if (Velocity < 0)
                    Velocity = 0;
            }
            else if (Velocity < 0)
            {
                Velocity += VEL_FRICT * Time.deltaTime;
                if (Velocity > 0)
                    Velocity = 0;            
            }
        }
    }*/

    // Unity method, triggered when collision was detected.
    void OnCollisionEnter()
    {
        if (HitWall != null)
            HitWall();
    }

    /// <summary>
    /// Stops all current movement of the car.
    /// </summary>
    public void Stop()
    {
        Velocity = 0;
        Rotation = Quaternion.AngleAxis(0, Vector3.up);
        Rotation = Quaternion.AngleAxis(0, Vector3.left);
    }
    #endregion
}