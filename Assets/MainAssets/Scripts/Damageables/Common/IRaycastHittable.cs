using UnityEngine;

namespace Assets.MainAssets.Scripts.Damageables.Common
{
    interface IRaycastHittable
    {
        void RegisterHit(RaycastHit raycastHit, Vector3 raycastOrigin);
    }
}