using UnityEngine;

public class RagdollOperator : MonoBehaviour
{
    [SerializeField] Transform ragdollRootBone;



    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms (originalRootBone, ragdollRootBone);

        //variations on this impact could be introduced based on different damaging abilities used
        ApplyExplosionToRagdoll (ragdollRootBone, 500f, transform.position, 10f);
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
        /* 
            to make the ragdoll not just flop, but react to the bullet force
            called here, with each damaging ability modifying the explosion force
        */
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
