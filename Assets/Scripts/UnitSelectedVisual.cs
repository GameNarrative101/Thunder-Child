using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] Mech pcMech;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();    
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UpdateVisual();
    }

    void UnitActionSystem_OnSelectedUnitChange (object sender, EventArgs empty)
    {
        UpdateVisual();

    }

    private void UpdateVisual()
    {
        if (UnitActionSystem.Instance.GetSelectedMech() == pcMech)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void ODestroy()
    {
        //otherwise it would stay subscribed and would keep trying to get the mesh renderer, getting a null result
        UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
    }
}
