using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction {x, y};

public class gravitySwitch : MonoBehaviour
{
    public enum DirectionList {x, y, nx, ny};
    public float speed = 30f;
    public DirectionList targetGravityDirection;
    public DirectionList currentGravityDirection;
    Vector3 currentGravity;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Physics.gravity = new Vector3((targetGravityDirection == DirectionList.x ? speed: 0), (targetGravityDirection == DirectionList.y ? speed : 0), 0);
            if (targetGravityDirection == DirectionList.y) {
                other.gameObject.GetComponent<PlayerMovement>().rotatePlayer(180);
                Physics.gravity = new Vector3(0, speed, 0);
            }
            if (targetGravityDirection == DirectionList.ny) {
                other.gameObject.GetComponent<PlayerMovement>().rotatePlayer(0);
                Physics.gravity = new Vector3(0, -speed, 0);
            }
            if (targetGravityDirection == DirectionList.x) {
                other.gameObject.GetComponent<PlayerMovement>().rotatePlayer(90);
                Physics.gravity = new Vector3(speed, 0, 0);
            }
            if (targetGravityDirection == DirectionList.nx) {
                Physics.gravity = new Vector3(-speed, 0, 0);
                other.gameObject.GetComponent<PlayerMovement>().rotatePlayer(270);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        currentGravity = Physics.gravity;
    }
}
