using UnityEngine;

public class RagdollOperator : MonoBehaviour
{
    [SerializeField] Transform ragdollRootBone;



    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms (originalRootBone, ragdollRootBone);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ApplyExplosionToRagdoll (ragdollRootBone, 500f, transform.position + randomDir, 10f);
    }    
    void MatchAllChildTransforms (Transform root, Transform clone)
    {
        /*     
            abstract recursive function that takes the root reference and cycles through all children, matching the ragdoll with the unit.
            then, it calls itself and runs through the second layer of children and so on until all children are matched.
            ragdoll then spawns at the exact position of the unit upon death.
        */
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }
    public void ApplyExplosionToRagdoll (Transform root, float explosionForce, 
    UnityEngine.Vector3 explosionPosition, float explosionRadius)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRadius);
        }
    }
}
