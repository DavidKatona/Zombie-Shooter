using UnityEngine;
using UnityEngine.AI;
using Assets.Zombies.Scripts;
using Assets.Zombies.Scripts.Ragdoll;
using Assets.Scripts.Damageables.Common;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieController : MonoBehaviour, IDamageable
{
    #region EDITOR EXPOSED FIELDS

    [Header("Agent Settings")]
    [Tooltip("The target game object that the attached Nav Mesh Agent component will follow.")]
    [SerializeField] private GameObject _agentTarget;

    [Tooltip("How far the agent is allowed to wander away from its previous destination.")]
    [SerializeField] private float _minimumWanderDistance = -5f;

    [Tooltip("How far the agent is allowed to wander away from its previous destination.")]
    [SerializeField] private float _maximumWanderDistance = 5f;

    [Header("Zombie Stats")]
    [Space(10)]
    [Tooltip("The amount of damage this particular zombie type deals each hit.")]
    [SerializeField] private int _damage = 10;

    [Header("Zombie Behaviour")]
    [Space(10)]
    [Tooltip("How close the player has to be (in units) to this game object, in order to get its attention.")]
    [Range(0, 100)]
    [SerializeField] private float _aggroDistance = 10f;

    [Tooltip("How close the target has to be (in units) to this game object, so it will keep chasing it around the map.")]
    [Range(0, 1000)]
    [SerializeField] private float _chaseDistance = 30f;

    [Tooltip("How close to the player this unit has to be in order to attack.")]
    [Range(0, 100)]
    [SerializeField] private float _attackRange = 2f;

    [Tooltip("The out of combat, wandering speed this unit moves with.")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 1f;

    [Tooltip("The speed at which this unit moves when its chasing its target.")]
    [Range(0, 10)]
    [SerializeField] private float _chaseSpeed = 2f;

    #endregion

    #region FIELDS

    private ZombieState _state = ZombieState.Idle;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The cached transform component attached to this game object.
    /// </summary>

    public Transform CachedTransform { get; set; }

    /// <summary>
    /// The cached animator component attached to this game object.
    /// </summary>

    public Animator CachedAnimator { get; private set; }

    /// <summary>
    /// The cached animator controller parameters fetched from the attached animator component attached to this game object.
    /// </summary>

    public AnimatorControllerParameter[] CachedAnimatorControllerParameters { get; private set; }

    /// <summary>
    /// The cached nav mesh agent component attached to this game object.
    /// </summary>

    public NavMeshAgent CachedNavMeshAgent { get; private set; }

    /// <summary>
    /// The cached ragdoll controller component attached to this game object or its parents.
    /// </summary>

    public RagdollController CachedRagdollController { get; private set; }

    /// <summary>
    /// The state this zombie is currently in. Represents the current behaviour its doing.
    /// </summary>

    public ZombieState State
    {
        get { return _state; }
        private set { _state = value; }
    }

    public GameObject AgentTarget
    {
        get { return _agentTarget; }
        private set { _agentTarget = value; }
    }

    /// <summary>
    /// How close the player has to be to this game object in order to get its attention (aggro it).
    /// </summary>

    public float AggroDistance { get { return _aggroDistance; } }

    /// <summary>
    /// How far the agent is allowed to wander away from its previous destination when setting a new one.
    /// </summary>

    public float MinimumWanderDistance { get { return _minimumWanderDistance; } }


    /// <summary>
    /// How far the agent is allowed to wander away from its previous destination when setting a new one.
    /// </summary>

    public float MaximumWanderDistance { get { return _maximumWanderDistance; } }

    /// <summary>
    /// How much damage this zombie deals with each strike.
    /// </summary>

    public int Damage { get { return _damage; } }

    /// <summary>
    /// How close the target has to be (in units) to this game object, so it will keep chasing it around the map.
    /// Note: The chase will be abandoned if the target gets outside of this range.
    /// </summary>

    public float ChaseDistance { get { return _chaseDistance; } }

    /// <summary>
    /// How close to the player this unit has to be in order to attack.
    /// </summary>

    public float AttackRange { get { return _attackRange; } }

    /// <summary>
    /// The out of combat movement speed this unit moves with (wandering speed).
    /// </summary>

    public float WalkSpeed { get { return _walkSpeed; } }

    /// <summary>
    /// The movement speed this unit moves with when its chasing its target (chase speed).
    /// </summary>

    public float ChaseSpeed { get { return _chaseSpeed; } }

    #endregion

    #region METHODS

    private void HandleStates()
    {
        switch (State)
        {
            case ZombieState.Idle:
                Idle();
                break;
            case ZombieState.Wander:
                Wander();
                break;
            case ZombieState.Chase:
                Chase();
                break;
            case ZombieState.Attack:
                Attack();
                break;
            case ZombieState.Dead:
                Die();
                break;
        }
    }

    private void Idle()
    {
        // Switch to chase if the player is within aggro distance.

        if (CanSeePlayer())
        {
            SwitchState(ZombieState.Chase);
        }
        else if (Random.Range(0, 5000) < 5)
        {
            SwitchState(ZombieState.Wander);
        }
    }

    private void Wander()
    {
        // Switch to chase if the player is within aggro distance.

        if (CanSeePlayer())
        {
            SwitchState(ZombieState.Chase);
        }
        else if (Random.Range(0, 5000) < 5)
        {
            SwitchState(ZombieState.Idle);
            ResetAnimatorParameters(CachedAnimatorControllerParameters);
            CachedNavMeshAgent.ResetPath();

            return;
        }

        // Reset all animation parameters if the agent is close to its destination so its not going to walk or run in place.

        if (CachedNavMeshAgent.remainingDistance <= 0.25f)
        {
            ResetAnimatorParameters(CachedAnimatorControllerParameters);
        }

        // Only generate a new target destination of the nav mesh agent does not have a path already.

        if (CachedNavMeshAgent.hasPath)
            return;

        // Generate a new target destination for the nav mesh agent to use.

        float newX = CachedTransform.position.x + Random.Range(MinimumWanderDistance, MaximumWanderDistance);
        float newZ = CachedTransform.position.z + Random.Range(MinimumWanderDistance, MaximumWanderDistance);
        float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
        Vector3 targetDestination = new Vector3(newX, newY, newZ);

        CachedNavMeshAgent.SetDestination(targetDestination);
        CachedNavMeshAgent.speed = WalkSpeed;
        CachedNavMeshAgent.stoppingDistance = 0;

        // Reset all animator parameters and notify the animator to start the walking animaton.

        ResetAnimatorParameters(CachedAnimatorControllerParameters);
        CachedAnimator.SetBool("IsWalking", true);
    }

    private void Chase()
    {
        // Set new destination to the player and change agent parameters.

        CachedNavMeshAgent.SetDestination(AgentTarget.transform.position);
        CachedNavMeshAgent.speed = ChaseSpeed;
        CachedNavMeshAgent.stoppingDistance = AttackRange;

        // Reset all animator parameters and notify the animator to start the walking animaton.

        ResetAnimatorParameters(CachedAnimatorControllerParameters);
        CachedAnimator.SetBool("IsRunning", true);

        // If within attack range, transition to attack state.

        if (CachedNavMeshAgent.remainingDistance <= CachedNavMeshAgent.stoppingDistance && !CachedNavMeshAgent.pathPending)
        {
            SwitchState(ZombieState.Attack);
        }

        // If outside of chase distance, disengage, and continue to wander.

        if (!IsWithinChaseDistance())
        {
            SwitchState(ZombieState.Wander);
            CachedNavMeshAgent.ResetPath();
        }
    }

    private void Attack()
    {
        // Rotate this game object to face its target.

        Vector3 targetPosition = new Vector3(AgentTarget.transform.position.x, CachedTransform.position.y, AgentTarget.transform.position.z);
        CachedTransform.LookAt(targetPosition);

        // Reset all animator parameters and notify the animator to start the walking animaton.

        ResetAnimatorParameters(CachedAnimatorControllerParameters);
        CachedAnimator.SetBool("IsAttacking", true);

        // Let this game object continue its chase after the player is outside of its attack range (stopping distance).

        if (DistanceToPlayer() > CachedNavMeshAgent.stoppingDistance)
        {
            SwitchState(ZombieState.Chase);
        }
    }

    private void OnAttackDone()
    {
        // Verify if the target is still valid by checking if it's still active in the scene.

        if (!CanSeePlayer())
            return;

        IDamageable damageable = AgentTarget.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(Damage);
        }
    }

    public void OnTargetDisabled()
    {
        if (!AgentTarget.activeInHierarchy)
        {
            CachedNavMeshAgent.ResetPath();
            ResetAnimatorParameters(CachedAnimatorControllerParameters);
            SwitchState(ZombieState.Idle);

            return;
        }
    }

    private void Die()
    {
        // To be implemented.
    }

    public void TakeDamage(int amount)
    {
        // ToDo: Finish the implementation of this function. It's only for testing purposes.

        ResetAnimatorParameters(CachedAnimatorControllerParameters);
        SwitchState(ZombieState.Dead);

        CachedRagdollController.ActivateRagdoll();
        CachedRagdollController.RagdollHipsRigidbody.AddForce(Camera.main.transform.forward * 3000);
    }

    /// <summary>
    /// A helper function that dynamically finds an object with a specific tag, should these objects be instantiated from a prefab and their target has not been
    /// defined manually in the inspector.
    /// </summary>

    private void RegisterTargetWithTag(string tag)
    {
        if (AgentTarget == null)
        {
            AgentTarget = GameObject.FindGameObjectWithTag(tag);
        }
    }

    /// <summary>
    /// Determine the distance between this game object and a target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>

    private float DistanceToPlayer()
    {
        // Compute a vector from this game object to the target (usually the player) and determine its magnitude.

        Vector3 vectorToTarget = AgentTarget.transform.position - transform.position;
        float distanceToPlayer = vectorToTarget.magnitude;

        return distanceToPlayer;
    }

    /// <summary>
    /// Determines whether this game object can see the player.
    /// </summary>
    /// <returns></returns>

    private bool CanSeePlayer()
    {
        if (AgentTarget.activeInHierarchy)
        {
            return DistanceToPlayer() < AggroDistance ? true : false;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the distance between the target (usually the player) and this game object is smaller than the chase distance.
    /// </summary>
    /// <returns></returns>

    private bool IsWithinChaseDistance()
    {
        return DistanceToPlayer() < ChaseDistance ? true : false;
    }

    /// <summary>
    /// Switch the current state of this game object to another.
    /// </summary>
    /// <param name="nextState">The state to switch to.</param>

    private void SwitchState(ZombieState nextState)
    {
        State = nextState;
    }

    /// <summary>
    /// Resets the passed in animator parameters to their default values.
    /// </summary>
    /// <param name="parameters">The list of parameters which should be changed.</param>

    private void ResetAnimatorParameters(AnimatorControllerParameter[] parameters)
    {
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            // When setting parameters with type bool, int, and float, we follow the principle of setting them to their C# default counterparts (i.e. bools to false, ints and floats to 0, etc.)

            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                CachedAnimator.SetBool(parameter.name, false);
            }
            else if (parameter.type == AnimatorControllerParameterType.Int || parameter.type == AnimatorControllerParameterType.Float)
            {
                CachedAnimator.SetFloat(parameter.name, 0);
            }
            else if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                CachedAnimator.ResetTrigger(parameter.name);
            }
        }
    }

    #endregion

    #region MONOBEHAVIOUR

    private void Awake()
    {
        // Cache and initialize components.

        CachedTransform = GetComponent<Transform>();
        CachedNavMeshAgent = GetComponent<NavMeshAgent>();
        CachedAnimator = GetComponent<Animator>();
        CachedAnimatorControllerParameters = CachedAnimator.parameters;
        CachedRagdollController = GetComponentInParent<RagdollController>();

        RegisterTargetWithTag("Player");
    }

    private void Update()
    {
        // Handle state machine behaviour.

        HandleStates();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AggroDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ChaseDistance);

        if (CachedNavMeshAgent != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(CachedNavMeshAgent.destination, 1f);
        }
    }

    #endregion
}