using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitOverheadUI : MonoBehaviour
{
    [SerializeField] PCMech unit;
    [SerializeField] Slider heatBar;
    [SerializeField] Slider shieldBar;
    [SerializeField] Slider powerBar;
    [SerializeField] TextMeshProUGUI corePowerText;
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] TextMeshProUGUI shieldText;

    HealthSystem healthSystem;



    void Start()
    {
        SetUIAndSubscriptions();
    }



    //==================================================================================================================================== 
    #region SETUP AND SUBSCRIPTIONS

    void SetUIAndSubscriptions()
    {
        healthSystem = unit.GetComponent<HealthSystem>();

        healthSystem.OnShieldChanged += HealthSystem_OnShieldChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        PCMech.OnCorePowerChange += Unit_OnCorePowerChange;
        PCMech.OnHeatChange += Unit_OnHeatChange;
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;

        UpdateShieldBar();
        UpdatePowerBar();
        UpdateHeatBar();
        UpdateCorePowerText();
        UpdateHeatText();
        UpdateShieldText();
    }
    void UnitActionSystem_OnSelectedUnitChange(object sender, EventArgs e)
    {
        UpdateCorePowerText();
        UpdatePowerBar();
        UpdateHeatText();
        UpdateHeatBar();
        UpdateShieldText();
        UpdateShieldBar();
    }
    void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateCorePowerText();
        UpdatePowerBar();
        UpdateHeatText();
        UpdateHeatBar();
    }
    void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        UpdateCorePowerText();
        UpdatePowerBar();
        UpdateHeatText();
        UpdateHeatBar();
    }
    void HealthSystem_OnDead(object sender, EventArgs e)
    {
        healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;
        PCMech.OnCorePowerChange -= Unit_OnCorePowerChange;
        PCMech.OnHeatChange -= Unit_OnHeatChange;
        UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        TurnSystemScript.Instance.OnTurnEnd -= TurnSystemScript_OnTurnEnd;
        healthSystem.OnDead -= HealthSystem_OnDead;

        Destroy(gameObject);
    }
    void Unit_OnCorePowerChange(object sender, EventArgs e)
    {
        UpdatePowerBar();
        UpdateCorePowerText();
    }
    void Unit_OnHeatChange(object sender, EventArgs e)
    {
        UpdateHeatBar();
        UpdateHeatText();
    }
    void HealthSystem_OnShieldChanged(object sender, EventArgs e)
    {
        UpdateShieldBar();
        UpdateShieldText();
    }

    #endregion



    //==================================================================================================================================== 
    #region UI UPDATES

    void UpdateShieldBar()
    {
        if (unit != null && healthSystem != null) { shieldBar.value = healthSystem.GetShieldNormalized(); }
    }
    void UpdatePowerBar()
    {
        if (unit != null) { powerBar.value = unit.GetCorePowerNormalized(); }
    }
    void UpdateHeatBar()
    {
        if (unit != null) { heatBar.value = unit.GetHeatNormalized(); }
    }
    void UpdateCorePowerText()
    {
        // PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();
        corePowerText.text = unit.GetCorePower().ToString();
    }
    void UpdateHeatText()
    {
        heatText.text = unit.GetHeat().ToString();
    }
    void UpdateShieldText()
    {
        shieldText.text = healthSystem.GetShield().ToString();
    }
    
    #endregion
}
