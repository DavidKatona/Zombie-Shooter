using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class ThirdPersonCharacterAnimator : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The first person character's transform component. This will determine where the game object to which this script is attached, should activate.")]
        [SerializeField] private Transform _firstPersonTransform = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The cached transform component attached to this game object.
        /// </summary>

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// The cached animator component attached to this game object.
        /// </summary>

        public Animator CachedAnimator { get; private set; }

        /// <summary>
        /// The cached transform component attached to the first person character in the scene.
        /// </summary>

        public Transform CachedFPSCharacterTransform { get { return _firstPersonTransform; } }

        #endregion

        #region METHODS

        /// <summary>
        /// Event response that's triggered when the OnPlayerDied event is invoked.
        /// </summary>

        public void OnPlayerDied_PlayDeathAnimation()
        {
            // Position the third person model to the location of the first person character.

            CachedTransform.position = new Vector3(CachedFPSCharacterTransform.position.x,
                Terrain.activeTerrain.SampleHeight(CachedFPSCharacterTransform.position),
                CachedFPSCharacterTransform.position.z);

            CachedTransform.rotation = CachedFPSCharacterTransform.rotation;

            // Fire animation trigger.

            CachedAnimator.SetTrigger("Death");
        }

        /// <summary>
        /// Event response that's triggered when the OnLevelCompleted event is invoked.
        /// </summary>

        public void OnLevelCompleted_PlayVictoryAnimation()
        {
            // Position the third person model to the location of the first person character.

            CachedTransform.position = new Vector3(CachedFPSCharacterTransform.position.x,
                Terrain.activeTerrain.SampleHeight(CachedFPSCharacterTransform.position),
                CachedFPSCharacterTransform.position.z);

            // Fire animation trigger.

            CachedAnimator.SetTrigger("Dance");
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedTransform = GetComponent<Transform>();
            CachedAnimator = GetComponent<Animator>();
        }

        #endregion
    }
}