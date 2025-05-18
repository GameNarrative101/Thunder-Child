using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform cameraTransform;



    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(cameraTransform);
    }
}
