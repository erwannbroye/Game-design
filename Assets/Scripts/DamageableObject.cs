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
    public List<string> hitableTag;
    public bool alwaysRemoveColliderOnDeath;
    public float nextRegen = 0;
    public UnityEvent onDeath = new UnityEvent();
    public UnityEvent onHit = new UnityEvent();



    void Update() {
        nextRegen += Time.deltaTime;
        if (nextRegen >= 1) {
            nextRegen = 0;
            hp = (hp + regenAmount > maxHp ? maxHp : hp + regenAmount);
        }
    }

    public void destroyOndeath() {
        Destroy(gameObject);
    }

    virtual public void takeDamage(int damage, GameObject origin)
    {
        if (!hitableTag.Contains(origin.tag))
            return;
        hp -= damage;
		Debug.Log(name + "got hit with " + damage + " damage");
        if (hp <= 0)
        {
            hp = 0;
            Debug.Log(name + " is dead.");
            if (alwaysRemoveColliderOnDeath)
                foreach(Collider c in GetComponents<Collider> ())
                    c.enabled = false;
        
            if (onDeath != null)
                onDeath.Invoke();
            else
                destroyOndeath();

        } else if (onHit != null)
            onHit.Invoke();
    }
}
