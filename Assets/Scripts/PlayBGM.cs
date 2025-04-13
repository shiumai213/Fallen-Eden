using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    public AudioClip bgmClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
