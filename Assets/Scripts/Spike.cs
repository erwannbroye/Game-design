using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other) {
        Debug.Log("damage");
        if (other.collider.tag == "Player") {
            other.gameObject.GetComponent<PlayerDamageable>().takeDamage(1000, gameObject);
        }
    }
}
