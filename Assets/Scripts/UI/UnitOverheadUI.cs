using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitOverheadUI : MonoBehaviour
{
    [SerializeField] PCMech unit;
    [SerializeField] Slider heatBar;
    [SerializeField] Slider shieldBar;
    [SerializeField] Slider powerBar;
    [SerializeField] HealthSystem healthSystem;




    void Start()
    {
        healthSystem.OnShieldChanged += HealthSystem_OnShieldChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        PCMech.OnCorePowerChange += Unit_OnCorePowerChange;

        
        UpdateShieldBar();
        UpdatePowerBar();
        // UpdateHeatBar();
    }




    private void Unit_OnCorePowerChange(object sender, EventArgs e)
    {
        UpdatePowerBar();
    }


    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        healthSystem.OnShieldChanged -= HealthSystem_OnShieldChanged;
        healthSystem.OnDead -= HealthSystem_OnDead;
        PCMech.OnCorePowerChange -= Unit_OnCorePowerChange;
        
        Destroy(gameObject);
    }

    private void HealthSystem_OnShieldChanged(object sender, EventArgs e)
    {
        UpdateShieldBar();
    }

    void UpdateShieldBar()
    {
        if (unit != null && healthSystem != null)
        {
            shieldBar.value = healthSystem.GetShieldNormalized();
        }
    }
    
    private void UpdatePowerBar()
    {
        if (unit != null)
        {
            powerBar.value = unit.GetCorePowerNormalized();
        }    
    }
}
