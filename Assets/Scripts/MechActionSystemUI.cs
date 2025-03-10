using UnityEngine;
using UnityEngine.UI;
using System;

public class MechActionSystemUI : MonoBehaviour
{

    [SerializeField] Transform actionButtonPrefab;

    //currently the container is dragged in from the scene, not as a prefab
    [SerializeField] Transform actionButtonContainerTransform;

    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        CreateMechActionButtons();
    }

    void CreateMechActionButtons ()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();

        foreach (BaseAction baseAction in selectedMech.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate (actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponentInChildren<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
        }
        
    }

    void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e)
    {
        CreateMechActionButtons();
    }
}
