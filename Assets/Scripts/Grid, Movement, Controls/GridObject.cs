using System.Collections.Generic;

//wanna use the constructor so no mono. This object will later have a bunch of use, like having a list of pcmechs on the object, etc.
public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    //make it a LIST of mechs to allow for multiple pc units to be on the same cell. Not in a game rule sense, just in a system understands it sense. cells keep updating nice
    private List<PCMech> pCMechList;
    IInteractable interactable;



    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        pCMechList = new List<PCMech>();
    }
    public override string ToString()
    {
        string pcMechString = "";
        foreach (PCMech pCMech in pCMechList)
        {
            pcMechString += pCMech + "\n";
        }
        //"\n" is new line. this makes it so each cell tells us when a pcMech is in that cell on top of just the numbers
        return gridPosition.ToString() + "\n" + pcMechString;
    }
    public PCMech GetPCMech()
    {
        if (HasAnyPcMech()) { return pCMechList[0]; }
        else { return null; }
    }
    public void AddPcMech(PCMech pcMech) { pCMechList.Add(pcMech); }
    public void RemovePcMech(PCMech pCMech) { pCMechList.Remove(pCMech); }
    public List<PCMech> GetPcMechList() { return pCMechList; }
    public bool HasAnyPcMech() { return pCMechList.Count > 0; }
    public IInteractable GetInteractable() => interactable;
    public void SetInteractable(IInteractable interactable) { this.interactable = interactable; }
}
