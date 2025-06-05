using System.Drawing;
using UnityEngine;
using UnityEngine.LightTransport;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] LayerMask mousePlaneLayerMask;

    //creating one instance of this class which is static because there will only ever be one of it (1 mouse).
    //We do this so that we can use mousePlaneLayerMask. If we don't, it just exists in the class, not an instance
    private static MouseWorld instance;

    private void Awake()
    {
        //another part of making an instance of this class. Not a problem because there will only ever be one object this class is attached to.
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        //get the camera to point a Ray called ray at where the mouse is pointing.
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
       
        //get the Ray called ray to give us what it hit with no limitation on distance but only on a layer we created called mousePlaneLayerMask so only the ground will be hit.
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);

        //The impact point in world space where the ray hit the collider.
        return raycastHit.point;
    }
}
