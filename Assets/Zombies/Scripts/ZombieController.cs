using System.Runtime.CompilerServices;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    #region FIELDS

    private bool _isWalkingPressed;
    private bool _isRunningPressed;
    private bool _isAttackingPressed;
    private bool _isDyingPressed;

    #endregion

    #region PROPERTIES

    public Animator CachedAnimator { get; private set; }

    public bool IsWalkingPressed
    {
        get { return _isWalkingPressed; }
        private set { _isWalkingPressed = value; }
    }

    public bool IsRunningPressed
    {
        get { return _isRunningPressed; }
        private set { _isRunningPressed = value; }
    }

    public bool IsAttackingPressed
    {
        get { return _isAttackingPressed; }
        private set { _isAttackingPressed = value; }
    }

    public bool IsDyingPressed
    {
        get { return _isDyingPressed; }
        private set { _isDyingPressed = value; }
    }

    #endregion

    #region METHODS

    private void ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IsWalkingPressed = !IsWalkingPressed;
            CachedAnimator.SetBool("IsWalking", IsWalkingPressed);

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            IsRunningPressed = !IsRunningPressed;
            CachedAnimator.SetBool("IsRunning", IsRunningPressed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            IsAttackingPressed = !IsAttackingPressed;
            CachedAnimator.SetBool("IsAttacking", IsAttackingPressed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            IsDyingPressed = !IsDyingPressed;
            CachedAnimator.SetBool("IsDying", IsDyingPressed);
        }
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Awake()
    {
        CachedAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadInput();
    }

    #endregion
}
