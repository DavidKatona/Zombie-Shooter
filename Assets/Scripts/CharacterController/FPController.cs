using UnityEngine;

public class FPController : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [Header("Movement")]
    [SerializeField] private float _speed = 0.1f;

    #endregion

    #region FIELDS



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
    /// Cached transform component attached to this GameObject.
    /// </summary>

    public Transform cachedTransform { get; private set; }

    #endregion

    #region MONOBEHAVIOUR

    private void Awake()
    {
        // Cache components.

        cachedTransform = GetComponent<Transform>();
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        cachedTransform.position += new Vector3(x * Speed, 0, z * Speed);
    }

    #endregion
}
