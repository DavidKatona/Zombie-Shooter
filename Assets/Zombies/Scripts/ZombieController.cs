using UnityEngine;
using UnityEngine.AI;
using Assets.Zombies.Scripts;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieController : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [Header("Agent Settings")]
    [Tooltip("The target game object that the attached Nav Mesh Agent component will follow.")]
    [SerializeField] private GameObject _agentTarget;

    [Tooltip("How close the player has to be (in units) to this game object, in order to get its attention.")]
    [Range(0, 100)]
    [SerializeField] private float _aggroDistance = 10f;

    [Tooltip("How close to the player this unit has to be in order to attack.")]
    [Range(0, 100)]
    [SerializeField] private float _attackRange = 2.5f;

    [Tooltip("The out of combat, wandering speed this unit moves with.")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 1f;

    [Tooltip("The speed at which this unit moves when its chasing its target.")]
    [Range(0, 10)]
    [SerializeField] private float _chaseSpeed = 2f;

    [Tooltip("How far the agent is allowed to wander away from its previous destination.")]
    [SerializeField] private float _minimumWanderDistance = -5f;

    [Tooltip("How far the agent is allowed to wander away from its previous destination.")]
    [SerializeField] private float _maximumWanderDistance = 5f;

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

    /// <summary>
    /// How far the agent is allowed to wander away from its previous destination when setting a new one.
    /// </summary>

    public float MinimumWanderDistance { get { return _minimumWanderDistance; } }


    /// <summary>
    /// How far the agent is allowed to wander away from its previous destination when setting a new one.
    /// </summary>

    public float MaximumWanderDistance { get { return _maximumWanderDistance; } }

    #endregion

    #region METHODS

    private void HandleStates()
    {
        switch (State)
        {
            case ZombieState.Idle:
                SwitchState(ZombieState.Wander);
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

    private void Wander()
    {
        // Switch to chase if the player is within aggro distance.

        if (CanSeePlayer())
            SwitchState(ZombieState.Chase);

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

        Debug.Log($"Distance to target: {CachedNavMeshAgent.remainingDistance} | Stopping distance: {CachedNavMeshAgent.stoppingDistance}");
        Debug.DrawLine(CachedTransform.position, AgentTarget.transform.position);

        if (CachedNavMeshAgent.remainingDistance <= CachedNavMeshAgent.stoppingDistance && !CachedNavMeshAgent.pathPending)
        {
            SwitchState(ZombieState.Attack);
        }
    }

    private void Attack()
    {

    }

    private void Die()
    {

    }

    /// <summary>
    /// Determine the distance between this game object and a target.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>

    private float DistanceToPlayer()
    {
        // Compute a vector from this game object to the target and determine its magnitude.

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
        return DistanceToPlayer() < AggroDistance ? true : false;
    }

    /// <summary>
    /// Switch the current state of this game object to another.
    /// </summary>
    /// <param name="nextState">The state to switch to.</param>

    private void SwitchState(ZombieState nextState)
    {
        Debug.Log($"{gameObject.name} has switched to {nextState} (was {State}).");

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
    }

    private void Update()
    {
        // Handle state machine behaviour.

        HandleStates();
    }

    #endregion
}