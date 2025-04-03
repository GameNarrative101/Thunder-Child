using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    State state;
    int maxShootDistance =5;
    float stateTimer;
    PCMech targetUnit;
    bool canShootBullet;

    //once tested and confirmed, store each state's timer locally
    [SerializeField] float aimingStateTime = 0.5f;
    [SerializeField] float shootingStateTime = 0.1f;
    [SerializeField] float coolOffStateTime = 0.5f;
    [SerializeField] int shootActionDamage = 10;

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
        if (!flowControl) {return;}
    }




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

        if (stateTimer <= 0f)
        {
            NextState();
        }

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
        OnShoot?.Invoke(this, new OnShootEventArgs{targetUnit=targetUnit, shootingUnit=pCMech});
        
        //static damage amount for now. implement real action logic later
        targetUnit.TakeDamage(shootActionDamage);
    }




    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
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

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition))
                {
                    //if the grid position the loop gives us is not valid, get another one from the loop, otherwise execute
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition))
                {
                    //skip all grid positions that are empty
                    continue;
                }

                PCMech targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == pCMech.IsEnemy())
                {
                    //check for target being an ally
                    continue;
                }
                //at this point, we know we have a valid grid with a unit on it, 
                //but we need to know which unit from the list is on there (from LevelGrid)
                validGridPositionList.Add(testGridPosition);

            }
        }
        return validGridPositionList;   
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(gridPosition);
        HealthSystem targetHealthSystem = targetUnit.GetComponent<HealthSystem>();

        state = State.Aiming;
        stateTimer = aimingStateTime;

        canShootBullet = true;
    }
}
