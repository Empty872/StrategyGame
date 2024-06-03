using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone;


    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDeath += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        var ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        var ragdoll = ragdollTransform.GetComponent<Ragdoll>();
        ragdoll.Setup(originalRootBone);
    }


}
