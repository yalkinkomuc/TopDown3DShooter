using UnityEngine;

public class Car_Interaction : Interactable
{
    private Car_Health_Controller carHealthController;
    private Car_Controller car;
    private Transform player;
    private float defaultPlayerScale;
    private Player Player;
    [Header("Exit Details")]
    [SerializeField] private Transform[] exitPoints;
    [SerializeField] private LayerMask whatToIgnoreForExit;
    [SerializeField] private float exitCheckRadius;
    //public SpriteRenderer aimSpriteeee;


    private void Start()
    {
        //aimSpriteeee = GameObject.Find("Aim").GetComponent<SpriteRenderer>();
        carHealthController = GetComponent<Car_Health_Controller>();
        car = GetComponent<Car_Controller>();
        player = GameManager.instance.player.transform;
        Player = GetComponent<Player>();
        
        foreach (var point in exitPoints)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
            point.GetComponent<SphereCollider>().enabled = false;
        }
    }


    public override void Interaction()
    {
        base.Interaction();

        GetIntoCar();
    }

    public void GetIntoCar()
    {
        ControlsManager.instance.SwitchToCarControls();
        carHealthController.UpdateCarHealthUI();
        car.ActivateCar(true);
        //aimSpriteeee.enabled = false;
        defaultPlayerScale = player.localScale.x;

        player.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.instance.ChangeCameraTarget(transform, 12, .5f);


    }

    public void GetOutOfCar()
    {

        if (car.carActive == false)
            return;

        car.ActivateCar(false);
        


        player.parent = null;
        player.position = GetExitPoint();
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);

        ControlsManager.instance.SwitchToCharacterControls();
        PlayerAim aim = GameManager.instance.player.aim;
        //aimSpriteeee.enabled = true ;
        CameraManager.instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f, 0);
        
    }

    private Vector3 GetExitPoint()
    {
        for (int i = 0; i < exitPoints.Length; i++)
        {
            if (IsExitClear(exitPoints[i].position))
                return exitPoints[i].position;
        }

        return exitPoints[0].position;
    }

    private bool IsExitClear(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(point, exitCheckRadius, ~whatToIgnoreForExit);
        return colliders.Length == 0;
    }

    private void OnDrawGizmos()
    {
        if (exitPoints.Length > 0)
        {
            foreach (var point in exitPoints)
            {
                Gizmos.DrawWireSphere(point.position, exitCheckRadius);
            }
        }
    }
}
