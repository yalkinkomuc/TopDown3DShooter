using UnityEngine;

public class CarSFX : MonoBehaviour
{
    private Car_Controller car;

    [SerializeField] private float engineVolume = 0.07f;

    [SerializeField] private AudioSource engineStartSFX;
    [SerializeField] private AudioSource engineWorkSFX;
    [SerializeField] private AudioSource engineStopSFX;

    
    private float maxSpeed = 10;

    public float minPitch = .75f;
    public float maxPitch = 1.5f;

    private bool allowCarSoundsEnabled;

    private void Start()
    {
        car = GetComponent<Car_Controller>();
        Invoke(nameof(AllowCarSounds), 1f);
    }

    private void Update()
    {
        UpdateEngineSound();
    }

    private void UpdateEngineSound()
    {
        float currentSpeed = car.speed;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed/maxSpeed);

        engineWorkSFX.pitch = pitch;
    }

    public void ActivateCarSFX(bool activate)
    {

        if(allowCarSoundsEnabled == false)
            return;

        if (activate)
        {
            engineStartSFX.Play();
            AudioManager.instance.SFXWithDelayAndFade(engineWorkSFX, true, engineVolume, 1);
        }
        else
        {
            AudioManager.instance.SFXWithDelayAndFade(engineWorkSFX, false, 0f, .25f);

            engineStopSFX.Play();
        }
    }

    private void AllowCarSounds() => allowCarSoundsEnabled = true;
}
