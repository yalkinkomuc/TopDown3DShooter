using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer frameTransposer;

    [SerializeField] private bool canChangeCamDistance;

    [SerializeField] private float targetCameraDistance;
    [SerializeField] private float distanceChangeRate;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

        virtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        frameTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();

    }

    private void Update()
    {
        UpdateCameraDistance();

    }

    private void UpdateCameraDistance()
    {
        if(canChangeCamDistance == false)
            return;

        float currentDistance = frameTransposer.m_CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistance) > 0.1f)
            return;
        
            frameTransposer.m_CameraDistance = Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);
        
    }

    public void ChangeCameraDistance(float distance,float newChangeRate = .25f)
    {
        distanceChangeRate = newChangeRate;
        targetCameraDistance = distance;
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;

    public void ChangeCameraTarget(Transform target,float cameraDistance = 10,float newLookAheadTime = 0)
    {
        virtualCam.Follow = target;
        frameTransposer.m_LookaheadTime = newLookAheadTime;
        ChangeCameraDistance(cameraDistance);
    }
}
