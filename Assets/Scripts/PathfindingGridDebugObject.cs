using UnityEngine;
using TMPro;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] TextMeshPro gCostText;
    [SerializeField] TextMeshPro hCostText;
    [SerializeField] TextMeshPro fCostText;
    [SerializeField] SpriteRenderer notWalkableVisual;
    PathNode pathNode;






    protected override void Update()
    {
        base.Update();

        //grab the text from the textmeshpro, go to grid object and grab the tostring of it (text override) and put it here.
        //The gridobject class grabs its tostring from the gridposition tostring where we actually told it to take x and z and print it kinda
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
        fCostText.text = pathNode.GetFCost().ToString();
        if (!pathNode.GetIsWalkable())
        {
            notWalkableVisual.gameObject.SetActive(true);
        }
    }
   
    
    




    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }
}
