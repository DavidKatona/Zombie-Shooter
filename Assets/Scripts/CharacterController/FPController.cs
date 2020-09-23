using UnityEngine;

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

        // Initialize fields/properties.

        CameraRotation = cachedCamera.transform.localRotation;
        CharacterRotation = cachedTransform.localRotation;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            cachedRigidbody.AddForce(0, 300, 0);
        }
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal") * Speed;
        float z = Input.GetAxis("Vertical") * Speed;

        cachedTransform.position += (cachedTransform.forward * z + cachedTransform.right * x);
    }

    private void LateUpdate()
    {
        float yRotation = Input.GetAxis("Mouse X") * MouseSensitivity;
        float xRotation = Input.GetAxis("Mouse Y") * MouseSensitivity;

        CameraRotation *= Quaternion.Euler(-xRotation, 0, 0);
        CharacterRotation *= Quaternion.Euler(0, yRotation, 0);

        CameraRotation = ClampCameraPitch(CameraRotation);

        cachedCamera.transform.localRotation = CameraRotation;
        cachedTransform.localRotation = CharacterRotation;
    }

    #endregion
}