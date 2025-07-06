using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int maxPCShield = 70;
    [SerializeField] int maxEnemyShield = 100;
    [SerializeField] GameObject ExpeditionFailed;
    [SerializeField] PCMech pcMech;
    // bool isDead = false;

    int shield;
    public event EventHandler OnDead;
    public event EventHandler OnShieldChanged;



    private void Awake()
    {
        SetMaxShield();
    }

    private void SetMaxShield()
    {
        if (pcMech.GetIsEnemy())
        {
            shield = maxEnemyShield;
        }
        else
        {
            shield = maxPCShield;
        }
    }

    public void Damage(int damageAmount)
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

        Debug.Log("Remaining shield =" + shield);
    }
    void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);

        if (!pcMech.GetIsEnemy())
        {
            ExpeditionFailed.SetActive(true);
        }
    }
    public float GetShieldNormalized()
    {
        if (pcMech.GetIsEnemy())
        {
            return shield / (float)maxEnemyShield;
        }
        else
        {
            return shield / (float)maxPCShield;
        }
    }
    public int GetShield()
    {
        return shield;
    }
    public void Heal(int healAmount)
    {
        shield += healAmount;

        if (!pcMech.GetIsEnemy() && shield > maxPCShield)
        {
            shield = maxPCShield;
        }

        OnShieldChanged?.Invoke(this, EventArgs.Empty);
    }
    //     public bool IsDead()
    // {
    //     return isDead;
    // }
}
