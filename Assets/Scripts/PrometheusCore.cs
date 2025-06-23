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
        Transform coreBase = transform.Find("SM_Prop_Lander_01");

        if (coreBase == null)
        {
            Debug.LogError("PrometheusCore: Could not find core base transform.");
            return;
        }

        GridPosition baseGridPosition = LevelGrid.Instance.GetGridPosition(coreBase.position);

        for (int x = -1; x <= 0; x++) // 2x2 footprint: bottom-right aligned
        {
            for (int z = 0; z <= 1; z++)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition occupiedGridPos = baseGridPosition + offset;

                if (!LevelGrid.Instance.IsValidPosition(occupiedGridPos)) continue;

                LevelGrid.Instance.SetInteractableAtGridPosition(occupiedGridPos, this);
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
