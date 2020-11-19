using Assets.Scripts.ObjectPooling;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticle : MonoBehaviour, IPooledObject
{
    #region EDITOR EXPOSED FIELDS

    [Header("Options")]
    [Tooltip("How long the particle should live before it's disabled and prepared for being pooled again.")]
    [SerializeField] private float _particleLifetime = 1f;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The cached particle system component attached to this game object.
    /// </summary>

    public ParticleSystem CachedParticleSystem { get; private set; }

    /// <summary>
    /// How long the particle should live before it's disabled and prepared for being pooled again.
    /// </summary>

    public float ParticleLifetime { get { return _particleLifetime; } }

    #endregion

    #region METHODS

    public void OnObjectSpawned()
    {
        RestartParticle();
    }

    /// <summary>
    /// Restarts the particle system by clearing it first.
    /// </summary>

    private void RestartParticle()
    {
        CachedParticleSystem.Clear();
        CachedParticleSystem.Play();
    }

    /// <summary>
    /// Disables the particle system after a delay.
    /// </summary>
    /// <param name="delay">The delay provided in seconds.</param>
    /// <returns></returns>

    private IEnumerator DisableParticle(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Awake()
    {
        // Cache and initialize components.

        CachedParticleSystem = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(DisableParticle(ParticleLifetime));
    }

    #endregion
}