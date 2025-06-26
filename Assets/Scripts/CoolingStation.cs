using System;
using UnityEngine;

public class CoolingStation : MonoBehaviour, IInteractable
{
    [SerializeField] PCMech pCMech;
 
 
 
    void Start()
    {
        FindAndSetCoolingStationPosition();
    }



    void FindAndSetCoolingStationPosition()
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
        pCMech.ResetHeat();
    }
}
