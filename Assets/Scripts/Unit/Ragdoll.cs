using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform _ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, _ragdollRootBone);
        var randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ApplyExplosionToRagdoll(originalRootBone, 300f, transform.position + randomDir, 10f);
    }
    private void MatchAllChildTransforms(Transform originalRootBone, Transform ragdollRootBone)
    {
        foreach (Transform childBone in originalRootBone)
        {
            Transform ragdollChildBone = ragdollRootBone.Find(childBone.name);
            if (ragdollChildBone != null)
            {
                ragdollChildBone.position = childBone.position;
                ragdollChildBone.rotation = childBone.rotation;

                MatchAllChildTransforms(childBone, ragdollChildBone);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
