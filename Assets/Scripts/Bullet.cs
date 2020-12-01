<<<<<<< HEAD
﻿using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;

    public GameObject impactEffect;

    public void Seek (Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);
        //Destroy(target.gameObject);
=======
﻿using System.Collections;
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
            Debug.Log("hit " + col.gameObject.name);
            col.gameObject.GetComponent<DamageableObject>().takeDamage(damage, gameObject);
        }
        Destroy(effect, 5f);
>>>>>>> 8009c49b7bea6b6b52dcf1ffa23a909e67c5e5c4
        Destroy(gameObject);
    }
}
