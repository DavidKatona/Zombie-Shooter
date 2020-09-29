using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Extensions;
using Assets.Scripts.Damageables.Common;
using UnityEngine;

namespace Assets.Scripts.CharacterStats
{
    public class CharacterManager : MonoBehaviour, IDamageable
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Scriptable Components")]
        [Tooltip("The scriptable object that holds information about the character's health.")]
        [SerializeField] private IntVariable _healthObject = null;

        [Header("Options")]
        [Tooltip("The audio clip that plays when the character takes damage.")]
        [SerializeField] private AudioClip[] _onDamagedAudioClips = null;

        [Tooltip("The aduio clip that plays when the character dies.")]
        [SerializeField] private AudioClip _onDeathAudioClip = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The "Health" scriptable object that's attached to this game object.
        /// NOTE: read-only. Should only be set from the editor.
        /// </summary>

        public IntVariable HealthObject { get { return _healthObject; } }

        /// <summary>
        /// The audio clip that plays when the character takes damage.
        /// </summary>

        public AudioClip[] OnDamagedAudioClips { get { return _onDamagedAudioClips; } }

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
                // Lower health.

                HealthObject.RuntimeValue -= amount;

                if (HealthObject.RuntimeValue <= 0)
                {
                    Die();
                    return;
                }

                // Play sound effect.

                AudioSource audioSource = new AudioSource();
                audioSource.InstantiateAudioSource(GetRandomAudioClip(OnDamagedAudioClips), transform.position);
            }
        }

        private void Die()
        {
            // Print to console.

            Debug.Log($"The character died (health: {HealthObject.RuntimeValue}).");

            // Play sound effect.

            AudioSource audioSource = new AudioSource();
            audioSource.InstantiateAudioSource(OnDeathAudioClip, transform.position);
        }

        private AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            var randomIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomIndex];
        }

        #endregion
    }
}
