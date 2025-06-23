using System;
using UnityEngine;

public class Extraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject expeditionSuccessful;
    private GridPosition gridPosition;
    bool isGreen;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        SetColorRed();
    }
    void SetColorGreen()
    {
        isGreen = true;
        if (meshRenderer != null && greenMaterial != null)
        {
            meshRenderer.material = greenMaterial;
        }
    }
    void SetColorRed()
    {
        isGreen = false;
        if (meshRenderer != null && redMaterial != null)
        {
            meshRenderer.material = redMaterial;
        }
    }
    public void Interact(Action onInteractComplete)
    {
        PrometheusCore prometheusCore = FindFirstObjectByType<PrometheusCore>();
        if (!prometheusCore.GetHasPrometheusCore()) {return;}

        if (isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen(); //only option. turns red for testing purposes
            expeditionSuccessful.SetActive(true);
        }
    }
}
