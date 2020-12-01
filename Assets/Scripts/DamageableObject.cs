using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageableObject : MonoBehaviour
{
    // Start is called before the first frame update
    public float hp;
    public float maxHp;
    public int regenAmount;
    public UnityEvent onDeath = new UnityEvent();
    public UnityEvent onHit = new UnityEvent();

    public List<string> hitableTag;

    virtual public void takeDamage(int damage, GameObject origin)
    {
        Debug.Log(origin.tag);
        if (!hitableTag.Contains(origin.tag))
            return;
        hp -= damage;
		Debug.Log(name + "got hit with " + damage + " damage");
        if (hp <= 0)
        {
            hp = 0;
            Debug.Log(name + " is dead.");
            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject);
        } else if (onHit != null)
            onHit.Invoke();
    }
}
