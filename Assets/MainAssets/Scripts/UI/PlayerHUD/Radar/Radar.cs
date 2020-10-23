using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD.Radar
{
    public class Radar : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Configuration")]
        [Tooltip("How far in units should the radar's detection range extend outwards from its center?")]
        [SerializeField] private float _mapScale = 2.0f;

        [SerializeField] private List<RadarObject> _radarObjects = new List<RadarObject>();

        #endregion

        #region FIELDS

        private static Radar _instance;

        #endregion

        #region PROPERTIES

        public static Radar Instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }

        public Transform CachedTransform { get; private set; }

        public RectTransform CachedRectTransform { get; private set; }

        public Transform CachedPlayerTransform { get; private set; }

        public float MapScale { get { return _mapScale; } }

        #endregion

        #region METHODS

        /// <summary>
        /// Initiliaze the static instance of this class and ensure that only one exists simultaneously.
        /// </summary>

        private void InitializeInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
        }

        public void RegisterRadarObject(GameObject gameObject, Image icon)
        {
            Image image = Instantiate(icon);
            _radarObjects.Add(new RadarObject() { Owner = gameObject, Icon = image });
        }

        public void RemoveRadarObject(GameObject gameObject)
        {
            List<RadarObject> newRadarObjects = new List<RadarObject>();

            for (int i = 0; i < _radarObjects.Count; i++)
            {
                if (_radarObjects[i].Owner == gameObject)
                {
                    Destroy(_radarObjects[i].Icon);
                    continue;
                }
                else
                {
                    newRadarObjects.Add(_radarObjects[i]);
                }
            }

            _radarObjects.RemoveRange(0, _radarObjects.Count);
            _radarObjects.AddRange(newRadarObjects);
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            InitializeInstance();
            CachedTransform = GetComponent<Transform>();
            CachedRectTransform = GetComponent<RectTransform>();
            CachedPlayerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        private void Update()
        {
            if (CachedPlayerTransform == null || CachedPlayerTransform.gameObject.activeInHierarchy)
            {
                foreach (var radarObject in _radarObjects)
                {
                    Vector3 objectPosition = radarObject.Owner.transform.position - CachedPlayerTransform.position;
                    float distanceToObject = Vector3.Distance(CachedPlayerTransform.position, radarObject.Owner.transform.position) * MapScale;

                    float deltaY = Mathf.Atan2(objectPosition.x, objectPosition.z) * Mathf.Rad2Deg - 270 - CachedPlayerTransform.eulerAngles.y;
                    objectPosition.x = distanceToObject * Mathf.Cos(deltaY * Mathf.Deg2Rad) * -1;
                    objectPosition.z = distanceToObject * Mathf.Sin(deltaY * Mathf.Deg2Rad);

                    radarObject.Icon.transform.SetParent(CachedTransform);
                    radarObject.Icon.transform.position = new Vector3(objectPosition.x + CachedRectTransform.pivot.x, objectPosition.z + CachedRectTransform.pivot.y, 0) + CachedTransform.position;
                }
            }
        }

        #endregion
    }
}