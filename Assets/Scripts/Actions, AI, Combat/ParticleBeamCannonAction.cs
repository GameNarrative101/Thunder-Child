using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBeamCannonAction : BaseAction
{
    [SerializeField] Transform beamProjectilePrefab;
    [SerializeField] int beamLength = 7;

    protected override void Awake()
    {
        base.Awake();
        isEnemyAction = false;
    }



    public override string GetActionName() => "Particle Beam Cannon";
    protected override (int, int, int) GetDamageByTier() => (18, 30, 42);
    public override int GetCorePowerCost() => 7;
    public override int GetHeatGenerated() => 7;

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition originGridPosition = pCMech.GetGridPosition();
        Vector2Int[] directions = new Vector2Int[] //all 8 directions, up to beamLength
        {
            new Vector2Int(1, 0),   // E
            new Vector2Int(-1, 0),  // W
            new Vector2Int(0, 1),   // N
            new Vector2Int(0, -1),  // S
            new Vector2Int(1, 1),   // NE
            new Vector2Int(-1, 1),  // NW
            new Vector2Int(1, -1),  // SE
            new Vector2Int(-1, -1), // SW
        };

        foreach (var dir in directions)
        {
            for (int i = 1; i <= beamLength; i++)
            {
                GridPosition target = originGridPosition + new GridPosition(dir.x * i, dir.y * i); // wait, Y or Z?!
                if (!LevelGrid.Instance.IsValidPosition(target)) break;
                validGridPositionList.Add(target);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action clearBusyOnActionComplete)
    {
        ActionStart(clearBusyOnActionComplete);

        GridPosition startGridPosition = pCMech.GetGridPosition();
        GridPosition[] path = LevelGrid.Instance.GetDirection(startGridPosition, targetGridPosition);
        List<GridPosition> damageLine = new List<GridPosition>(); //skipping the origin 

        for (int i = 1; i < path.Length && i <= beamLength; i++)
        {
            damageLine.Add(path[i]);
        }

        //beam VFX
        Transform beamTransform = Instantiate(beamProjectilePrefab, pCMech.GetWorldPosition(), Quaternion.identity);
        ParticleBeam beam = beamTransform.GetComponent<ParticleBeam>();
        beam.SetShooter(pCMech);
        beam.TryShootBeam(LevelGrid.Instance.GetWorldPosition(damageLine[^1])); //endpoint of beam

        BeamDamage(damageLine);
    }
    private void BeamDamage(List<GridPosition> damageLine)
    {
        var (rolledDamage, _, _) = GetRolledDamageAndKnockback();
        foreach (GridPosition gridPos in damageLine)
        {
            List<PCMech> pcMechList = new List<PCMech> (LevelGrid.Instance.GetMechListAtGridPosition(gridPos));
            foreach (PCMech mech in pcMechList)
            {
                mech.TakeDamage(rolledDamage);
            }
        }

        ActionComplete();
    }
}
