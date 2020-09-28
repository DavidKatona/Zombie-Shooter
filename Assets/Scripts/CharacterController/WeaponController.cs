using Assets.Scripts.Extensions;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.CharacterController
{
    public class WeaponController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The scriptable object that holds information about the character's ammunition.")]
        [SerializeField] private IntVariable _ammoObject = null;

        [Header("Options")]
        [Tooltip("The amount of ammunition the weapon uses per shot.")]
        [SerializeField] private int _ammoDepletedPerShot = 1;

        [Tooltip("The audio clip that should play when the weapon is fired.")]
        [SerializeField] private AudioClip _weaponShotClip = null;

        [Tooltip("The audio clip that should play when the weapon is fired without ammunition.")]
        [SerializeField] private AudioClip _weaponTriggerClip = null;

        #endregion

        #region FIELDS

        // Input specific fields.

        private bool _isFirePressed;
        private bool _isReloadPressed;
        private bool _isHolsterPressed;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The scriptable object that holds information about the character's ammunition and this game object interacts with.
        /// </summary>

        public IntVariable AmmoObject { get { return _ammoObject; } }

        /// <summary>
        /// The amount of ammunition depleted when this weapon is fired (ammo/shot).
        /// </summary>

        public int AmmoDepletedPerShot { get { return _ammoDepletedPerShot; } }

        /// <summary>
        /// The audio clip that's played when the weapon is fired.
        /// </summary>

        public AudioClip WeaponShotClip { get { return _weaponShotClip; } }

        /// <summary>
        /// The audio clip that's played when the weapon fired with an empty magazine.
        /// </summary>
        /// 

        public AudioClip WeaponTriggerClip { get { return _weaponTriggerClip; } }

        /// <summary>
        /// Cached animator component attached to the GameObject or one of its children.
        /// </summary>

        public Animator CachedAnimator { get; private set; }

        #endregion

        #region METHODS

        private void ReadInput()
        {
            _isFirePressed = Input.GetButtonDown("Fire1");

            _isReloadPressed = Input.GetButtonDown("Reload");

            _isHolsterPressed = Input.GetButtonDown("Holster");
        }

        private void Shoot()
        {
            if (!_isFirePressed || CachedAnimator.GetBool("IsGunHolstered"))
                return;

            // Notify the animator to start the "Firing" animation.

            CachedAnimator.SetTrigger("Fire");
        }

        /// <summary>
        /// Generates shooting effects. Declared for use in animation events only.
        /// </summary>

        private void PlayShootingEffects()
        {
            // Sound effects.

            var clipToPlay = AmmoObject.RuntimeValue > 0 ? WeaponShotClip : WeaponTriggerClip;
            AudioSource audioSource = new AudioSource();
            audioSource.InstantiateAudioSource(clipToPlay, transform.position);
        }

        /// <summary>
        /// Decreses the amount of ammunition supplied for the weapon. Called from animation events.
        /// </summary>

        private void DecrementAmmo()
        {
            if (AmmoObject.RuntimeValue <= 0)
                return;

            AmmoObject.RuntimeValue--;
        }

        private void Reload()
        {
            if (_isReloadPressed && !CachedAnimator.GetBool("IsGunHolstered"))
            {
                // ToDo: Add further reloading logic.

                CachedAnimator.SetTrigger("Reload");
            }
        }

        private void Holster()
        {
            if (_isHolsterPressed)
            {
                CachedAnimator.SetBool("IsGunHolstered", !CachedAnimator.GetBool("IsGunHolstered"));
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            // Cache and initialize components.

            CachedAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Read Input.

            ReadInput();

            // Handle shooting.

            Shoot();

            // Handle reloading.

            Reload();

            // Handle holstering.

            Holster();
        }

        #endregion
    }
}