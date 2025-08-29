using System;
using System.Collections.Generic;
using UnityEngine;

public class PCMech : MonoBehaviour
{
    //all grid position stuff are here for the sake of forced movement handling
    GridPosition gridPosition;
    HealthSystem healthSystem;
    BaseAction[] baseActionArray;

    bool isDead = false;
    float knockbackSpeed = 60f;
    int turnsTaken = 0;

    [SerializeField] bool isEnemy;
    [SerializeField] int corePower = 3;
    [SerializeField] int corePowerPerTurn = 3;
    [SerializeField] int maxCorePower = 15;
    [SerializeField] int heat = 0;
    [SerializeField] int maxHeat = 10;
    [SerializeField] int heatReductionPerTurn = 3;

    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnCorePowerChange;
    public static event EventHandler OnHeatChange;
    //static makes it so any instance of this class in other classes can change things, and all instances will be updated



    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
        BaseAction.ResetPlayerActionFlag();
    }
    private void Start()
    {
        SetGridPosition();
        baseActionArray = GetComponents<BaseAction>();

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        healthSystem.OnDead += HealthSystem_OnDead;
    }
    private void Update()
    {
        GetNewGridPositionAndUpdate();
    }



    //==================================================================================================================================== 
    #region GRID & MOVEMENT 

    void SetGridPosition()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddMechAtGridPosition(gridPosition, this);
    }
    void GetNewGridPositionAndUpdate()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.MechMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }
    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;

    public void Knockback(int knockbackDistance, GridPosition forceOriginGridPosition)
    {
        GridPosition selfGridPos = GetGridPosition();
        GridPosition[] knockbackDirectionPath = LevelGrid.Instance.GetDirection(forceOriginGridPosition, selfGridPos);

        if (knockbackDirectionPath.Length < 2)
        {
            Debug.LogWarning("Knockback direction invalid — no path found.");
            return;
        }

        Vector2Int knockbackDir = new Vector2Int(
            knockbackDirectionPath[^1].x - knockbackDirectionPath[^2].x,
            knockbackDirectionPath[^1].z - knockbackDirectionPath[^2].z
        );

        List<GridPosition> knockbackPath = new List<GridPosition>();
        GridPosition current = selfGridPos;

        for (int i = 0; i < knockbackDistance; i++)
        {
            GridPosition next = new GridPosition(current.x + knockbackDir.x, current.z + knockbackDir.y);

            if (!LevelGrid.Instance.IsValidPosition(next)) break; 
            if (LevelGrid.Instance.HasAnyPcMechOnGridPosition(next)) break;

            knockbackPath.Add(next);
            current = next;
        }

        int actualDistance = knockbackPath.Count;
        int knockbackDamage = knockbackDistance - actualDistance;

        if (actualDistance == 0)
        {
            Debug.Log("Knockback blocked immediately — applying full knockback damage.");
        }

        if (actualDistance > 0)
        {
            StartCoroutine(KnockbackMoveRoutine(knockbackPath));
        }

        if (knockbackDamage > 0)
        {
            Debug.Log($"Partial knockback damage: dealing {knockbackDamage} damage.");
            TakeDamage(knockbackDamage);
        }
    }
    private System.Collections.IEnumerator KnockbackMoveRoutine(List<GridPosition> path)
    {
        foreach (GridPosition targetGridPos in path)
        {
            Vector3 start = transform.position;
            Vector3 end = LevelGrid.Instance.GetWorldPosition(targetGridPos);

            float timer = 0f;
            float duration = Vector3.Distance(start, end) / knockbackSpeed;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            transform.position = end;
        }

        GetNewGridPositionAndUpdate();
    }

    #endregion



    //==================================================================================================================================== 
    #region RESOURCES

    public bool CanSpendCorePowerForAction(BaseAction baseAction) { return corePower >= baseAction.GetCorePowerCost(); }
    public bool TrySpendCorePowerForAction(BaseAction baseAction)
    {
        if (CanSpendCorePowerForAction(baseAction))
        {
            SpendCorePower(baseAction.GetCorePowerCost());
            return true;
        }
        return false;
    }
    void SpendCorePower(int amount) // UnitActionSystem makes it impossible to spend power you don't have
    {
        corePower -= amount;
        OnCorePowerChange?.Invoke(this, EventArgs.Empty);
    }
    public void GainCorePower(int amount)
    {
        corePower += amount;
        OnCorePowerChange?.Invoke(this, EventArgs.Empty);

        if (corePower > maxCorePower)
        {
            OverLoad();
        }
    }
    private void OverLoad()
    {
        int overloadAmount = corePower - maxCorePower;

        if (isEnemy == false) // NO ENEMY OVERLOAD
        {
            GainHeat(overloadAmount);
        }
    }
    public int GetCorePower() => corePower;
    public float GetCorePowerNormalized() => corePower / (float)maxCorePower;

    public void TryReduceHeat(int amount)
    {
        if (heat - amount >= 0) { ReduceHeat(amount); }
        print("Not enough heat to reduce!");
    }
    void ReduceHeat(int amount)
    {
        heat -= amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat < 0) { heat = 0; }
    }
    public void ResetHeat()
    {
        heat = 0;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
    }
    public void GainHeat(int amount) //effects of overheating to be implemented later
    {
        heat += amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat > maxHeat)
        {
            OverHeat();
        }
    }
    private void OverHeat()
    {
        int overHeatAmount = heat - maxHeat;

        if (isEnemy == false) //NO ENEMY OVERHEAT
        {
            TakeDamage(overHeatAmount);
        } 
    }
    public int GetHeat() => heat;
    public float GetHeatNormalized() => heat / (float)maxHeat;

    #endregion



    //==================================================================================================================================== 
    #region HEALTH & ACTIONS

    public bool GetIsEnemy() => isEnemy;
    public bool GetIsDead() => isDead;
    public void TakeDamage(int damageAmount) { healthSystem.Damage(damageAmount); }
    
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }
    public BaseAction[] GetBaseActionArray() => baseActionArray;
    public void IncrementTurnTaken() => turnsTaken++;
    public int GetTurnsTaken() => turnsTaken;
    public void ResetTurnsTaken() => turnsTaken = 0;

    #endregion



    //==================================================================================================================================== 
    #region SUBSCRIPTIONS

    void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        //increase core power at the beginning of the player's turn
        if ((GetIsEnemy() && !TurnSystemScript.Instance.GetIsPlayerTurn()) ||
        (!GetIsEnemy() && TurnSystemScript.Instance.GetIsPlayerTurn()))
        {
            GainCorePower(corePowerPerTurn);
            OnCorePowerChange?.Invoke(this, EventArgs.Empty);

            TryReduceHeat(heatReductionPerTurn);
            OnHeatChange?.Invoke(this, EventArgs.Empty);
        }

        BaseAction.ResetPlayerActionFlag(); // Reset the player action flag for the next turn
    }
    void HealthSystem_OnDead(object sender, EventArgs e)
    {
        isDead = true;
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        Destroy(gameObject);
    }

    #endregion

}
