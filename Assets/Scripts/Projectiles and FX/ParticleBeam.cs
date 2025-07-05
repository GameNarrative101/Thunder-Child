using UnityEngine;

public class ParticleBeam : MonoBehaviour
{
    PCMech pcMech;
    [SerializeField] LineRenderer lineRenderer;
    Vector3 targetPosition;


    void ShootBeam()
    {
        lineRenderer.useWorldSpace = true;

        lineRenderer.SetPosition(0, pcMech.GetWorldPosition());
        lineRenderer.SetPosition(1, targetPosition);
    }

    public void TryShootBeam(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        ShootBeam();
    }
    public void SetShooter(PCMech shooter)
    {
        this.pcMech = shooter;
    }
}
