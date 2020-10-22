using Assets.Scripts.Damageables.Common;
using Assets.Scripts.Extensions;
using Assets.Scripts.ScriptableObjects;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Scripts.CharacterController
{
    public class WeaponController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Components")]
        [Tooltip("The scriptable object that holds information about the character's ammunition.")]
        [SerializeField] private IntVariable _ammoObject = null;

        [Tooltip("The scriptable object that holds information about the ammunition loaded into the weapon.")]
        [SerializeField] private IntVariable _ammoClipObject = null;

        [Tooltip("The firepoint of the weapon from which rays are cast and effects are instantiated at.")]
        [SerializeField] private Transform _firepointTransform = null;

        [Header("Options")]
        [Tooltip("The amount of ammunition the weapon uses per shot.")]
        [SerializeField] private int _ammoDepletedPerShot = 1;

        [Tooltip("The audio clip that should play when the weapon is fired.")]
        [SerializeField] private AudioClip _weaponShotClip = null;

        [Tooltip("The audio clip that should play when the weapon is fired without ammunition.")]
        [SerializeField] private AudioClip _weaponTriggerClip = null;

        [Tooltip("The audio clip that should play when the weapon is reloaded.")]
        [SerializeField] private AudioClip _weaponReloadClip = null;

        #endregion

        #region FIELDS

        // Input specific fields.

        private bool _isFirePressed;
        private bool _isReloadPressed;
        private bool _isHolsterPressed;
        private bool _canShoot = true;
        private bool _canReload = true;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached animator component attached to the GameObject or one of its children.
        /// </summary>

        public Animator CachedAnimator { get; private set; }

        /// <summary>
        /// Cached audio source component attached to this GameObject.
        /// </summary>

        public AudioSource CachedAudioSource { get; private set; }

        /// <summary>
        /// The scriptable object that holds information about the character's ammunition and this game object interacts with.
        /// </summary>

        public IntVariable AmmoObject { get { return _ammoObject; } }

        /// <summary>
        /// The scriptable object that holds information about the ammunition loaded into the weapon and its value is decremented each time the weapon is fired.
        /// </summary>

        public IntVariable AmmoClipObject { get { return _ammoClipObject; } }

        /// <summary>
        /// The firepoint of the weapon from which rays are cast and effects are instantiated at.
        /// </summary>

        public Transform FirepointTransform { get { return _firepointTransform; } }

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
        /// The audio clip that's played when the weapon is reloaded.
        /// </summary>

        public AudioClip WeaponReloadClip { get { return _weaponReloadClip; } }

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
            if (!_isFirePressed || CachedAnimator.GetBool("IsGunHolstered") || !_canShoot)
                return;

            StartCoroutine(DisableShooting(1f));
            StartCoroutine(DisableReloading(1.1f));

            // Notify the animator to start the "Firing" animation.

            CachedAnimator.SetTrigger("Fire");
        }

        private IEnumerator DisableShooting(float duration)
        {
            _canShoot = false;
            yield return new WaitForSeconds(duration);
            _canShoot = true;
        }

        private IEnumerator DisableReloading(float duration)
        {
            _canReload = false;
            yield return new WaitForSeconds(duration);
            _canReload = true;
        }

        /// <summary>
        /// Generates shooting effects. Declared for use in animation events only.
        /// </summary>

        private void PlayShootingEffects()
        {
            // Sound effects.

            var clipToPlay = AmmoClipObject.RuntimeValue > 0 ? WeaponShotClip : WeaponTriggerClip;
            CachedAudioSource.PlayOneShot(clipToPlay);

            HandleShootingLogic();
        }

        private void HandleShootingLogic()
        {
            if (AmmoClipObject.RuntimeValue <= 0)
                return;


            RaycastHit hitInfo;
            if (Physics.Raycast(FirepointTransform.position, Camera.main.transform.forward, out hitInfo, 200))
            {
                GameObject objectToHit = hitInfo.collider.gameObject;
                IDamageable damageable = objectToHit.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(1);
                }
            }
        }

        /// <summary>
        /// Decreses the amount of ammunition supplied for the weapon. Called from animation events.
        /// </summary>

        private void DecrementAmmo()
        {
            if (AmmoClipObject.RuntimeValue <= 0)
                return;

            AmmoClipObject.RuntimeValue--;
        }

        private void Reload()
        {
            if (_isReloadPressed && !CachedAnimator.GetBool("IsGunHolstered") && _canReload)
            {
                // If ammo pool is empty or clip is full, cancel reloading.

                if (AmmoObject.RuntimeValue <= 0 || AmmoClipObject.RuntimeValue == AmmoClipObject.MaximumValue)
                    return;

                // Disable shooting logic for the length of the reload animation.

                StartCoroutine(DisableShooting(4f));
                StartCoroutine(DisableReloading(4.1f));

                // Play reloading sound effect.

                CachedAudioSource.PlayOneShot(WeaponReloadClip);

                // Notify animator to play animation.

                CachedAnimator.SetTrigger("Reload");

                // Reloading logic.

                var amountOfAmmoToReload = AmmoClipObject.MaximumValue - AmmoClipObject.RuntimeValue;
                var amountOfAmmoAvailable = amountOfAmmoToReload < AmmoObject.RuntimeValue ? amountOfAmmoToReload : AmmoObject.RuntimeValue;

                AmmoObject.RuntimeValue -= amountOfAmmoAvailable;
                AmmoClipObject.RuntimeValue += amountOfAmmoAvailable;
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
            CachedAudioSource = GetComponent<AudioSource>();
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