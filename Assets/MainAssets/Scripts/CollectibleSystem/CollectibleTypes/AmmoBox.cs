using Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.CollectibleSystem.CollectibleTypes
{
    public class AmmoBox : BaseCollectible
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The scriptable object that this ammo box should interact with.")]
        [SerializeField] private IntVariable _targetObject = null;

        [Header("Options")]
        [Tooltip("The amonut of ammunition restored upon pickup.")]
        [SerializeField] private int _ammoRestored = 20;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The scriptable object that's attached to this game object and is interacted with (read-only).
        /// </summary>

        public IntVariable TargetObject { get { return _targetObject; } }

        /// <summary>
        /// The amount of ammunition this collectible type restores on pickup.
        /// </summary>

        public int AmmoRestored { get { return _ammoRestored; } }

        #endregion

        #region METHODS

        protected override void ApplyEffects()
        {
            var currentAmmunition = TargetObject.RuntimeValue;
            var maximumAmmunition = TargetObject.MaximumValue;

            if (currentAmmunition < maximumAmmunition)
            {
                TargetObject.RuntimeValue += AmmoRestored;

                PlayPickupSound();
                Destroy(gameObject);
            }
        }

        #endregion
    }
}