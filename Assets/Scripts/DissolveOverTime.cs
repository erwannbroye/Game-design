using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
public class DissolveOverTime : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public bool disolve;
    public float speed = 2;

    public UnityEvent onDisovle = new UnityEvent();
    public UnityEvent onSpawn = new UnityEvent();

    private bool canCallSpawnEvent = false;
    private bool canCallDisolveEvent = false;

    private void Start(){
        meshRenderer = this.GetComponent<MeshRenderer>();
        spawnObject();
    }

    public void disolveObject() {
        canCallDisolveEvent = true;
        disolve = true;
        meshRenderer.material.SetFloat("_Cutoff", 0);
    }

    public void spawnObject() {
        canCallSpawnEvent = true;
        disolve = false;
        meshRenderer.material.SetFloat("_Cutoff", 1);
    }

    private void Update(){
        Material[] mats = meshRenderer.materials;

        // mats[0].SetFloat("_Cutoff", Mathf.Sin(t * speed));
        if (disolve && mats[0].GetFloat("_Cutoff") <=  1)
            mats[0].SetFloat("_Cutoff", mats[0].GetFloat("_Cutoff") + (Time.deltaTime * speed));
        if (!disolve && mats[0].GetFloat("_Cutoff") >=  0)
            mats[0].SetFloat("_Cutoff", mats[0].GetFloat("_Cutoff") + (Time.deltaTime * -speed));
        if (onDisovle != null && canCallDisolveEvent && mats[0].GetFloat("_Cutoff") >= 1) {
            onDisovle.Invoke();
            canCallDisolveEvent = false;
        }
        if (onSpawn != null && canCallSpawnEvent && mats[0].GetFloat("_Cutoff") <= 0) {
            onSpawn.Invoke();
            canCallSpawnEvent = false;
        }
        
        meshRenderer.materials = mats;
    }
}
