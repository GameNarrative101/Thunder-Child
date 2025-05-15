using UnityEngine;

public class PathNode
{
    public GridPosition gridPosition { get; private set; }
    int gCost;
    int hCost;
    int fCost;
    PathNode cameFromPathNode;
    public bool isWalkable = true;




    public PathNode (GridPosition gridPosition) => this.gridPosition = gridPosition;
    public GridPosition GetGridPosition() => gridPosition;
    public override string ToString() => $"{gridPosition.x}, {gridPosition.z}";
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;
    public void SetGCost(int gCost) => this.gCost = gCost;
    public void SetHCost(int hCost) => this.hCost = hCost;
    public void CalculateFCost() => fCost = gCost + hCost;
    public void ResetCameFromPathNode() => cameFromPathNode = null;
    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;
    public PathNode GetCameFromPathNode() => cameFromPathNode;
    public bool GetIsWalkable() => isWalkable;
    public void SetIsWalkable(bool isWalkable) => this.isWalkable = isWalkable;
}
