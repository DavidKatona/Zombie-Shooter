using UnityEngine;

namespace Assets.Scripts.CollectibleSystem.CollectibleTypes
{
    public class AmmoBox : BaseCollectible
    {
        #region EDITOR EXPOSED FIELDS



        #endregion

        #region METHODS

        protected override void ApplyEffects()
        {
            Debug.Log("Called from AmmoBox Type.");
        }

        #endregion
    }
}
