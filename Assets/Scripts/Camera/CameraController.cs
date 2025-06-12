using System;
using Unity.Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 10f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float yawFactor = 80f;
    [SerializeField] float defaultPitch = 26.73f;
    [SerializeField] float followSmoothTime = 0.15f;
    [SerializeField] const float MinFollowYOffset = 5.44f;
    [SerializeField] const float MaxFollowYOffset = 12.7f;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] Transform followTarget;
    [SerializeField] Vector3 followOffset = new Vector3(0, 0, 0);
    // [SerializeField] float yRelativeToParent = -10.7f;

    Vector3 currentVelocity;
    Vector3 targetFollowOffset;
    CinemachineFollow followComponent;
    float accumulatedYaw = 0f;
    bool isFollowingTarget = false;



    private void Start()
    {
        followComponent = (CinemachineFollow)cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        targetFollowOffset = followComponent.FollowOffset;

        FollowTarget();

        MoveAction moveAction = FindFirstObjectByType<MoveAction>();
        if (moveAction != null)
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
    }
    void Update()
    {
        AutoVSManualCameraMove();
        ProcessMove();
        ProcessRotation();
        ProcessZoom();
    }

    private void OnDestroy()
    {
        MoveAction moveAction = FindFirstObjectByType<MoveAction>();
        if (moveAction != null)
        {
            moveAction.OnStartMoving -= MoveAction_OnStartMoving;
            moveAction.OnStopMoving -= MoveAction_OnStopMoving;
        }
    }
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        isFollowingTarget = true;
        FollowTarget();
    }
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        isFollowingTarget = false;
    }
    private void AutoVSManualCameraMove()
    {
        bool manualInput = InputManager.Instance.GetCameraMoveVector() != Vector2.zero;

        if (manualInput)
        {
            isFollowingTarget = false;
        }

        if (isFollowingTarget && followTarget != null)
        {
            FollowTarget();
        }
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
    void FollowTarget()
    {
        if (followTarget == null) return;

        // Smoothly follow the target position with an offset
        Vector3 targetPosition = followTarget.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, followSmoothTime);
    }
}
