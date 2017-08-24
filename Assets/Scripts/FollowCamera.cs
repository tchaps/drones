using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    public GameObject Target;
    public float damping = 1;
    Vector3 offset;

    void Start()
    {
        //target = GameObject.Find("/Tracks/Track/Drone2");
        offset = Target.transform.position - transform.position;
    }

    void LateUpdate()
    {
        if(Target != null)
        {
            float currentAngle = transform.eulerAngles.y;

            float desiredAngle = Target.transform.eulerAngles.y;
            float angle = Mathf.LerpAngle(currentAngle, desiredAngle, damping);

            Quaternion rotation = Quaternion.Euler(0, angle, 0) * Quaternion.Euler(0, 60, 0);
            transform.position = Target.transform.position - (rotation * offset);

            transform.LookAt(Target.transform);
            }
    }

    #region methods 
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
        //offset = this.Target.transform.position - transform.position;
    }

#endregion
}