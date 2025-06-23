using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxShield = 70;
    [SerializeField] int shield;
    [SerializeField] GameObject ExpeditionFailed;
    [SerializeField] PCMech pcMech;
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

        Debug.Log ("Remaining shield =" + shield);
    }
    void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);

        if (!pcMech.IsEnemy())
        {
            ExpeditionFailed.SetActive(true);
        }
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
