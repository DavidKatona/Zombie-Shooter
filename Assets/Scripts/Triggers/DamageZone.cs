using Assets.Scripts.Damageables.Common;
using UnityEngine;

namespace Assets.Scripts.Triggers
{
    public class DamageZone : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Zone Parameters")]
        [Tooltip("The amount of damage this zone deals on collision.")]
        [SerializeField] private int _damageDone = 10;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The amount of damage this zone deals to IDamageable objects on collision.
        /// </summary>

        public int DamageDone { get { return _damageDone; } }

        #endregion

        #region MONOBEHAVIOUR

        private void OnCollisionEnter(Collision collision)
        {
            var damageable = collision.gameObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(DamageDone);
            }
        }

        #endregion
    }
}
