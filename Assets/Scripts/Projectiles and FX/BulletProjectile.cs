using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform bulletHitVFXPrefab;
    Vector3 targetPosition;



    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    void Update()
    {
        Vector3 moveDir = (targetPosition-transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        
        float moveSpeed = 200f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            //preventing any overshot for an object this fast
            transform.position=targetPosition;

            //to keep the trail there a bit longer after the bullet hits, unparent and autodestruct it
            trailRenderer.transform.parent = null;

            Destroy (gameObject);

            Instantiate (bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }
}
