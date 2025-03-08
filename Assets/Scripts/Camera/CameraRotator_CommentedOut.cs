//using UnityEngine;
//using UnityEngine.InputSystem;

//public class CameraRotator_CommentedOut : MonoBehaviour
//{
//    //[SerializeField] float cameraSpeed = 10f;
//    //[SerializeField] float yRelativeToParent = -10.7f;
//    //[SerializeField] float rotationSpeed = 10f;
//    [SerializeField] float defaultPitch = 26.73f;
//    [SerializeField] float yawFactor = 80f;
//    //Vector3 inputMoveDirection = new Vector3(0, 0, 0);
//    //Vector2 movement;
//    Vector2 rotation;
//    float accumulatedYaw = 0f;


//    void Update()
//    {
//        //ProcessTranslation();
//        ProcessRotation();
//    }
//    public void OnRotate(InputValue inputValue)
//    {
//        rotation = inputValue.Get<Vector2>();
//    }

//    void ProcessRotation()
//    {
//        accumulatedYaw += rotation.x * yawFactor * Time.deltaTime;

//        transform.localRotation = Quaternion.Euler(defaultPitch, accumulatedYaw, 0);
//    }

//    //public void OnMove (InputValue inputValue)
//    //{
//    //    movement = inputValue.Get<Vector2>();
//    //}

//    // void ProcessTranslation()
//    //{
//    //    //float xMove = movement.x * cameraSpeed * Time.deltaTime;
//    //    //float zMove = movement.y * cameraSpeed * Time.deltaTime;

//    //    //transform.localPosition = new Vector3(transform.localPosition.x + xMove, yRelativeToParent, transform.localPosition.z + zMove);


//    //    Vector3 moveVector = transform.forward * movement.y + transform.right * movement.x;

//    //    transform.position += moveVector * cameraSpeed * Time.deltaTime;

//    //}




//}
