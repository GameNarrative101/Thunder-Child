using UnityEngine;

//for handling each prefab. used when gridsystemvisuals instantiates prefabs
public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] PCMech selectedPcMech;



    public void Show(Material material)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = material;
    }
    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
