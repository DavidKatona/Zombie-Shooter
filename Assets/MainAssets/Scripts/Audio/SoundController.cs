using UnityEngine;

namespace Assets.Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("The footstep audio clips that will be randomly played when the character moves")]
        [SerializeField] private AudioClip[] _footstepSounds;

        [Tooltip("The audio clip that plays when the character jumps.")]
        [SerializeField] private AudioClip _jumpSound;

        [Tooltip("The audio clip that plays when the character hits the ground.")]
        [SerializeField] private AudioClip _landingSound;

        #endregion

        #region FIELDS

        private bool _areFootstepsInvoked;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The array of audio clips from which a random clip is picked when the character moves around and is grounded.
        /// </summary>

        public AudioClip[] FootstepSounds
        {
            get { return _footstepSounds; }
            private set { _footstepSounds = value; }
        }

        /// <summary>
        /// The audio clip that plays when the character jumps.
        /// </summary>

        public AudioClip JumpSound
        {
            get { return _jumpSound; }
            set { _jumpSound = value; }
        }

        /// <summary>
        /// The audio clip that plays when the character hits the ground.
        /// </summary>

        public AudioClip LandingSound
        {
            get { return _landingSound; }
            set { _landingSound = value; }
        }

        /// <summary>
        /// The main cached audio source component attached to the game object.
        /// </summary>

        public AudioSource CachedAudioSource { get; private set; }


        /// <summary>
        /// The cached animator component attached to the game object.
        /// </summary>
        public Animator CachedAnimator { get; private set; }

        /// <summary>
        /// The audio source component that will play all footstep related sounds.
        /// </summary>

        public AudioSource FootstepsAudioSource { get; private set; } = null;

        #endregion

        #region METHODS

        public void PlayFootstepAudio()
        {
            GameObject audioSourceHolder = new GameObject("Footstep sound");
            audioSourceHolder.transform.position = transform.position;
            AudioSource audioSource = audioSourceHolder.AddComponent<AudioSource>();

            int randomClip = Random.Range(0, FootstepSounds.Length);
            audioSource.clip = FootstepSounds[randomClip];
            audioSource.Play();
            Destroy(audioSourceHolder, audioSource.clip.length);
        }

        public void PlayJumpingSound()
        {
            GameObject audioSourceHolder = new GameObject("Jumping sound");
            audioSourceHolder.transform.position = transform.position;
            AudioSource audioSource = audioSourceHolder.AddComponent<AudioSource>();

            audioSource.clip = JumpSound;
            audioSource.Play();
            Destroy(audioSourceHolder, JumpSound.length);
        }

        public void PlayLandingSound()
        {
            GameObject audioSourceHolder = new GameObject("Landing sound");
            audioSourceHolder.transform.position = transform.position;
            AudioSource audioSource = audioSourceHolder.AddComponent<AudioSource>();

            audioSource.clip = LandingSound;
            audioSource.Play();
            Destroy(audioSourceHolder, LandingSound.length);
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedAudioSource = GetComponent<AudioSource>();
            CachedAnimator = GetComponent<Animator>();
        }

        #endregion
    }
}
