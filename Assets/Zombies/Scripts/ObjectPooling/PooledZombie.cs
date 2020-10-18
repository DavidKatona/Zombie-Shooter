using Assets.Zombies.Scripts;
using Assets.Scripts.ObjectPooling;
using UnityEngine;

public class PooledZombie : MonoBehaviour, IPooledObject
{
    #region EDITOR EXPOSED FIELDS

    [Header("Components")]
    [Tooltip("The animated model with the ZombieController script attached to it.")]
    [SerializeField] private GameObject _animatedModel = null;

    [Tooltip("The ZombieController component attached to the animated model.")]
    [SerializeField] private ZombieController _zombieController = null;

    [Tooltip("The ragdoll object childed to this game object.")]
    [SerializeField] private GameObject _ragdollObject = null;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The animated model with the ZombieController script attached to it.
    /// </summary>

    public GameObject AnimatedModel { get { return _animatedModel; } }

    /// <summary>
    /// The ZombieController component attached to the animated model.
    /// </summary>

    public ZombieController ZombieController { get { return _zombieController; } }

    /// <summary>
    /// The ragdoll object childed to this game object.
    /// </summary>

    public GameObject RagdollObject { get { return _ragdollObject; } }

    #endregion

    #region METHODS

    public void OnObjectSpawned()
    {
        ResetZombie();
    }

    private void ResetZombie()
    {
        AnimatedModel.SetActive(true);
        RagdollObject.SetActive(false);
        ZombieController.SwitchState(ZombieState.Idle);
    }

    #endregion

}