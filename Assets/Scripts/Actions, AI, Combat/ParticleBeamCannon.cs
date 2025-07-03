using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBeamCannon : BaseAction
{
    int maxParticleBeamDistance = 7;

    public override string GetActionName() => "Particle Beam Cannon";

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        throw new NotImplementedException();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        /* 
        1. make a particle beam prefab
        2. in its script, make it extend from the PCMech position to the mousegrid position (a la grenade projectile)
        3. make it deal damage to all PCMechs in its path (a la grenadelauncher)


        KNOCKBACK:
        handle like the pcmech takedamage method
        receive and pass a direction into the method somehow, and an int foor number of squares to knock back
        then, in the action scripts, that just goes into the same place as takedamage
         */
    }
}
