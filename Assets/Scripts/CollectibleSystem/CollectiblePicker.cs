using UnityEngine;

namespace Assets.Scripts.CollectibleSystem
{
    public class CollectiblePicker : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var collectible = other.GetComponentInChildren<ICollectible>();

            if (collectible != null)
            {
                collectible.Collect();
            }
        }
    }
}
