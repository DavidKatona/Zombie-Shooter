using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [Header("Configuration")]
    [Tooltip("The game event this listener will subscribe to OnEnable.")]
    [SerializeField] private GameEvent _event = null;

    [Tooltip("The response this listener should perform when the game event is invoked.")]
    [SerializeField] private UnityEvent _response = null;

    #endregion

    #region PROPERTIES

    /// <summary>
    /// The game event this listener will subscribe to once OnEnable is called.
    /// </summary>

    public GameEvent Event { get { return _event; } }

    /// <summary>
    /// The response this listener will perform once the game event is invoked.
    /// </summary>

    public UnityEvent Response { get { return _response; } }

    #endregion

    #region METHODS

    /// <summary>
    /// This function gets called when the game event's Raise method is called.
    /// </summary>

    public void OnEventRaised()
    {
        Response.Invoke();
    }

    #endregion

    #region MONOBEHAVIOUR

    private void OnEnable()
    {
        Event.SubscribeListener(this);
    }

    private void OnDisable()
    {
        Event.UnsubscribeListener(this);
    }

    #endregion
}