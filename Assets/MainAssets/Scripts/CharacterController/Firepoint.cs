﻿using UnityEngine;

namespace Assets.CharacterSystems.Scripts.CharacterController
{
    public class Firepoint : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            if (!gameObject.activeInHierarchy)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Camera.main.transform.forward * 200);
            Gizmos.DrawWireSphere(transform.position, 0.05f);
        }
    }
}