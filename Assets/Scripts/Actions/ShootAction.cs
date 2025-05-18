using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    enum State { Aiming, Shooting, Cooloff }

    State state;
    PCMech targetUnit;
    bool canShootBullet;
    float stateTimer;

    //once tested and confirmed, store each state's timer locally
    [SerializeField] float aimingStateTime = 0.5f;
    [SerializeField] float shootingStateTime = 0.1f;
    [SerializeField] float coolOffStateTime = 0.5f;
    [SerializeField] int shootActionDamage = 10;
    [SerializeField] int maxShootDistance = 2;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        //adding more to the event, instead of <> on event handler. another option
        public PCMech targetUnit;
        public PCMech shootingUnit;
    }



    private void Update()
    {
        bool flowControl = HandleShooting();
        if (!flowControl) { return; }
    }



    //==================================================================================================================================== 
    #region SHOOTIN' THANGS
    private bool HandleShooting()
    {
        if (!isActive) { return false; }
        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                //rotate towards target
                Vector3 aimDir = (targetUnit.GetWorldPosition() - pCMech.GetWorldPosition()).normalized;
                float rotationSpeed = 30f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, rotationSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    ShootBullet();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
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
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }
    private void ShootBullet()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = pCMech });

        //static damage amount for now. implement real action logic later
        targetUnit.TakeDamage(shootActionDamage);
    }
    #endregion



    //==================================================================================================================================== 
    #region OVERRIDES
    public override string GetActionName() => "Shoot";
    public override int GetHeatGenerated()
    {
        if (!pCMech.IsEnemy()) { return 4; }
        else { return 0; }
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(gridPosition);
        HealthSystem targetHealthSystem = targetUnit.GetComponent<HealthSystem>();

        state = State.Aiming;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    /* RESTOROE THIS AND DELETE ALL OTHERS */

    /*     public override List<GridPosition> GetValidActionGridPositionList()
        {
            List <GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition pcMechGridPosition = pCMech.GetGridPosition();

            //we weanna cycle through all valid grid positions. so, for every x from left to right within range and same with every z,
            //gimme a new grid position (offset) so we can add it to current grid position
            for (int x = -maxShootDistance; x <= maxShootDistance; x++)
            {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                    // Skip invalid grid positions
                    if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;

                    // Skip grid positions without any units
                    if (!LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue;

                    // Skip grid positions with allied units
                    PCMech targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(testGridPosition);
                    if (targetUnit.IsEnemy() == pCMech.IsEnemy()) continue;

                    // Add valid grid position to the list
                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;   
        } */
    #endregion



    //==================================================================================================================================== 
    #region GETTERS
    public int GetMaxShootDistance() => maxShootDistance;
    public PCMech GetTargetUnit() => targetUnit;
    // public int GetCorePowerCost() => 2;
    // public bool IsValidActionGridPosition(GridPosition gridPosition) => GetValidActionGridPositionList().Contains(gridPosition);
    // public bool IsTargetInRange(PCMech targetUnit) => GetValidActionGridPositionList().Contains(targetUnit.GetGridPosition());
    #endregion



    //==================================================================================================================================== 
    #region ENEMY AI SHOOT ACTION
    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100,
        };
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        return GetValidActionGridPositionList(pcMechGridPosition);

    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition pcMechGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        //we weanna cycle through all valid grid positions. so, for every x from left to right within range and same with every z,
        //gimme a new grid position (offset) so we can add it to current grid position
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                // Skip invalid grid positions
                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;

                // Skip grid positions without any units
                if (!LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue;

                // Skip grid positions with allied units
                PCMech targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == pCMech.IsEnemy()) continue;

                // Add valid grid position to the list
                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }
    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    } 
    #endregion
}
