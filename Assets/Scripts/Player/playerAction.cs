using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAction : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public SpawnProjectile spawnProjectileMethod;
    public float fireRate = 1.0f;
	private float fireTime = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fireTime -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && fireTime <= 0 && animator.GetBool("attacking") == false) {
            animator.Play("robot|Attack", -1, 0f);
            animator.SetBool("attacking", true);
            fireTime = fireRate;
        }
    }

    public void fire() {
        spawnProjectileMethod.spawnBullet();
    }

    public void nextFire() {
        animator.SetBool("attacking", false);
    }

    public void stopSlideAnim() {
        animator.SetInteger("SlideState", 0);
    }
}
