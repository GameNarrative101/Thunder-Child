using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    Action<Vector3> onExplodedCallback; //ADDED
    Vector3 targetPosition;
    float totalDistance;
    Vector3 positionXZ;

    [SerializeField] Transform grenadeExplodeVfxPrefab;
    [SerializeField] TrailRenderer grenadeTrailRenderer;
    [SerializeField] AnimationCurve arcYAnimationCurve;

    public static event EventHandler OnAnyGrenadeExploded;



    void Update()
    {
        HandleProjectile();
    }



    private void HandleProjectile()
    {
        Vector3 MoveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 45f;
        positionXZ += MoveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.3f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            onExplodedCallback?.Invoke(targetPosition);
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            grenadeTrailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);
            // onGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action<Vector3> onExploded)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
        this.onExplodedCallback = onExploded;
    }
}
