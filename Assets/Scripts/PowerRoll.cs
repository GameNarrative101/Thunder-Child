using UnityEngine;

public class PowerRoll : MonoBehaviour
{
    public static PowerRoll Instance { get; private set; }
    public enum PowerRollTier { Tier1, Tier2, Tier3 }
    int rollBonus = 3;



    private void Awake()
    {
        SetInstanceAndDebug();
    }



    void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError($"Multiple PowerRoll instances! This: {transform.name}, Existing: {Instance.transform.name}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public PowerRollTier Roll(int bonusModifier = 0) //CAN receive an extra bonus modifier (from actions for now)
    {
        int roll1 = Random.Range(1, 11); // 1 to 10 inclusive
        int roll2 = Random.Range(1, 11);
        int total = roll1 + roll2 + rollBonus + bonusModifier;

        PowerRollTier tier;

        if (total <= 11) tier = PowerRollTier.Tier1;
        else if (total <= 16) tier = PowerRollTier.Tier2;
        else tier = PowerRollTier.Tier3;

        Debug.Log($"Power Roll: {roll1} + {roll2} + BaseBonus({rollBonus}) + Extra({bonusModifier}) = {total} → {tier}");

        return tier;
    }
}
