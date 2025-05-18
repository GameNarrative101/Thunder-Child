using UnityEngine;
using TMPro;

//called from gridsystem
public class GridDebugObject : MonoBehaviour
{
    [SerializeField] TextMeshPro textMeshPro;
    public object gridObject;

    public virtual void SetGridObject (object gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        //grab the text from the textmeshpro, go to grid object and grab the to string of it (text override) and put it here.
        //The gridobject class grabs its tostring from the gridposition tostring where we actually told it to take x and z and print it kinda
        textMeshPro.text = gridObject.ToString();
    }
}
