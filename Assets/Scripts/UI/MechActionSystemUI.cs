using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MechActionSystemUI : MonoBehaviour
{

    [SerializeField] Transform actionButtonPrefab;

    //currently the container is dragged in from the scene, not as a prefab
    [SerializeField] Transform actionButtonContainerTransform;
    [SerializeField] TextMeshProUGUI CorePowerText;



    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystemScript.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        PCMech.OnAnyCorePowerChange += PCMech_OnAnyCorePowerChange;


        CreateMechActionButtons();
        UpdateCorePower();
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
        UpdateCorePower();
    }

    void UnitActionSystem_OnActionStarted(object sender, System.EventArgs e)
    {
        CreateMechActionButtons();
        UpdateCorePower();
    }

    void UpdateCorePower()
    {
        PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();
        CorePowerText.text = selectedMech.GetCorePower().ToString();
    }

    void TurnSystem_OnTurnChange(object sender, System.EventArgs e)
    {
        UpdateCorePower();
    }

    void PCMech_OnAnyCorePowerChange(object sender, System.EventArgs e)
    {
        UpdateCorePower();
    }
}
