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
    [SerializeField] bool isEnemy;

    [SerializeField] int corePower = 3;
    [SerializeField] int corePowerIncrease = 3;
    [SerializeField] int maxCorePower = 15;
    [SerializeField] int heat = 0;
    [SerializeField] int maxHeat = 10;
    /* [SerializeField] */ float knockbackSpeed = 60f;

    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public static event EventHandler OnCorePowerChange;
    public static event EventHandler OnHeatChange;
    //static makes it so any instance of this class in other classes can change things, and all instances will be updated



    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }
    private void Start()
    {
        SetGridPosition();

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        healthSystem.OnDead += HealthSystem_OnDead;

        // Time.timeScale = 0.1f;
    }
    private void Update()
    {
        GetNewGridPositionAndUpdate();
    }



    //==================================================================================================================================== 
    #region GRID FUNCTIONS 

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

    #endregion



    //==================================================================================================================================== 
    #region RESOURCE FUNCTIONS

    void SpendCorePower(int amount) // instead of making it impossible to go below 0, we make it impossible to spend power you don't have (on UnitActionSystem)
    {
        corePower -= amount;
        OnCorePowerChange?.Invoke(this, EventArgs.Empty);
        if (corePower > maxCorePower) { print("Overloaded!"); }
    }
    void ReduceHeat(int amount)
    {
        heat -= amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat < 0) { heat = 0; }
    }

    #endregion



    //==================================================================================================================================== 
    #region SUBSCRIPTIONS

    void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        //increase core power at the beginning of the player's turn
        if ((IsEnemy() && !TurnSystemScript.Instance.IsPlayerTurn()) ||
        (!IsEnemy() && TurnSystemScript.Instance.IsPlayerTurn()))
        {
            GainCorePower(corePowerIncrease);
            OnCorePowerChange?.Invoke(this, EventArgs.Empty);

            TryReduceHeat(2);
            OnHeatChange?.Invoke(this, EventArgs.Empty);
        }
    }
    void HealthSystem_OnDead(object sender, EventArgs e)
    {
        isDead = true;
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        Destroy(gameObject);
    }

    #endregion



    //==================================================================================================================================== 
    #region PUBLIC FUNCTIONS

    public bool IsEnemy() => isEnemy;
    public bool IsDead() => isDead;
    public void TakeDamage(int damageAmount) { healthSystem.Damage(damageAmount); }
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
    public void GainCorePower(int amount)
    {
        corePower += amount;
        OnCorePowerChange?.Invoke(this, EventArgs.Empty);
        if (corePower > maxCorePower) { print("Overloaded!"); }
    }
    public void GainHeat(int amount) //effects of overheating to be implemented later
    {
        heat += amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat > maxHeat) { print("Overheated!"); }
    }
    public void TryReduceHeat(int amount)
    {
        if (heat - amount >= 0) { ReduceHeat(amount); }
        print("Not enough heat to reduce!");
    }
    public void ResetHeat()
    {
        heat = 0;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
    }
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

        List <GridPosition> knockbackPath = new List<GridPosition>();
        GridPosition current = selfGridPos;
        for (int i = 0; i < knockbackDistance; i++)
        {
            GridPosition next = new GridPosition(current.x + knockbackDir.x, current.z + knockbackDir.y);
            if (!LevelGrid.Instance.IsValidPosition(next)) break; //CONDITIONS WILL CHANGE SIGNIFICANTLY ONCE DAMAGE IS ADDED TO KNOCKBACK
            if (LevelGrid.Instance.HasAnyPcMechOnGridPosition(next)) break;

            knockbackPath.Add(next);
            current = next;
        }

        if (knockbackPath.Count == 0)
        {
            Debug.Log("No valid tiles for knockback");
            return;
        }

        StartCoroutine(KnockbackMoveRoutine(knockbackPath));
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
    #region GETTIN' THANGS

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
    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;
    public int GetCorePower() => corePower;
    public float GetCorePowerNormalized() => corePower / (float)maxCorePower;
    public int GetHeat() => heat;
    public float GetHeatNormalized() => heat / (float)maxHeat;

    #endregion

        /* 
        KNOCKBACK:
        handle like the pcmech takedamage method
        receive and pass a direction into the method somehow, and an int foor number of squares to knock back
        then, in the action scripts, that just goes into the same place as takedamage
         */
}
