
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Transform playerBody;

    public PlayerControls controls { get; private set; }
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerWeaponController weaponController { get; private set; }
    public WeaponVisualController visualController { get; private set; }
    public PlayerInteraction playerInteraction { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public PlayerHealth health { get; private set; }
    public PlayerSoundFX sound { get; private set; }
   
    public Animator animator { get; private set; }

    public bool controlsEnabled { get; private set; }
    private void Awake()
    {
        

        ragdoll = GetComponent<Ragdoll>();
        animator = GetComponentInChildren<Animator>();    
        controls = new PlayerControls();
        aim = GetComponent<PlayerAim>();
        sound = GetComponent<PlayerSoundFX>();
        movement = GetComponent<PlayerMovement>();
        weaponController = GetComponent<PlayerWeaponController>();
        visualController = GetComponent<WeaponVisualController>();
        playerInteraction = GetComponent<PlayerInteraction>();
        health = GetComponent<PlayerHealth>();
        controls = ControlsManager.instance.controls;
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Character.UI_MissionToolTipSwitch.performed += ctx => UI.instance.inGameUI.SwitchMissionToolTip();

        controls.Character.UI_Pause.performed += ctx => UI.instance.PauseSwitch();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void SetControlsEnabled (bool enabled)
    {
        controlsEnabled = enabled;
        ragdoll.CollidersActive(enabled);
        aim.EnableAimLaser(enabled);
    }
}
