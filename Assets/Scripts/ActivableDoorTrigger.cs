using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableDoorTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public ActivableDoor linkedDoor;
    bool hasBeenTrigger;

    private void OnCollisionEnter(Collision other) {
        if (!hasBeenTrigger && other.collider.tag == "Player_bullet") {
            linkedDoor.removeOneLock();
            hasBeenTrigger = true;
        }
    }
}
