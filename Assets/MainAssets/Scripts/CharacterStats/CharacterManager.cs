using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Damageables.Common;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.CharacterStats
{
    public class CharacterManager : MonoBehaviour, IDamageable
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Scriptable Components")]
        [Tooltip("The scriptable object that holds information about the character's health.")]
        [SerializeField] private IntVariable _healthObject = null;

        [Tooltip("The scriptable object that holds the listeners for the OnPlayerDied event.")]
        [SerializeField] private GameEvent _onPlayerDied = null;

        [Header("Other Components")]
        [Tooltip("The game object that will be activated when the player views the character from a 3rd person perspective.")]
        [SerializeField] private GameObject _thirdPersonCharacter = null;

        [Header("Options")]
        [Tooltip("The audio clip that plays when the character takes damage.")]
        [SerializeField] private AudioClip[] _onDamagedAudioClips = null;

        [Tooltip("The audio clips that reflect the impact of an attack dealt on the player.")]
        [SerializeField] private AudioClip[] _onDamagedSplashClips = null;

        [Tooltip("The aduio clip that plays when the character dies.")]
        [SerializeField] private AudioClip _onDeathAudioClip = null;

        #endregion

        #region FIELDS

        private bool _isRecentlyDamaged;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The cached transform component attached to this game object.
        /// </summary>

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// The cached audio source component attached to this game object.
        /// </summary>

        public AudioSource CachedAudioSource { get; private set; }

        /// <summary>
        /// The "Health" scriptable object that's attached to this game object.
        /// NOTE: read-only. Should only be set from the editor.
        /// </summary>

        public IntVariable HealthObject { get { return _healthObject; } }

        /// <summary>
        /// The game object that will be activated when the player views the character from a 3rd person perspective
        /// </summary>

        public GameObject ThirdPersonCharacter { get { return _thirdPersonCharacter; } }

        /// <summary>
        /// The scriptable object that holds the listeners for the OnPlayerDied event.
        /// </summary>

        public GameEvent OnPlayerDied { get { return _onPlayerDied; } }

        /// <summary>
        /// The audio clip that plays when the character takes damage.
        /// </summary>

        public AudioClip[] OnDamagedAudioClips { get { return _onDamagedAudioClips; } }

        /// <summary>
        /// The audio clips that reflect the impact of an attack dealt on the player.
        /// </summary>

        public AudioClip[] OnDamagedSplashClips { get { return _onDamagedSplashClips; } }

        /// <summary>
        /// The audio clip that plays when the character dies.
        /// </summary>

        public AudioClip OnDeathAudioClip { get { return _onDeathAudioClip; } }

        #endregion

        #region METHODS

        /// <summary>
        /// The implementation of TakeDamage from IDamageable. Reduces the character's health by a given amount.
        /// </summary>
        /// <param name="amount"></param>

        public void TakeDamage(int amount)
        {
            if (HealthObject == null)
                return;

            if (HealthObject.RuntimeValue > 0)
            {
                // Lower health and play splash sound.

                HealthObject.RuntimeValue -= amount;
                CachedAudioSource.PlayOneShot(GetRandomAudioClip(OnDamagedSplashClips), 0.5f);

                if (HealthObject.RuntimeValue <= 0)
                {
                    Die();
                    return;
                }

                // Play sound effect.

                if (!_isRecentlyDamaged)
                {
                    _isRecentlyDamaged = true;
                    StartCoroutine(ResetDamagedState(0.8f));

                    CachedAudioSource.PlayOneShot(GetRandomAudioClip(OnDamagedAudioClips));
                }
            }
        }

        private IEnumerator ResetDamagedState(float delay)
        {
            yield return new WaitForSeconds(delay);
            _isRecentlyDamaged = false;
        }

        private void Die()
        {
            // Print to console.

            Debug.Log($"The character died (health: {HealthObject.RuntimeValue}).");

            // Play sound effect.

            AudioSource.PlayClipAtPoint(GetRandomAudioClip(OnDamagedSplashClips), CachedTransform.position);
            AudioSource.PlayClipAtPoint(OnDeathAudioClip, CachedTransform.position);

            // Disable player.

            gameObject.SetActive(false);
            ThirdPersonCharacter.SetActive(true);

            // Raise the OnPlayerDied event.

            OnPlayerDied?.Raise();
        }

        private AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            var randomIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomIndex];
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedTransform = GetComponent<Transform>();
            CachedAudioSource = GetComponent<AudioSource>();
        }

        #endregion
    }
}