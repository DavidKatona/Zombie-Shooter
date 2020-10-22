using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD
{
    public class AmmoReserve : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The ammo reserve scriptable object from which this component reads data.")]
        [SerializeField] private IntVariable _ammoReserveObject = null;

        [Tooltip("The text component which is modified when the ammo reserve is changed.")]
        [SerializeField] private Text _ammoReserveText = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The ammo reserve scriptable object from which this component reads data.
        /// </summary>

        public IntVariable AmmoReserveObject { get { return _ammoReserveObject; } }

        /// <summary>
        /// The text UI element which is modified when the amount of bullets are changed in the ammo reserve.
        /// </summary>

        public Text AmmoReserveText { get { return _ammoReserveText; } }

        #endregion

        #region METHODS

        public void ChangeAmmoAmount()
        {
            if (AmmoReserveObject == null || AmmoReserveText == null)
                return;

            AmmoReserveText.text = AmmoReserveObject.RuntimeValue.ToString();
        }

        private void InitializeAmmoReserve()
        {
            if (AmmoReserveObject != null && AmmoReserveText != null)
            {
                AmmoReserveText.text = AmmoReserveObject.RuntimeValue.ToString();
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            InitializeAmmoReserve();
        }

        #endregion
    }
}