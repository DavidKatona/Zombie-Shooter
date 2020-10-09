using System.Collections.Generic;
using UnityEngine;

namespace Assets.Zombies.Scripts.ObjectPooling
{
    public class ObjectPooler : MonoBehaviour
    {
        #region FIELDS

        private static ObjectPooler _instance;
        private List<Pool> _pools = new List<Pool>();

        #endregion

        #region PROPERTIES

        public static ObjectPooler Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        public Dictionary<string, Queue<GameObject>> PoolDictionary { get; set; }

        public List<Pool> Pools { get { return _pools; } }

        #endregion

        #region METHODS

        /// <summary>
        /// Initiliaze the static instance of this class and ensure that only one exists simultaneously.
        /// </summary>

        private void InitializeInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Adds a new pool to the object pooler defined by a tag, size, and a prefab.
        /// </summary>
        /// <param name="tag">The tag to assign to the pool.</param>
        /// <param name="size">The size of the pool.</param>
        /// <param name="prefab">The prefab used by the pool.</param>
        /// <param name="position">The position at which the pooled object will be instantiated.</param>

        public void AddPool(string tag, int size, GameObject prefab, Vector3 position)
        {
            Pool poolToAdd = new Pool()
            {
                Tag = tag,
                Size = size,
                Prefab = prefab
            };

            Pools.Add(poolToAdd);

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolToAdd.Size; i++)
            {
                GameObject obj = Instantiate(poolToAdd.Prefab, position, Quaternion.identity);
                obj.transform.parent = gameObject.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            PoolDictionary.Add(poolToAdd.Tag, objectPool);
        }

        /// <summary>
        /// Spawns (activates) an object from a pool with a given tag.
        /// </summary>
        /// <param name="tag">The tag of the pool to access.</param>
        /// <param name="position">The position where the spawned object should be moved to after activation.</param>
        /// <param name="rotation">The rotation that the object should be in after being activated.</param>
        /// <returns></returns>

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!PoolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag: {tag} does not exist.");
                return null;
            }

            GameObject objectToSpawn = PoolDictionary[tag].Dequeue();

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            PoolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            InitializeInstance();
            PoolDictionary = new Dictionary<string, Queue<GameObject>>();
        }

        #endregion
    }
}
