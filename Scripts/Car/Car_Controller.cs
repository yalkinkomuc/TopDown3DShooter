using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum driveType { frontWheelDrive, rearWheelDrive, allWheelDrive }

[RequireComponent(typeof(Rigidbody))]

public class Car_Controller : MonoBehaviour
{

    public CarSFX carSFX { get; private set; }

    public bool carActive {  get; private set; }
    private PlayerControls controls;

    [SerializeField] private LayerMask whatisGround;

    public Rigidbody rb {  get; private set; }
    private float moveInput;
    private float steerInput;

    public float speed;


    [Header("CarSettings")]
    [SerializeField] private driveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350,1000)]
    [SerializeField] private float carMass = 400;
    [Range(.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1;
    [Range(.5f, 2f)]
    [SerializeField] private float rearWheelTraction = 1;
    [Range(20,80)]
    [SerializeField] private float wheelsMass = 30;


    [Range(30, 60)]
    [SerializeField] private float turnSensitivity = 30;

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed;
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7;
    [Range(.5f, 10)]
    [SerializeField] private float accelerationSpeed = 2;
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = .5f;
    [Range(0, 1)]
    [SerializeField] private float rearDriftFactor = .5f;
    [SerializeField] private float driftDuration = 1;
    private bool isDrifting;
    private float driftTimer;
    private bool canEmitTrails = true;

    [Header("Brake Settings")]
    [Range(0, 10)]
    [SerializeField] private float frontBrakesSensivity = 5;
    [Range(0, 10)]
    [SerializeField] private float rearBrakesSensivity = 5;
    [Range(4000, 6000)]
    [SerializeField] private float brakePower = 5000;
    private bool isBraking;


    private Car_Wheel[] wheels;
    private UI ui;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        carSFX = GetComponent<CarSFX>();

        ui = UI.instance;
        controls = ControlsManager.instance.controls;
        //ControlsManager.instance.SwitchToCarControls();



      

        AssignInputEvents();
        SetupDefaultValues();
        ActivateCar(false);
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.wcd.mass = wheelsMass;

            if (wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffness(frontWheelTraction);

            if(wheel.axelType == AxelType.Rear)
                wheel.SetDefaultStiffness(rearWheelTraction);

        }
    }
    private void Update()
    {
        if(carActive == false)
            return;
       

        speed = rb.velocity.magnitude;
        ui.inGameUI.UpdateSpeedText(Mathf.RoundToInt(speed * 10) + "KM/H");



        speed = rb.velocity.magnitude;
        driftTimer -= Time.deltaTime;

        if (driftTimer < 0)
            isDrifting = false;


    }
    private void FixedUpdate()
    {
        
        if(carActive == false)
            return;

        AnimateWheels();


        ApplyTrailsOnTheGround();
        ApplyBrakes();
        ApplyDrive();
        ApplySteering();
        ApplySpeedLimit();

        if (isDrifting)
            ApplyDrift();
        else
            StopDrift();


    }

    

    private void ApplyDrive()
    {

        currentSpeed = moveInput * accelerationSpeed * Time.deltaTime;

        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (driveType == driveType.frontWheelDrive)
            {
                if (wheel.axelType == AxelType.Front)
                    wheel.wcd.motorTorque = motorTorqueValue;


            }
            else if (driveType == driveType.rearWheelDrive)
            {
                if (wheel.axelType == AxelType.Rear)
                    wheel.wcd.motorTorque = motorTorqueValue;
            }
            else
            {
                wheel.wcd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensitivity;
                wheel.wcd.steerAngle = Mathf.Lerp(wheel.wcd.steerAngle, targetSteerAngle, .5f);
            }
        }
    }



    private void ApplyBrakes()
    {

        foreach(var wheel in wheels)
        {
            bool frontBrakes = wheel.axelType == AxelType.Front;
            float brakeSensitivity = frontBrakes ? frontBrakesSensivity : rearBrakesSensivity;

            float newBrakeTorque = brakePower * brakeSensitivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0;



            
                wheel.wcd.brakeTorque = currentBrakeTorque;
        }

        
        
    }

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : rearDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.wcd.sidewaysFriction;

            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.wcd.sidewaysFriction = sidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStifness();
        }
    }
    private void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.wcd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }



    }

    private void ApplyTrailsOnTheGround()
    {

        if(canEmitTrails==false)
            return; 


        foreach (var wheel in wheels)
        {
            WheelHit hit;

            if (wheel.wcd.GetGroundHit(out hit))
            {
                if (whatisGround == (whatisGround | (1 << hit.collider.gameObject.layer)))
                {
                    wheel.trailRenderer.emitting = true;
                }
                else
                {
                    wheel.trailRenderer.emitting = false;
                }
            }
            else
            {
                wheel.trailRenderer.emitting = false;
            }
        }
    }

    public void ActivateCar(bool activate)
    {
        carActive = activate;

        if(carSFX != null)
            carSFX.ActivateCarSFX(activate);
    }




    public void DestroyCar()
    {
        canEmitTrails = false;

        foreach (var wheel in wheels)
        {
            wheel.trailRenderer.emitting = false;
        }

        rb.drag = 1;
        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = .9f;
        rearDriftFactor = .9f;

    }
    private void AssignInputEvents()
    {
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            moveInput = input.y;
            steerInput = input.x;
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;


        
        controls.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfCar();

        controls.Car.UI_Pause.performed += ctx => UI.instance.PauseSwitch();
    
    }

    [ContextMenu("FocusCameraAndEnable")]
    public void TestThisCar()
    {
        ActivateCar(true);
        CameraManager.instance.ChangeCameraTarget(transform, 12);
    }


}
