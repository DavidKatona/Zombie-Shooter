using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [Header("Audio Clips")]
    [Tooltip("The audio clip that plays when the player's weapon is being fired.")]
    [SerializeField] private AudioClip _shotSound;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The audio clip that plays when the player shoots.
    /// </summary>

    public AudioClip ShotSound
    {
        get { return _shotSound; }
        private set { _shotSound = value; }
    }

    /// <summary>
    /// The cached audio source component attached to the game object.
    /// </summary>

    public AudioSource CachedAudioSource { get; private set; }

    #endregion

    #region METHODS

    public void PlayShootingSound()
    {
        CachedAudioSource.clip = ShotSound;
        CachedAudioSource.Play();
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Awake()
    {
        // Cache components.

        CachedAudioSource = GetComponent<AudioSource>();
    }
    #endregion
}