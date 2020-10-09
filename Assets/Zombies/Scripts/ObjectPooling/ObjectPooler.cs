using System.Collections.Generic;
using UnityEngine;

namespace Assets.Zombies.Scripts.ObjectPooling
{
    public class ObjectPooler : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("The list of pools this object pooler holds.")]
        [SerializeField] private List<Pool> _pools = null;

        #endregion

        #region FIELDS

        private static ObjectPooler _instance;

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
            if (Instance != null || Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
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

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

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

        private void Start()
        {
            // Iterate through all the pools and add them to the pool dictionary.

            foreach (Pool pool in Pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.Size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                PoolDictionary.Add(pool.Tag, objectPool);
            }
        }

        #endregion
    }
}
