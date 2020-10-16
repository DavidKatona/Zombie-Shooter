using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Game Event", menuName = "Event Systems/Game Event", order = 0)]
    public class GameEvent : ScriptableObject
    {
        #region FIELDS

        private List<GameEventListener> _listeners = new List<GameEventListener>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The list of listeners subscribed to this game event.
        /// </summary>

        public List<GameEventListener> Listeners { get { return _listeners; } }

        #endregion

        #region METHODS

        /// <summary>
        /// Raise the event and notify all subscribed listeners.
        /// </summary>

        public void Raise()
        {
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                Listeners[i].OnEventRaised();
            }
        }

        /// <summary>
        /// Subscribe a new listener to this game event.
        /// </summary>
        /// <param name="listener"></param>

        public void SubscribeListener(GameEventListener listener)
        {
            Listeners.Add(listener);
        }

        /// <summary>
        /// Unsubscribe a listener from this game event.
        /// </summary>
        /// <param name="listener"></param>

        public void UnsubscribeListener(GameEventListener listener)
        {
            Listeners.Remove(listener);
        }

        #endregion
    }
}