using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{
    public GameObject firepoint;
    public List<GameObject> vfx = new List<GameObject>();
    private GameObject effectToSpawn;
    public int bulletSpeed;
    public int bulletDamage;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void spawnBullet() {
        GameObject bullet;
        if (firepoint) {
            bullet = Instantiate(effectToSpawn, firepoint.transform.position, firepoint.transform.rotation);
            bullet.GetComponent<BulletPlayer>().damage = bulletDamage;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            bullet.tag = gameObject.tag;
            rb.AddForce(firepoint.transform.forward * bulletSpeed, ForceMode.Impulse);
            Destroy(bullet, 5f);
        }
    }
}