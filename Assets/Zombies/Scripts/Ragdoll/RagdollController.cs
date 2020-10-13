using UnityEngine;
using UnityEngine.AI;

namespace Assets.Zombies.Scripts.Ragdoll
{
    public class RagdollController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("The game object that holds the animated model and is not a ragdoll.")]
        [SerializeField] private GameObject _animatedModel = null;

        [Tooltip("The game object that is affected by physics and is the ragdoll converted prefab of the animated model.")]
        [SerializeField] private GameObject _ragdoll = null;

        [Tooltip("The rigidbody component attached to the hips of the ragdoll.")]
        [SerializeField] private Rigidbody _ragdollHipsRigidbody = null;

        [Tooltip("The nav mesh agent component attached to the animated model.")]
        [SerializeField] private NavMeshAgent _navMeshAgent = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The game object that holds the animated model and is not a ragdoll.
        /// </summary>

        public GameObject AnimatedModel { get { return _animatedModel; } }

        /// <summary>
        /// The cached transform component attached to the animated model.
        /// </summary>

        public Transform CachedModelTransform { get; private set; }

        /// <summary>
        /// The game object that is affected by physics and is the ragdoll converted prefab of the animated model.
        /// </summary>

        public GameObject Ragdoll { get { return _ragdoll; } }

        /// <summary>
        /// The rigidbody component attached to the hips of the ragdoll and the one on which forces (such as from shooting) apply.
        /// </summary>

        public Rigidbody RagdollHipsRigidbody { get { return _ragdollHipsRigidbody; } }

        /// <summary>
        /// The cached transform component attached to the ragdoll.
        /// </summary>

        public Transform CachedRagdollTransform { get; private set; }

        /// <summary>
        /// The nav mesh agent component attached to the animated model.
        /// </summary>

        public NavMeshAgent NavMeshAgent { get { return _navMeshAgent; } }

        #endregion

        #region METHODS

        /// <summary>
        /// Activates the ragdoll object defined by this script.
        /// </summary>

        public void ActivateRagdoll()
        {
            CopyTransformData(AnimatedModel.transform, Ragdoll.transform, NavMeshAgent.velocity);
            Ragdoll.SetActive(true);
            AnimatedModel.SetActive(false);
        }

        /// <summary>
        /// Copies transform specific information and rigidbody velocities from transform A to transform B.
        /// </summary>
        /// <param name="sourceTransform"></param>
        /// <param name="destinationTransform"></param>
        /// <param name="velocity"></param>

        private void CopyTransformData(Transform sourceTransform, Transform destinationTransform, Vector3 velocity)
        {
            if (sourceTransform.childCount != destinationTransform.childCount)
            {
                Debug.LogWarning("Invalid transform copy. They need to match transform hierarchies.");
                return;
            }

            for (int i = 0; i < sourceTransform.childCount; i++)
            {
                var source = sourceTransform.GetChild(i);
                var destination = destinationTransform.GetChild(i);
                destination.position = source.position;
                destination.rotation = source.rotation;

                var rigidbody = destination.GetComponent<Rigidbody>();

                if (rigidbody != null)
                {
                    rigidbody.velocity = velocity;
                }

                CopyTransformData(source, destination, velocity);
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedModelTransform = AnimatedModel.GetComponent<Transform>();
            CachedRagdollTransform = Ragdoll.GetComponent<Transform>();
            Ragdoll.SetActive(false);
        }

        #endregion
    }
}