using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    private PlayerControls playerControls;
    private CharacterController characterController;

    [Header("MovementInfo")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    private float speed;
    private Vector3 directionMovement;
    float gravityForce;
    public Vector2 moveInput { get; private set; }
    private bool isRunning;
    [SerializeField] private float turnSpeed;


    private AudioSource runSFX;
    

    private Animator anim;





    private void Start()
    {
        player = GetComponent<Player>();


        runSFX = player.sound.runSFX;
        //Invoke(nameof(AllowFootsteps), 1f);

        characterController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        speed = walkSpeed;
        AssignInputEvents();


    }

    private void Update()
    {
        if (player.health.isDead)
            return;

        ApplyMovement();

        ApplyRotation();

        AnimationControllers();


    }


    private void AnimationControllers()
    {
        float xVelocity = Vector3.Dot(directionMovement.normalized, transform.right);
        float zVelocity = Vector3.Dot(directionMovement.normalized, transform.forward);

        anim.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        anim.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunAnimation = isRunning && directionMovement.magnitude > 0 ;
        anim.SetBool("isRunning", playRunAnimation);

    }



    private void ApplyRotation()
    {
        Vector3 lookingDirection = player.aim.getMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        directionMovement = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();


        if (directionMovement.magnitude > 0)
        {
            PlayFootstepsSFX();

            characterController.Move(directionMovement * Time.deltaTime * speed);
        }



    }


    private void PlayFootstepsSFX()
    {
        //if (canPlayFootsteps == false)
        //    return;

        if (isRunning)
        {
            if (runSFX.isPlaying == false)
                runSFX.Play();
        }

    }
    private void StopFootstepsSFX()
    {



        runSFX.Stop();


    }

    



    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            gravityForce -= 9.81f * Time.deltaTime;
            directionMovement.y = gravityForce;


        }

        else
        {
            directionMovement.y = -.5f;

        }
    }

    #region New Input System

    private void AssignInputEvents()
    {
        playerControls = player.controls;

        playerControls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        playerControls.Character.Movement.canceled += context =>
        {
            StopFootstepsSFX();           
            moveInput = Vector2.zero;
        };


        playerControls.Character.Run.performed += context =>
        {

            speed = runSpeed;
            isRunning = true;
        };

        playerControls.Character.Run.canceled += context =>

        {
            speed = walkSpeed;
            isRunning = false;
        };

        
        

        

    }



    #endregion

}
