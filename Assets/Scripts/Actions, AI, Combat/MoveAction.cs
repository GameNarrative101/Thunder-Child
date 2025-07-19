using UnityEngine;
using System.Collections.Generic;
using System;
public class MoveAction : BaseAction
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] int maxMoveDistance = 4;

    List<Vector3> positionList;
    Vector3 targetPosition;
    int currentPositionIndex;
    int enemyMovesLeft = 1;

    public event EventHandler OnStopMoving;
    public event EventHandler<OnStartMovingEventArgs> OnStartMoving;
    public class OnStartMovingEventArgs : EventArgs
    {
        public Vector3 worldFrom;
        public Vector3 worldTo;
    }



    private void Start()
    {
        if (TurnSystemScript.Instance != null)
        {
            TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        }
        else
        {
            Debug.LogError("TurnSystemScript.Instance is null in MoveAction.Start");
        }
    }
    void Update()
    {
        if (!isActive) return;

        clickToMove();
    }



    //==================================================================================================================================== 
    #region MOVING THINGS

    void clickToMove()
    {
        targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Smooth rotation toward move direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move with MoveTowards to avoid overshooting and hitching
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if we've arrived at the current path point
        if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            currentPositionIndex++;

            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    #endregion  



    //==================================================================================================================================== 
    #region OVERRIDES

    public override string GetActionName() => "Move";
    public override int GetCorePowerCost() => 1;
    public override int GetHeatGenerated() => 1;
    public override void TakeAction(GridPosition gridPosition, Action clearBusyOnActionComplete)
    {
        if (pCMech.GetIsEnemy())
        {
            enemyMovesLeft--;
        }

        List<GridPosition> pathGridPositionList =
            Pathfinding.Instance.FindPath(pCMech.GetGridPosition(), gridPosition, out int pathLength); //where pathfinding happens

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, new OnStartMovingEventArgs
        {
            worldFrom = transform.position,
            worldTo = targetPosition 
        });

        ActionStart(clearBusyOnActionComplete);
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;
                if (pcMechGridPosition == testGridPosition) continue;//unit's own position is invalid
                if (LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue; //other unit's positions are invalid
                if (!Pathfinding.Instance.GetIsWalkableGridPosInPath(testGridPosition)) continue; //invalid if the pathfinding node isn't walkable
                if (!Pathfinding.Instance.HasValidPath(pcMechGridPosition, testGridPosition)) continue; //invalid if there's no path to target position

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(pcMechGridPosition, testGridPosition) >
                    maxMoveDistance * pathfindingDistanceMultiplier) continue; //path too long, position invalid

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    #endregion



    //==================================================================================================================================== 
    #region ENEMY MOVE

    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        if (pCMech != null && pCMech.GetIsEnemy())
        {
            enemyMovesLeft = 1;
        }
    }
    public override EnemyAIAction GetBestEnemyAIAction(GridPosition simulatedMovePosition)
    {
        if (pCMech.GetIsEnemy() && enemyMovesLeft <= 0)
        {
            return null;
        }

        PCMech closestTarget = GetClosestPCMech(simulatedMovePosition);
        if (closestTarget != null && Pathfinding.Instance.HasValidPath(simulatedMovePosition, closestTarget.GetGridPosition()))
        {
            int distance = Pathfinding.Instance.GetPathLength(simulatedMovePosition, closestTarget.GetGridPosition());

            int fallbackValue = Mathf.Max(1, 100 - distance); // Closer = higher value
            return new EnemyAIAction
            {
                gridPosition = simulatedMovePosition,
                actionValue = fallbackValue
            };
        }

        return null;
    }
    private PCMech GetClosestPCMech(GridPosition fromPosition)
    {
        PCMech closest = null;
        int shortestDistance = int.MaxValue;

        foreach (PCMech target in UnitManager.Instance.GetFriendlyMechList())
        {
            int distance = Pathfinding.Instance.GetPathLength(fromPosition, target.GetGridPosition());
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = target;
            }
        }

        return closest;

    }
    
    #endregion
}
