using UnityEngine;

public class AudioSourceFinder : MonoBehaviour
{
    void Update()
    {
        // Sahnedeki tüm AudioSource bileşenlerini bul
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
        {
            // Eğer ses kaynağı çalıyorsa, bilgilerini göster
            if (audioSource.isPlaying)
            {
                Debug.Log("Çalan Ses Kaynağı: " + audioSource.gameObject.name);
            }
        }
    }
}
