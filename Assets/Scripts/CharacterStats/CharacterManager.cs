using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Damageables.Common;
using Assets.Scripts.Extensions;
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
                // Play sound effect.

                AudioSource audioSource = new AudioSource();
                audioSource.InstantiateAudioSource(GetRandomAudioClip(OnDamagedAudioClips), transform.position);

                // Lower health.

                HealthObject.RuntimeValue -= amount;
            }
        }

        private AudioClip GetRandomAudioClip(AudioClip[] audioClips)
        {
            var randomIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomIndex];
        }

        #endregion
    }
}
