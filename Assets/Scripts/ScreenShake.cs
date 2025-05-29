using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    CinemachineImpulseSource cinemachineImpulseSource;

    void Awake()
    {
        SetInstanceAndDebug();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one ScreenShake" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Shake(float intensity=1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
