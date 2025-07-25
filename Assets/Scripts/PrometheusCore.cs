using System;
using UnityEngine;

public class PrometheusCore : MonoBehaviour, IInteractable
{
    bool hasPrometheusCore;
    public static event EventHandler OnPrometheusCoreCollected;



    void Start()
    {
        FindAndSetPrometheusCorePosition();
        hasPrometheusCore = false;
    }



    void FindAndSetPrometheusCorePosition()
    {
        Collider collider = GetComponentInChildren<Collider>();
        Bounds bounds = collider.bounds;

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        GridPosition minGridPos = LevelGrid.Instance.GetGridPosition(min);
        GridPosition maxGridPos = LevelGrid.Instance.GetGridPosition(max);

        for (int x = minGridPos.x; x <= maxGridPos.x; x++)
        {
            for (int z = minGridPos.z; z <= maxGridPos.z; z++)
            {
                GridPosition gridPos = new GridPosition(x, z);
                if (!Pathfinding.Instance.GetIsWalkableGridPosInPath(gridPos)) //checks if an object is there
                {
                    LevelGrid.Instance.SetInteractableAtGridPosition(gridPos, this);
                }
            }
        }
    }
    public void Interact(Action onInteractComplete)
    {
        Light spotLight = GetComponentInChildren<Light>();
        if (spotLight != null && spotLight.type == LightType.Spot)
        {
            spotLight.enabled = false;
        }
        OnPrometheusCoreCollected?.Invoke(this, EventArgs.Empty);
        hasPrometheusCore = true;
    }
    public bool GetHasPrometheusCore()
    {
        return hasPrometheusCore;
    }
}
