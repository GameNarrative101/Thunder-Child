using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform bulletHitVFXPrefab;
    Vector3 targetPosition;

    public void Setup (Vector3 targetPosition)
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

        //at high speeds, the object can bug out and go back and forth, so we check for that and then destroy
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            //preventing any overshot for an object this fast
            transform.position=targetPosition;

            //to keep the trail there a bit longer after the bullet hits, we unparent and autodestruct it
            trailRenderer.transform.parent = null;

            Destroy (gameObject);

            Instantiate (bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
    }
}
