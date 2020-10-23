using UnityEngine;
using UnityEngine.UI;

namespace Assets.MainAssets.Scripts.UI.PlayerHUD.Radar
{
    public class RadarObjectRegisterer : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Configuration")]
        [SerializeField] private Image _image = null;

        #endregion

        #region PROPERTIES

        public Image Image { get { return _image; } }

        #endregion

        #region MONOBEHAVIOUR

        private void Start()
        {
            Radar.Instance.RegisterRadarObject(gameObject, Image);
        }

        private void OnDisable()
        {
            Radar.Instance.RemoveRadarObject(gameObject);
        }

        #endregion
    }
}