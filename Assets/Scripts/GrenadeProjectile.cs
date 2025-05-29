using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    Action onGrenadeBehaviourComplete;
    Vector3 targetPosition;
    [SerializeField] float damageRadius = 8f;
    [SerializeField] int grenadeDamage = 10;

    void Update()
    {
        Vector3 MoveDir = (targetPosition - transform.position).normalized;
        float moveSpeed = 25f;
        transform.position += MoveDir * moveSpeed * Time.deltaTime;

        float reachedTargetDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out PCMech pcMech))
                {
                    pcMech.TakeDamage(grenadeDamage);
                }
            }

            Destroy(gameObject);
            onGrenadeBehaviourComplete();
        }
        
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;

        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }
}
