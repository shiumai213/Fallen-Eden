using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource = default;
    [SerializeField] private AudioClip[] bgmclips;

    //[SerializeField] private AudioSource SoundEffect = default;
    //[SerializeField] private AudioClip[] SFXClips;

    public enum BGM
    {
        HNBNJNJ,
        Centray,
        QuestionNo9,
    }

    public void PlayBGM(BGM bgm)
    {
        if (bgmAudioSource.clip != bgmclips[(int)bgm])
        {
            bgmAudioSource.clip = bgmclips[(int)(bgm)];
            bgmAudioSource.Play();
        }
    }
}
