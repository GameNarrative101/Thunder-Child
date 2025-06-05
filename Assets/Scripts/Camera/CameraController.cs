using Unity.Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 10f;
    [SerializeField] const float MinFollowYOffset = 5.44f;
    [SerializeField] const float MaxFollowYOffset = 12.7f;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float defaultPitch = 26.73f;
    [SerializeField] float yawFactor = 80f;
    // [SerializeField] float yRelativeToParent = -10.7f;

    Vector3 targetFollowOffset;
    CinemachineFollow followComponent;
    float accumulatedYaw = 0f;

    private void Start()
    {
        //this thing is of type Cine..Follow then the last part is how you access the new cinemachine components I guess. camera pipeline
        followComponent = (CinemachineFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        targetFollowOffset = followComponent.FollowOffset;
    }

    void Update()
    {
        ProcessMove();
        ProcessRotation();
        ProcessZoom();
    }



    void ProcessMove()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        float originalY = transform.position.y;
        transform.position += moveVector * cameraSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
    }
    void ProcessRotation()
    {   
        accumulatedYaw += InputManager.Instance.GetCameraRotateAmount() * yawFactor * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(defaultPitch, accumulatedYaw, 0);
    }
    private void ProcessZoom()
    {
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MinFollowYOffset, MaxFollowYOffset);
        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime);
    }
}
