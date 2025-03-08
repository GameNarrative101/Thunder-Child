using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine.TargetTracking;
using System.Security.Cryptography;

public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 10f;
    [SerializeField] float yRelativeToParent = -10.7f;
    [SerializeField] float defaultPitch = 26.73f;
    [SerializeField] float yawFactor = 80f;
    [SerializeField] const float MinFollowYOffset = 5.44f;
    [SerializeField] const float MaxFollowYOffset = 12.7f;
    [SerializeField] float zoomAmount = 1f;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float zoomSpeed = 5f;

    Vector2 movement;
    Vector2 rotation;
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
        ProcessTranslation();
        ProcessRotation();
        ProcessZoom();
    }


    //Camera Movement (WASD):
    public void OnMove (InputValue inputValue)
    {
        movement = inputValue.Get<Vector2>();
    }
    void ProcessTranslation()
    {
        //gets input, makes a vector 3 of it, and makes sure AFTER it's done, that its y is always zero, no moving that one
        Vector3 moveVector = transform.forward * movement.y + transform.right * movement.x;
        moveVector.y = 0;

        //makes a vector 3 of where we are plus all 3 vectors above, then makes sure y is the thing we set. no moving that one
        Vector3 newPosition = transform.position + moveVector * cameraSpeed * Time.deltaTime;
        newPosition.y = yRelativeToParent; 

        transform.position = newPosition;
    }

    
    //Camera Rotation (Q/E):
    public void OnRotate (InputValue inputValue)
    {
        rotation = inputValue.Get<Vector2>();
    }
    void ProcessRotation()
    {
        accumulatedYaw += rotation.x * yawFactor * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(defaultPitch, accumulatedYaw, 0);
    }
    
    
    //Camera Zoom (scroll)
    private void ProcessZoom()
    {
        //Vector3 followOffset = followComponent.FollowOffset;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MinFollowYOffset, MaxFollowYOffset);
        followComponent.FollowOffset = Vector3.Lerp(followComponent.FollowOffset, targetFollowOffset, zoomSpeed * Time.deltaTime);
    }



}
