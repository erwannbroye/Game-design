using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    // Start is called before the first frame update

    public Rigidbody parentRigidbody;
    public float timer;
    float currentTime;
    bool willFall;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
            willFall = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (willFall) {
            currentTime += Time.deltaTime;
            if (currentTime >= timer)
                parentRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;
        }
    }
}
