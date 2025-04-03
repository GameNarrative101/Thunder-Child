using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] int shield;
    // bool isDead = false;


    public event EventHandler OnDead;




    public void Damage (int damageAmount)
    {
        shield -= damageAmount;

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

    //     public bool IsDead()
    // {
    //     return isDead;
    // }
}
