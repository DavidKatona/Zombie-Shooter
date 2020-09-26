using UnityEngine;

namespace Assets.Scripts.CollectibleSystem
{
    public class CollectiblePicker : MonoBehaviour
    {
        #region MONOBEHAVIOUR

        /// <summary>
        /// Checks if the colliding game object implements the ICollectible interface, and if so, calls its Collect() method.
        /// </summary>
        /// <param name="other"></param>

        private void OnCollisionEnter(Collision other)
        {
            var collectible = other.gameObject.GetComponent<ICollectible>();

            if (collectible != null)
            {
                collectible.Collect();
            }
        }

        #endregion
    }
}
