using UnityEngine;
using System.Collections;
 
public class MovingObjectLoop : MonoBehaviour
{
    public float amplitude;
    public float speed;
    private Vector3 tempVal;
    private Vector3 tempPos;
    int selected = 0;
    public enum Direction {x, y, z};
    
    public Direction myDirection;

    void Start () 
    {
        if (myDirection == Direction.y)
            tempVal.y = transform.position.y;
        if (myDirection == Direction.x)
            tempVal.x = transform.position.x;
        if (myDirection == Direction.z)
            tempVal.z = transform.position.z;
        tempPos = transform.position;
    }

    void Update () 
    {        
        if (myDirection == Direction.y)
            tempPos.y = tempVal.y + amplitude * Mathf.Sin(speed * Time.time);
        if (myDirection == Direction.x)
            tempPos.x = tempVal.x + amplitude * Mathf.Sin(speed * Time.time);
        if (myDirection == Direction.z)
            tempPos.z = tempVal.z + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = tempPos;
    }
}