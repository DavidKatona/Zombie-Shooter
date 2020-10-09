using Assets.Zombies.Scripts.ObjectPooling;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Zombies.Scripts
{

    // Consider turning the spawner into a zombie pooler instead so instantiation will not cause performance hits at runtime.
    // ToDo: Research on object pooling, refactor this into a zombie pooler.
    // Note: Ragdolls should be childed to the actual zombie prefabs as deactivated objects (so they'll be part of the pooling system too), and simply turn them on when they die and disable the animated models.

    public class Spawner : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Spawner Settings")]
        [Tooltip("The game object that will be spawned by this spawner.")]
        [SerializeField] private GameObject _prefabToSpawn = null;

        [Tooltip("The number of game objects to spawn.")]
        [SerializeField] private int _quantity = 0;

        [Tooltip("The amount of time that should pass between new enemies being spawned. Should never be 0 as that would decrease performance.")]
        [SerializeField] private float _delayBetweenSpawns = 1f;

        [Tooltip("The radius of the sphere in which game objects will randomly spawn.")]
        [SerializeField] private float _spawnRadius = 10.0f;

        [Tooltip("The collider that should be used as a trigger to start the spawning sequence. Set to null to spawn at start.")]
        [SerializeField] private Collider _spawnTrigger = null;

        [Header("Parenting Settings")]
        [Space(10)]
        [Tooltip("The game object under which all spawned game objects will be grouped together. Set to null to disable parenting.")]
        [SerializeField] private GameObject _parentUnder = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The cached transform component attached to this game object.
        /// </summary>

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// The cached object pooler instance.
        /// </summary>

        public ObjectPooler CachedObjectPooler { get; private set; }

        /// <summary>
        /// The game object (prefab) that this spawner will instantiate.
        /// </summary>

        public GameObject PrefabToSpawn { get { return _prefabToSpawn; } }

        /// <summary>
        /// The number of game objects that will be instantiated by this specific spawner.
        /// </summary>

        public int Quantity { get { return _quantity; } }

        /// <summary>
        /// The amount of time that should pass between new game objects being spawned. Should never be 0 as that would decrease performance.
        /// </summary>

        public float DelayBetweenSpawns { get { return _delayBetweenSpawns; } }

        /// <summary>
        /// The radius of the sphere in which game objects will randomly spawn.
        /// </summary>

        public float SpawnRadius { get { return _spawnRadius; } }

        /// <summary>
        /// The cached collider (used as a trigger) attached to this game object.
        /// </summary>

        public Collider SpawnTrigger
        {
            get { return _spawnTrigger; }
            private set { _spawnTrigger = value; }
        }

        /// <summary>
        /// The game object under which all spawned game objects will be grouped together. Set to null to disable parenting.
        /// </summary>

        public GameObject ParentUnder { get { return _parentUnder; } }

        #endregion

        #region METHODS

        /// <summary>
        /// A coroutine that spawns X number of game objects with a given delay between each spawn.
        /// </summary>
        /// <param name="amount">The amount of game objects to spawn.</param>
        /// <param name="delay">The delay between each spawn.</param>
        /// <returns></returns>

        private IEnumerator SpawnEnemies(int amount, float delay)
        {
            if (PrefabToSpawn == null)
                yield break;

            Debug.Log($"{gameObject.name}: starting spawn sequence with amount: {amount} objects, delay between instantiations: {delay} seconds.");

            for (int i = 0; i < amount; i++)
            {
                // Generate a random point inside a sphere and then sample a position on the nav mesh.

                Vector3 randomPoint = CachedTransform.position + Random.insideUnitSphere * SpawnRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPoint, out hit, Mathf.Infinity, NavMesh.AllAreas))
                {
                    var objectToInstantiate = Instantiate(PrefabToSpawn, hit.position, Quaternion.identity);

                    // Parent the instantiated objects under another game object if that parent exists.

                    if (ParentUnder != null)
                    {
                        objectToInstantiate.transform.parent = ParentUnder.transform;
                    }
                }
                else
                {
                    // Should sampling a position on the nav mesh fail, reiterate.

                    i--;
                }

                // We don't want all enemies to be spawned at once so we delay their instantiation.

                yield return new WaitForSeconds(delay);
            }

            Debug.Log($"{gameObject.name}: spawning sequence has finished.");
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedTransform = GetComponent<Transform>();
            CachedObjectPooler = ObjectPooler.Instance;
        }

        private void Start()
        {
            if (SpawnTrigger == null)
            {
                StartCoroutine(SpawnEnemies(Quantity, DelayBetweenSpawns));
                return;
            }

            SpawnTrigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (SpawnTrigger != null && other.gameObject.tag == "Player")
            {
                StartCoroutine(SpawnEnemies(Quantity, DelayBetweenSpawns));
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, SpawnRadius);
        }

        #endregion
    }
}