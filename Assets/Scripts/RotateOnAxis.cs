using UnityEngine;
using System.Collections;
 
public class RotateOnAxis : MonoBehaviour
{
     public float speed;                  //Set in Inspector 
     private Quaternion tempPos;
    public enum Direction {x, y, z};
    public Direction myDirection;
     
     void Start () 
     {

     }
 
     void Update () 
     {        
         transform.RotateAround(transform.position, (myDirection == Direction.y ? transform.up : (myDirection == Direction.z ? transform.forward : transform.right)), Time.deltaTime * speed);
     }
}