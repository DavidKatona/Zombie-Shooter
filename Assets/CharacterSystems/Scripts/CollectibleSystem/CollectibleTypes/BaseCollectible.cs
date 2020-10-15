using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.CollectibleSystem.CollectibleTypes
{
    /// <summary>
    /// The abstract base class for collectible types.
    /// </summary>
    public abstract class BaseCollectible : MonoBehaviour, ICollectible
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Audio Files")]
        [Tooltip("The audio file(s) that play when this collectible is picked up.")]
        [SerializeField] private AudioClip _pickupSound = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The cached transform component attached to the collectible object.
        /// </summary>

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// The audio clip that plays when this collectible is picked up.
        /// </summary>

        public AudioClip PickupSound { get { return _pickupSound; } }

        #endregion

        #region METHODS

        /// <summary>
        /// The implementation of Collect() from ICollectible. Specific for this type.
        /// </summary>

        public void Collect()
        {
            Debug.Log($"{gameObject.name} of type {GetType().Name} has been interacted with.");
            ApplyEffects();
        }

        /// <summary>
        /// Plays the pickup sound that's defined for this or derived types.
        /// </summary>

        protected void PlayPickupSound()
        {
            AudioSource.PlayClipAtPoint(PickupSound, CachedTransform.position);
        }

        /// <summary>
        /// Applies effects based on this collectible type's implementation. Override this in derived types to alter the default behaviour.
        /// </summary>

        protected virtual void ApplyEffects()
        {
            Debug.Log("Called from BaseType.");
        }

        #endregion

        #region MONOBEHAVIOUR

        protected virtual void Awake()
        {
            CachedTransform = GetComponent<Transform>();
        }

        #endregion
    }
}