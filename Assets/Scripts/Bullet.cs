using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    public int damage = 5;

    void OnCollisionEnter(Collision col) {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (col.gameObject.GetComponent<DamageableObject>())
        {
            col.gameObject.GetComponent<DamageableObject>().takeDamage(damage, gameObject);
        }
        Destroy(effect, 5f);
        Destroy(gameObject);
    }
}
