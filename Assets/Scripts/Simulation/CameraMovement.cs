/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
#endregion


/// <summary>
/// Component for simple camera target following.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    #region Members
    // Distance of Camera to target in Z direction, to be set in Unity Editor.
    [SerializeField]
    private int CamZ = -10;
    // The initial Camera target, to be set in Unity Editor.
    [SerializeField]
    private GameObject Target;
    // The speed of camera movement, to be set in Unity Editor.
    [SerializeField]
    private float CamSpeed = 5f;
    // The speed of camera movement when reacting to user input, to be set in Unity Editor.
    [SerializeField]
    private float UserInputSpeed = 50f;
    // Whether the camera can be controlled by user input, to be set in Unity Editor.
    [SerializeField]
    private bool AllowUserInput;

    /// <summary>
    /// The bounds the camera may move in.
    /// </summary>
    public RectTransform MovementBounds
    {
        get;
        set;
    }

    private Vector3 targetCamPos;

    public float damping = 1;
    Vector3 offset;
    #endregion


    #region Methods
    /// <summary>
    /// Sets the target to follow.
    /// </summary>
    /// <param name="target">The target to follow.</param>
    public void SetTarget(GameObject target)
    {
        //Set position instantly if previous target was null
        //if (Target == null && !AllowUserInput && target != null)
        //    SetCamPosInstant(target.transform.position);

        this.Target = target;
        offset = this.Target.transform.position - transform.position;
    }



      void LateUpdate()
    {
        if(Target != null)
        {

            float currentAngle = transform.eulerAngles.y;
            float desiredAngle = this.Target.transform.eulerAngles.y;
            float angle = Mathf.LerpAngle(currentAngle, desiredAngle, damping);

            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.position = this.Target.transform.position - (rotation * offset);

            transform.LookAt(this.Target.transform);
        }
    }

    ///// <summary>
    ///// Instantly sets the camera position to the given position, without interpolation.
    ///// </summary>
    ///// <param name="camPos">The position to set the camera to.</param>
    //public void SetCamPosInstant(Vector3 camPos)
    //{
    //    camPos.z = CamZ;
    //    this.transform.position = camPos;
    //    targetCamPos = this.transform.position;
    //}

    //private void CheckUserInput()
    //{
    //    float horizontalInput, verticalInput;

    //    horizontalInput = Input.GetAxis("Horizontal");
    //    verticalInput = Input.GetAxis("Vertical");

    //    targetCamPos += new Vector3(horizontalInput * UserInputSpeed * Time.deltaTime, verticalInput * UserInputSpeed * Time.deltaTime);
    //}
    #endregion
}
