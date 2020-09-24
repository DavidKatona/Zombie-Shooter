using UnityEngine;

namespace Assets.Scripts.CharacterController
{
    public class FPController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Movement")]
        [SerializeField] private float _speed = 0.1f;

        [Header("Camera Movement")]
        [SerializeField] private float _mouseSensitivity = 5f;
        [SerializeField] private float _maximumCameraPitch = 90f;
        [SerializeField] private float _minimumCameraPitch = -90f;

        #endregion

        #region FIELDS

        private float _xAxisInputModifier;
        private float _zAxisInputModifier;
        private bool _isJumpPressed;
        private Quaternion _cameraRotation;
        private Quaternion _characterRotation;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Maximum movement speed (in m/s).
        /// </summary>

        public float Speed
        {
            get { return _speed; }
            set { _speed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Mouse sensitivity modifier that affects both X and Y axis movement.
        /// </summary>

        public float MouseSensitivity
        {
            get { return _mouseSensitivity; }
            set { _mouseSensitivity = value; }
        }

        public float MaximumCameraPitch
        {
            get { return _maximumCameraPitch; }
            set { _maximumCameraPitch = value; }
        }

        public float MinimumCameraPitch
        {
            get { return _minimumCameraPitch; }
            set { _minimumCameraPitch = value; }
        }

        public Quaternion CameraRotation
        {
            get { return _cameraRotation; }
            set { _cameraRotation = value; }
        }

        public Quaternion CharacterRotation
        {
            get { return _characterRotation; }
            set { _characterRotation = value; }
        }

        /// <summary>
        /// Cached transform component attached to the GameObject.
        /// </summary>

        public Transform cachedTransform { get; private set; }

        /// <summary>
        /// Cached capsule collider component attached to the GameObject.
        /// </summary>
        /// 
        public CapsuleCollider cachedCapsuleCollider { get; private set; }

        /// <summary>
        /// Cached rigidbody compoonent attached to the GameObject.
        /// </summary>

        public Rigidbody cachedRigidbody { get; private set; }

        /// <summary>
        /// Cached camera component attached to the GameObject or one of its children.
        /// </summary>

        public Camera cachedCamera { get; private set; }

        /// <summary>
        /// Cached animator component attached to the GameObject or one of its children.
        /// </summary>

        public Animator cachedAnimator { get; private set; }

        #endregion

        #region METHODS

        private bool IsGrounded()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(cachedTransform.position, cachedCapsuleCollider.radius, Vector3.down, out hitInfo,
                (cachedCapsuleCollider.height / 2f) - cachedCapsuleCollider.radius + 0.1f))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clamps the pitch (x axis rotation) of the camera component so its constrained between two values.
        /// </summary>
        /// <param name="currentCameraRotation"></param>
        /// <returns></returns>

        private Quaternion ClampCameraPitch(Quaternion currentCameraRotation)
        {
            currentCameraRotation.x /= currentCameraRotation.w;
            currentCameraRotation.y /= currentCameraRotation.w;
            currentCameraRotation.z /= currentCameraRotation.w;
            currentCameraRotation.w = 1.0f;

            float cameraPitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(currentCameraRotation.x);
            cameraPitch = Mathf.Clamp(cameraPitch, MinimumCameraPitch, MaximumCameraPitch);
            currentCameraRotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * cameraPitch);

            return currentCameraRotation;
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache components.

            cachedTransform = GetComponent<Transform>();
            cachedCapsuleCollider = GetComponent<CapsuleCollider>();
            cachedRigidbody = GetComponent<Rigidbody>();
            cachedCamera = GetComponentInChildren<Camera>();
            cachedAnimator = GetComponentInChildren<Animator>();

            // Initialize fields/properties.

            CameraRotation = cachedCamera.transform.localRotation;
            CharacterRotation = cachedTransform.localRotation;
        }

        private void Update()
        {
            // Handle input. Should go to its own class.

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                _isJumpPressed = true;
            }

            _xAxisInputModifier = Input.GetAxis("Horizontal");
            _zAxisInputModifier = Input.GetAxis("Vertical");

            float yRotation = Input.GetAxis("Mouse X") * MouseSensitivity;
            float xRotation = Input.GetAxis("Mouse Y") * MouseSensitivity;

            CameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
            CharacterRotation *= Quaternion.Euler(0, yRotation, 0);

            CameraRotation = ClampCameraPitch(CameraRotation);

            if (Input.GetButtonDown("Fire1"))
            {
                cachedAnimator.SetBool("IsFiring", true);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                cachedAnimator.SetBool("IsFiring", false);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                cachedAnimator.SetBool("IsGunHolstered", !cachedAnimator.GetBool("IsGunHolstered"));
            }
        }

        private void FixedUpdate()
        {
            // Handle movement.

            if (_isJumpPressed)
            {
                cachedRigidbody.AddForce(0, 300, 0);
                _isJumpPressed = false;
            }

            cachedTransform.position += (cachedTransform.forward * _zAxisInputModifier * Speed + cachedTransform.right * _xAxisInputModifier * Speed);
        }

        private void LateUpdate()
        {
            // Handle camera movement and animations in late update.

            cachedCamera.transform.localRotation = CameraRotation;
            cachedTransform.localRotation = CharacterRotation;
        }

        #endregion
    }
}