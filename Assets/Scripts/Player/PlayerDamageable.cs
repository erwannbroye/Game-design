using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : DamageableObject
{
    // Start is called before the first frame update

    public int deathHeight;
    
    void Update() {
        nextRegen += Time.deltaTime;
        if (nextRegen >= 1) {
            nextRegen = 0;
            hp = (hp + regenAmount > maxHp ? maxHp : hp + regenAmount);
        }
        if (transform.position.y < deathHeight && onDeath != null)
            onDeath.Invoke();
    }

    override public void takeDamage(int damage, GameObject origin)
    {
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

        } else if (onHit != null)
            onHit.Invoke();
    }
}
