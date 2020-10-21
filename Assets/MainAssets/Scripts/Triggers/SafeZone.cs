using Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class SafeZone : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The event that should be invoked when this safezone is reached.")]
        [SerializeField] private GameEvent _onSafeZoneReached = null;

        [Tooltip("The first person character game object that will be disabled when reaching the safezone.")]
        [SerializeField] private GameObject _firstPersonCharacter = null;

        [Tooltip("The third person character gam eobject that will be enabled when reaching the safezone.")]
        [SerializeField] private GameObject _thidPersonCharacter = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The game event that's raised when the safe zone is reached.
        /// </summary>
        
        public GameEvent OnSafeZoneReached { get { return _onSafeZoneReached; } }

        /// <summary>
        /// The first person character game object that will be disabled upon reaching the safezone.
        /// </summary>

        public GameObject FirstPersonCharacter { get { return _firstPersonCharacter; } }

        /// <summary>
        /// The third person character game object that will be enabled upon reaching the safezone.
        /// </summary>

        public GameObject ThirdPersonCharacter { get { return _thidPersonCharacter; } }

        #endregion

        #region MONOBEHAVIOUR

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                // Disable player.

                FirstPersonCharacter.SetActive(false);
                ThirdPersonCharacter.SetActive(true);

                // Raise the OnPlayerDied event.

                OnSafeZoneReached?.Raise();
            }
        }

        #endregion
    }
}
