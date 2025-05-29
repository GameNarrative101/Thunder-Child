using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherAction : BaseAction
{
    public override string GetActionName()
    {
        return "Grenade Launcher";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        throw new NotImplementedException();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        throw new NotImplementedException();
    }
}
