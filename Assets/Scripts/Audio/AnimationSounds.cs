using Assets.Scripts.Extensions;
using UnityEngine;

public class AnimationSounds : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [Header("Audio Files")]
    [SerializeField] private AudioClip[] _footstepClips;

    #endregion

    #region PROPERTIES

    public AudioClip[] FootstepClips { get { return _footstepClips; } }

    #endregion

    #region METHODS

    private void PlayFootStepSound()
    {
        AudioSource audioSource = new AudioSource();
        audioSource.InstantiateAudioSource(GetRandomClip(FootstepClips), transform.position);
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        var randomIndex = Random.Range(0, audioClips.Length);
        return audioClips[randomIndex];
    }

    #endregion
}
