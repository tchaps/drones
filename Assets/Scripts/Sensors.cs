using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour {
    private float sensorLength = 5f; // distance max que le senseur peut détecter

    private float frontSensorStartPoint = 0.28f;
    private float upSensorStartPoint = 0.05f;
    private float downSensorStartPoint = 0.05f;

    private float angle = 60f;

    public bool debug = false;

    // Bit shift the index of the layer (9) to get a bit mask
    private int layerMask = 1 << 9;

    //private List<Bounds> obstaclesBounds;
    /*
    void Start()
    { 
        obstaclesBounds = new List<Bounds>();
        
        obstaclesBounds.Add(GameObject.Find("CenterWalls").GetComponent<Renderer>().bounds);
        obstaclesBounds.Add(GameObject.Find("VerticalObstacle").GetComponent<Renderer>().bounds);
        obstaclesBounds.Add(GameObject.Find("HorizontalObstacle").GetComponent<Renderer>().bounds);
        obstaclesBounds.Add(GameObject.Find("VertHorizObstacle").GetComponent<Renderer>().bounds);

        Debug.Log(obstaclesBounds);
       
    }
    */

    // Update is called once per frame
    void Update () {
        if (debug)
        {
            //Debug.Log("devant face : " + getFrontSensorDistance(0));
            //Debug.Log("devant droite : " + getFrontSensorDistance(angle));
            //Debug.Log("devant gauche : " + getFrontSensorDistance(-angle));

            //Debug.Log("bas face : " + getDownSensorDistance(0));
            //Debug.Log("bas droite : " + getDownSensorDistance(angle));
            //Debug.Log("bas gauche : " + getDownSensorDistance(-angle));

            //Debug.Log("haut face : " + getUpSensorDistance(0));
            //Debug.Log("haut droite : " + getUpSensorDistance(angle));
            //Debug.Log("haut gauche : " + getUpSensorDistance(-angle));
        }
    }

    private void Awake()
    {
        layerMask = ~layerMask;
    }

    public float getFrontSensorDistance()
    {
        return getFrontSensorDistance(0);
    }
    public float getFrontSensorDistanceDroite()
    {
        return getFrontSensorDistance(angle);
    }
    public float getFrontSensorDistanceGauche()
    {
        return getFrontSensorDistance(-angle);
    }
    public float getDownSensorDistance()
    {
        return getDownSensorDistance(0);
    }
    public float getDownSensorDistanceDroite()
    {
        return getDownSensorDistance(angle);
    }
    public float getDownSensorDistanceGauche()
    {
        return getDownSensorDistance(-angle);
    }
    public float getUpSensorDistance()
    {
        return getUpSensorDistance(0);
    }
    public float getUpSensorDistanceDroite()
    {
        return getUpSensorDistance(angle);
    }
    public float getUpSensorDistanceGauche()
    {
        return getUpSensorDistance(-angle);
    }

    float getDownSensorDistance(float _angle)
    {
        Vector3 pos = transform.position - transform.up * downSensorStartPoint;
        RaycastHit hit;
        float distance = 0.0f;

        if (Physics.Raycast(pos, Quaternion.AngleAxis(_angle, transform.forward) * -transform.up, out hit, sensorLength, layerMask))
        {
            Debug.DrawLine(pos, hit.point, Color.white);
            distance = Vector3.Distance(hit.point, transform.position);
        } else
        {
            distance = sensorLength;
        }
        return distance;
    }

    float getFrontSensorDistance(float _angle)
    {
        Vector3 pos = transform.position + transform.forward * frontSensorStartPoint;
        RaycastHit hit;
        float distance = 0.0f;

        if (Physics.Raycast(pos, Quaternion.AngleAxis(_angle, transform.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(pos, hit.point, Color.white);
            distance = Vector3.Distance(hit.point, transform.position);
        } else
        {
            distance = sensorLength;
        }
        return distance;
    }

    float getUpSensorDistance(float _angle)
    {
        Vector3 pos = transform.position + transform.up * upSensorStartPoint;
        RaycastHit hit;
        float distance = 0.0f;

        if (Physics.Raycast(pos, Quaternion.AngleAxis(_angle, transform.forward) * transform.up, out hit, sensorLength))
        {
            Debug.DrawLine(pos, hit.point, Color.white);
            distance = Vector3.Distance(hit.point, transform.position);
        } else
        {
            distance = sensorLength;
        }
        return distance;
    }
    /*
    bool isInObstacle()
    {
        foreach (Bounds bounds in obstaclesBounds)
        {
            if (bounds.Contains(transform.position))
                return true;
        }
        return false;
    }
    */

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("BOOM");
        //Time.timeScale = 0; // freeze le jeu
        //Application.LoadLevel(Application.loadedLevel); // relance le jeu
    }

}
