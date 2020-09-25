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
        private float _xAxisRotation;
        private float _yAxisRotation;
        private bool _isJumpPressed;
        private bool _isWalkingInvoked;
        private bool _wasGrounded;
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

        /// <summary>
        /// The maximum pitch the camera can achieve in degrees.
        /// </summary>

        public float MaximumCameraPitch
        {
            get { return _maximumCameraPitch; }
            set { _maximumCameraPitch = value; }
        }

        /// <summary>
        /// The minimum pitch the camera can achieve in degrees.
        /// </summary>

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

        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// Cached capsule collider component attached to the GameObject.
        /// </summary>
        /// 
        public CapsuleCollider CachedCapsuleCollider { get; private set; }

        /// <summary>
        /// Cached rigidbody compoonent attached to the GameObject.
        /// </summary>

        public Rigidbody CachedRigidbody { get; private set; }

        /// <summary>
        /// Cached camera component attached to the GameObject or one of its children.
        /// </summary>

        public Camera CachedCamera { get; private set; }

        /// <summary>
        /// Cached animator component attached to the GameObject or one of its children.
        /// </summary>

        public Animator CachedAnimator { get; private set; }

        /// <summary>
        /// Cached sound controller component attached to the GameObject or one of its children.
        /// </summary>

        public SoundController CachedSoundController { get; private set; }

        #endregion

        #region METHODS

        private void HandleAnimations()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CachedAnimator.SetTrigger("Fire");
            }

            if (Input.GetButtonDown("Reload"))
            {
                CachedAnimator.SetTrigger("Reload");
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CachedAnimator.SetBool("IsGunHolstered", !CachedAnimator.GetBool("IsGunHolstered"));
            }

            if (_xAxisInputModifier != 0 || _zAxisInputModifier != 0)
            {
                if (!CachedAnimator.GetBool("IsWalking") && IsGrounded())
                {
                    CachedAnimator.SetBool("IsWalking", true);
                    InvokeRepeating("InvokeFootstepSounds", 0, 0.4f);
                }
            }
            else if (CachedAnimator.GetBool("IsWalking"))
            {
                CachedAnimator.SetBool("IsWalking", false);
                CancelInvoke("InvokeFootstepSounds");
                _isWalkingInvoked = false;
            }
        }

        /// <summary>
        /// Invokes the PlayFootstepAudio() method from the cached sound controller attached to this game object.
        /// (Note: This function was wrapped inside this method so the string we pass to Invoke() won't be affected by refactoring).
        /// </summary>

        private void InvokeFootstepSounds()
        {
            _isWalkingInvoked = true;
            CachedSoundController.PlayFootstepAudio();
        }

        private bool IsGrounded()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(CachedTransform.position, CachedCapsuleCollider.radius, Vector3.down, out hitInfo,
                (CachedCapsuleCollider.height / 2f) - CachedCapsuleCollider.radius + 0.1f))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clamps the pitch (x axis rotation) of the camera component so its constrained between two values.
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>

        private Quaternion ClampCameraPitch(Quaternion quaternion)
        {
            quaternion.x /= quaternion.w;
            quaternion.y /= quaternion.w;
            quaternion.z /= quaternion.w;
            quaternion.w = 1.0f;

            float cameraPitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x);
            cameraPitch = Mathf.Clamp(cameraPitch, MinimumCameraPitch, MaximumCameraPitch);
            quaternion.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * cameraPitch);

            return quaternion;
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedTransform = GetComponent<Transform>();
            CachedCapsuleCollider = GetComponent<CapsuleCollider>();
            CachedRigidbody = GetComponent<Rigidbody>();
            CachedCamera = GetComponentInChildren<Camera>();
            CachedAnimator = GetComponentInChildren<Animator>();
            CachedSoundController = GetComponentInChildren<SoundController>();

            // Initialize fields/properties.

            CameraRotation = CachedCamera.transform.localRotation;
            CharacterRotation = CachedTransform.localRotation;
        }

        private void Update()
        {
            // Handle input. Should go to its own class.

            bool isGrounded = IsGrounded();
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                _isJumpPressed = true;
            }
            else if (!_wasGrounded && isGrounded)
            {
                CachedSoundController.PlayLandingSound();
            }

            _wasGrounded = isGrounded;

            _xAxisInputModifier = Input.GetAxis("Horizontal");
            _zAxisInputModifier = Input.GetAxis("Vertical");

            _yAxisRotation = Input.GetAxis("Mouse X") * MouseSensitivity;
            _xAxisRotation = Input.GetAxis("Mouse Y") * MouseSensitivity;

            HandleAnimations();
        }

        private void FixedUpdate()
        {
            // Handle movement.

            if (_isJumpPressed)
            {
                CachedRigidbody.AddForce(0, 300, 0);
                _isJumpPressed = false;

                CachedSoundController.PlayJumpingSound();

                if (CachedAnimator.GetBool("IsWalking"))
                {
                    CancelInvoke("InvokeFootstepSounds");
                    _isWalkingInvoked = false;
                }
            }

            CachedTransform.position += (CachedTransform.forward * _zAxisInputModifier * Speed + CachedTransform.right * _xAxisInputModifier * Speed);

            CharacterRotation *= Quaternion.Euler(0, _yAxisRotation, 0);
            CachedTransform.localRotation = CharacterRotation;

            CameraRotation *= Quaternion.Euler(-_xAxisRotation, 0, 0);
            CameraRotation = ClampCameraPitch(CameraRotation);
            CachedCamera.transform.localRotation = CameraRotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CachedAnimator.GetBool("IsWalking") && !_isWalkingInvoked)
            {
                InvokeRepeating("InvokeFootstepSounds", 0, 0.4f);
            }
        }

        #endregion
    }
}