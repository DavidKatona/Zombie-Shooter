using Assets.Scripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] private GameEvent _event = null;
    [SerializeField] private UnityEvent _response = null;

    #endregion

    #region PROPERTIES

    public GameEvent Event { get { return _event; } }

    public UnityEvent Response { get { return _response; } }

    #endregion

    #region METHODS

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