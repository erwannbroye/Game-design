using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPad : MonoBehaviour
{

    public float jumpForce;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            other.gameObject.GetComponent<Rigidbody>().AddForce(other.gameObject.transform.up * jumpForce * 10);
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
