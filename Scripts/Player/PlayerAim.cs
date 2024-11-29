using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private CameraManager cameraManager;
    private Player player;
    private PlayerControls controls;


    [SerializeField] private Transform aim;

    [SerializeField] private bool isAimingPrecisely;
    [SerializeField] private float offsetChangeRate = 6;
    private float offsetY;

    [Header("Aim Visual - Laser")]

    [SerializeField] private LineRenderer aimLaser;

    [Header("AimCameraControl")]
    [SerializeField] private float preciseAimCamDistance = 6;
    [SerializeField] private float regularAimCamDistance = 7;
    [SerializeField] private float camChangeRate = 5;
    [Space]

    [Header("Aim Layers")]
    [SerializeField] private LayerMask regularAim;
    [SerializeField] private LayerMask preciseAim;

    [Header("CameraControl")]
    [SerializeField] Transform cameraTarget;
    [SerializeField] private float minCameraDistance = 1.5f;
    [SerializeField] private float maxCameraDistance = 4;
    [SerializeField] private float CameraSensitivity = 5f;

    private Vector2 mouseInput;

    private RaycastHit lastKnownMousePOS;
    private void Start()
    {
        cameraManager = CameraManager.instance;
        player = GetComponent<Player>();
        AssignInputEvents();
        //Cursor.visible = false;
    }

    private void EnablePreciseAim(bool enable)
    {
        isAimingPrecisely = !isAimingPrecisely;
        //Cursor.visible = false ;

        if (enable)
        {
            cameraManager.ChangeCameraDistance(preciseAimCamDistance, camChangeRate);
            Time.timeScale = .9f;
        }
        else
        {
            cameraManager.ChangeCameraDistance(regularAimCamDistance, camChangeRate);
            Time.timeScale = 1;
        }
    }

    private void Update()
    {
        if (player.health.isDead)
            return;

        if (player.controlsEnabled == false)
            return;




        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    public Transform GetAimCameraTarget()
    {
        cameraTarget.position = player.transform.position;
        return cameraTarget;
    }

    public void EnableAimLaser(bool enable) => aimLaser.enabled = enable;
    private void UpdateAimVisuals()
    {

        aim.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        aimLaser.enabled = player.weaponController.WeaponReady();

        if (aimLaser.enabled == false)
            return;

        WeaponModel weaponModel = player.visualController.CurrentWeaponModel();

        weaponModel.transform.LookAt(aim);
        weaponModel.gunPoint.LookAt(aim);


        Transform gunPoint = player.weaponController.GunPoint();
        Vector3 laserDirection = player.weaponController.BulletDirection();

        float laserTipLenght = .5f;
        float gunDistance = player.weaponController.CurrentWeapon().gunDistance;

        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLenght = 0;
        }


        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght);
    }
    private void UpdateAimPosition()
    {


        aim.position = getMouseHitInfo().point;

        Vector3 newAimPosition = isAimingPrecisely ? aim.position : transform.position;

        aim.position = new Vector3(aim.position.x, newAimPosition.y + AdjustedOffsetY(), aim.position.z);


    }

    private float AdjustedOffsetY()
    {
        if (isAimingPrecisely)
            offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * offsetChangeRate * .5f);
        else
            offsetY = Mathf.Lerp(offsetY, 1, Time.deltaTime * offsetChangeRate);

        return offsetY;
    }

    public Transform Aim() => aim;



    public bool canAimPrecisely() => isAimingPrecisely;

    #region Camera
    
    private void UpdateCameraPosition()
    {

        bool canMoveCamera = Vector3.Distance(cameraTarget.position, desiredCamPosition()) > 1;

        if(canMoveCamera == false)
            return; 

        cameraTarget.position = Vector3.Lerp(cameraTarget.position, desiredCamPosition(), CameraSensitivity * Time.deltaTime);
    }

    private Vector3 desiredCamPosition()
    {
        float actualMaxCameraDistance;
        bool movingDownwards = player.movement.moveInput.y < -.5f;

        if (movingDownwards)
        {
            actualMaxCameraDistance = minCameraDistance;
        }
        else
            actualMaxCameraDistance = maxCameraDistance;


        Vector3 desiredCamPosition = getMouseHitInfo().point;
        Vector3 aimDirection = (desiredCamPosition - transform.position).normalized;

        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCamPosition);
        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);



        desiredCamPosition = transform.position + aimDirection * clampedDistance;
        desiredCamPosition.y = transform.position.y + 1;

        return desiredCamPosition;

    }
    #endregion
    public RaycastHit getMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, preciseAim))
        {
            lastKnownMousePOS = hitInfo;
            return hitInfo;
        }

        return lastKnownMousePOS;

    }
    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.PreciseAim.performed += context => EnablePreciseAim(true);
        controls.Character.PreciseAim.canceled += context => EnablePreciseAim(false);


        controls.Character.Aim.performed += context => mouseInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;
    }
}
