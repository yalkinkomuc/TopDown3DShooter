using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private bool playBGM;

    private int bgmIndex;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        PlayBGM(2);
    }
    private void Update()
    {
        if (playBGM == false && BgmIsPlaying())
            StopAllBGM();

        if (playBGM && bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();


    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = .85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        float pitch = Random.Range(minPitch, maxPitch);
        sfx.pitch = pitch;

        sfx.Play();
    }

    public void SFXWithDelayAndFade(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        StartCoroutine(SFXDelayAndFadeCO(source, play, targetVolume, delay,fadeDuration ));
    }

    public void PlayBGM(int index)
    {
        StopAllBGM();

        bgmIndex = index;
        bgm[index].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    [ContextMenu("PlayRandomMusic")]
    public void PlayRandomBGM()
    {
        StopAllBGM();
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }

        return false;
    }

    private IEnumerator SFXDelayAndFadeCO(AudioSource source, bool play, float targetVolume, float delay = 0, float fadeDuration = 1)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = play ? 0 : source.volume;
        float endVolume = play ? targetVolume : 0;
        float elapsed = 0;

        if (play)
        {
            source.volume = 0;
            source.Play();
        }

        // fade in fade out süresi

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration);
            yield return null;
        }

        source.volume = endVolume;

        if (play == false)
            source.Stop();
    }

}
