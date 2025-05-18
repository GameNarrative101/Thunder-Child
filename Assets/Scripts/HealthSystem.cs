using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxShield = 70;
    [SerializeField] int shield;
    // bool isDead = false;

    public event EventHandler OnDead;
    public event EventHandler OnShieldChanged;



    private void Awake()
    {
        shield = maxShield;
    }



    public void Damage (int damageAmount)
    {
        shield -= damageAmount;
        OnShieldChanged?.Invoke(this, EventArgs.Empty);

        if (shield < 0)
        {
            shield = 0;
        }

        if (shield == 0)
        {
            // isDead=true;
            Die();
        }

        Debug.Log (shield);
    }
    //what actually happens upon unit death is not defined here so this script can be flexible.
    void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }
    public float GetShieldNormalized()
    {
        return shield / (float)maxShield;
    }
    public int GetShield()
    {
        return shield;
    }
    //     public bool IsDead()
    // {
    //     return isDead;
    // }
}
