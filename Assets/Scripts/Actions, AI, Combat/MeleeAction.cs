using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    enum State { BeforeHit, AfterHit }
    State state;
    float stateTimer;
    PCMech targetUnit;
    public event EventHandler OnMeleeActionStarted;
    public event EventHandler OnMeleeActionCompleted;
    public static event EventHandler OnAnyMeleeActionHit; //for screen shake
    [SerializeField] int maxMeleeDistance = 1;


    void Update()
    {
        bool flowControl = HandleMelee();
        if (!flowControl) { return; }
    }



    //==================================================================================================================================== 
    #region THE ACTION    

    private bool HandleMelee()
    {
        if (!isActive) { return false; }
        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.BeforeHit:
                //rotate towards target
                Vector3 aimDir = (targetUnit.GetWorldPosition() - pCMech.GetWorldPosition()).normalized;
                float rotationSpeed = 30f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, rotationSpeed * Time.deltaTime);
                break;
            case State.AfterHit:

                break;
        }

        if (stateTimer <= 0f) { NextState(); }
        return true;
    }
    private void NextState()
    {
        switch (state)
        {
            //going through the first state happens on the TakeAction function
            case State.BeforeHit:
                state = State.AfterHit;
                float AfterHitStateTime = 0.5f;
                stateTimer = AfterHitStateTime;
                targetUnit.TakeDamage(GetRolledDamage());
                OnAnyMeleeActionHit?.Invoke(this, EventArgs.Empty); //for screen shake
                break;
            case State.AfterHit:
                OnMeleeActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    #endregion



    //==================================================================================================================================== 
    #region OVERRIDES

    public override string GetActionName() => "Melee";
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        for (int x = -maxMeleeDistance; x <= maxMeleeDistance; x++)
        {
            for (int z = -maxMeleeDistance; z <= maxMeleeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;// Skip invalid grid positions
                if (!LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue;// Skip grid positions without any units

                PCMech targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == pCMech.IsEnemy()) continue;// Skip grid positions with allied units

                validGridPositionList.Add(testGridPosition);// Add valid grid position to the list
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action clearBusyOnActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(gridPosition);

        state = State.BeforeHit;
        float BeforeHitStateTime = 0.7f;
        stateTimer = BeforeHitStateTime;

        OnMeleeActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(clearBusyOnActionComplete);
    }
    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }
    protected override (int, int, int) GetDamageByTier() => (10, 20, 30);

    #endregion



    //==================================================================================================================================== 
    #region GETTERS
    public int GetMaxMeleeDistance() => maxMeleeDistance;

    #endregion
}
