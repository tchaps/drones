using UnityEngine;
using System.Collections;

public class MoveTowards : MonoBehaviour
{
    public int X;
    public int Y;
    public int Z;
    
    public float rotationSpeed;
    public float moveSpeed;

    private Vector3 nextPosition;

    private void Start()
    {
        nextPosition = new Vector3(X, Y, Z);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {

            Vector3 targetDir = nextPosition - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed, 0.0F);
            Debug.DrawRay(transform.position, newDir, Color.red);
            transform.rotation = Quaternion.LookRotation(newDir);

            transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed);
        }
    }
}
