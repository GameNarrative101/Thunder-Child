using System;
using UnityEngine;

public class RagdollSpawner : MonoBehaviour
{
    [SerializeField] Transform ragdollPrefab;
    [SerializeField] Transform originalRootBone;

    HealthSystem healthSystem;


    
    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += HealthSystem_OnDead;
    }



    void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate (ragdollPrefab, transform.position, transform.rotation);
        RagdollOperator ragdollOperator = ragdollTransform.GetComponent<RagdollOperator>();
        ragdollOperator.Setup(originalRootBone);        
    }
}
