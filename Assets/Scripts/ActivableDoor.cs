using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableDoor : MonoBehaviour
{
    // Start is called before the first frame update
    public int numberOfLocks;
    void Start()
    {
        
    }

    public void removeOneLock() {
        numberOfLocks -= 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (numberOfLocks == 0) {
            foreach(Collider c in GetComponents<Collider> ())
                    c.enabled = false;
            gameObject.GetComponent<DissolveOverTime>().disolveObject();
            numberOfLocks = -1;
        }
    }
}
