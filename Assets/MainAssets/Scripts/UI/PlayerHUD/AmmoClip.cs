using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD
{
    public class AmmoClip : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The ammo clip scriptable object from which this component reads data.")]
        [SerializeField] private IntVariable _ammoClipObject = null;

        [Tooltip("The text component which is modified when the ammo clip is changed.")]
        [SerializeField] private Text _ammoClipText = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The ammo clip scriptable object from which this component reads data.
        /// </summary>

        public IntVariable AmmoClipObject { get { return _ammoClipObject; } }

        /// <summary>
        /// The text UI element which is modified when the amount of bullets are changed in the ammo clip.
        /// </summary>

        public Text AmmoClipText { get { return _ammoClipText; } }

        #endregion

        #region METHODS

        public void ChangeAmmoAmount()
        {
            if (AmmoClipObject == null || AmmoClipText == null)
                return;

            AmmoClipText.text = AmmoClipObject.RuntimeValue.ToString();
        }

        private void InitializeAmmoClip()
        {
            if (AmmoClipObject != null && AmmoClipText != null)
            {
                AmmoClipText.text = AmmoClipObject.RuntimeValue.ToString();
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            InitializeAmmoClip();
        }

        #endregion
    }
}
