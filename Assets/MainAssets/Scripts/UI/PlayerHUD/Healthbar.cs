using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD
{
    public class Healthbar : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The scriptable object from which the healthbar reads data.")]
        [SerializeField] private IntVariable _healthObject = null;

        [Tooltip("The UI element that will be affected by a change in health.")]
        [SerializeField] private Slider _healthSlider = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The scriptable object from which the healthbar reads data.
        /// </summary>

        public IntVariable HealthObject { get { return _healthObject; } }

        /// <summary>
        /// The UI element that will be affected by a change in health.
        /// </summary>

        public Slider HealthSlider { get { return _healthSlider; } }

        #endregion

        #region METHODS

        public void ChangeHealthbarFillRate()
        {
            if (HealthObject == null || HealthSlider == null)
                return;

            HealthSlider.value = HealthObject.RuntimeValue;
        }

        private void InitializeHealthbar()
        {
            if (HealthObject != null && HealthSlider != null)
            {
                HealthSlider.maxValue = HealthObject.MaximumValue;
                HealthSlider.value = HealthObject.RuntimeValue;
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        public void Awake()
        {
            // Cache and initialize components.

            InitializeHealthbar();
        }

        #endregion
    }
}
