using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = "Variable Types/Int Variable", order = 0)]
    public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Values")]
        [Tooltip("The initial value this object holds before its deserialized by Unity.")]
        [SerializeField] private int _initialValue = 0;

        [Tooltip("The maximum value this scriptable object can store.")]
        [SerializeField] private int _maximumValue = 1;

        [Tooltip("The minimum value this scriptable object can store.")]
        [SerializeField] private int _minimumValue = 0;

        #endregion

        #region FIELDS

        private int _runtimeValue;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The initial value that's stored on this scriptable object before its deserialized by Unity.
        /// </summary>

        public int InitialValue
        { 
            get { return _initialValue; }
            private set{ _initialValue = value; }
        }

        /// <summary>
        /// The runtime value that will be used after this object is deserialized by Unity.
        /// </summary>

        public int RuntimeValue
        {
            get { return _runtimeValue; }
            set
            {
                _runtimeValue = value;
                Mathf.Clamp(_runtimeValue, MinimumValue, MaximumValue);
            }
        }

        /// <summary>
        /// The maximum value this scriptable object can store.
        /// </summary>

        public int MaximumValue { get { return _maximumValue; } }

        /// <summary>
        /// The minimum value this scriptable object can store.
        /// </summary>

        public int MinimumValue { get { return _minimumValue; } }

        #endregion

        #region SCRIPTABLE OBJECT

        public void OnValidate()
        {
            InitialValue = Mathf.Clamp(InitialValue, MinimumValue, MaximumValue);
            RuntimeValue = InitialValue;
        }

        public void OnAfterDeserialize()
        {
            RuntimeValue = InitialValue;
        }

        public void OnBeforeSerialize() { }

        #endregion
    }
}