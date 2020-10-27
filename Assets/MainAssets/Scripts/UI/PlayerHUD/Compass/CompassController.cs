using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD.Compass
{
    public class CompassController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [SerializeField] private Transform _objectiveTransform = null;
        [SerializeField] private GameObject _pointer = null;
        [SerializeField] private RectTransform _compassBar = null;

        #endregion

        #region FIELDS

        private Vector3[] _cornerPositions = new Vector3[4];

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The cached transform component attached to the player.
        /// </summary>

        public Transform CachedPlayerTransform { get; private set; }
        
        /// <summary>
        /// The cached transform component attached to the goal/objective game object.
        /// </summary>

        public Transform ObjectiveTransform { get { return _objectiveTransform; } }

        /// <summary>
        /// The UI element (usually an arrow) that points towards the goal's transform.
        /// </summary>

        public GameObject Pointer { get { return _pointer; } }

        /// <summary>
        /// The rect transform component of the Pointer.
        /// </summary>

        public RectTransform PointerRectangle { get; private set; }

        /// <summary>
        /// The rect transform component of the compass bar game object.
        /// </summary>

        public RectTransform CompassBar { get { return _compassBar; } }

        /// <summary>
        /// The scale of the compass bar calculated from the two bottom corners of the compass' rect transform.
        /// </summary>

        public float CompassBarScale { get; private set; }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedPlayerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            PointerRectangle = Pointer.GetComponent<RectTransform>();
            CompassBar.GetLocalCorners(_cornerPositions);
            CompassBarScale = Vector3.Distance(_cornerPositions[1], _cornerPositions[2]);
        }

        private void Update()
        {
            if (ObjectiveTransform == null || CachedPlayerTransform == null)
                return;

            // Compute the direction the pointer should aim towards.

            Vector3 direction = ObjectiveTransform.position - CachedPlayerTransform.position;

            // Determine the angle between the player's forward vector and our direction.

            float angleToDirection = Vector3.SignedAngle(CachedPlayerTransform.forward, direction, CachedPlayerTransform.up);

            // Modify the result so it fits the scale of the compass bar, then position it.

            angleToDirection = Mathf.Clamp(angleToDirection, -90, 90) / 180f * CompassBarScale;
            PointerRectangle.localPosition = new Vector3(angleToDirection, PointerRectangle.localPosition.y, PointerRectangle.localPosition.z);
        }

        #endregion

    }
}