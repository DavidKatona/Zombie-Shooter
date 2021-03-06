﻿using Assets.MainAssets.Scripts.Damageables.Common;
using Assets.Scripts.Damageables.Common;
using Assets.Scripts.ScriptableObjects;
using Assets.Zombies.Scripts.ObjectPooling;
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

        [Header("Effects")]
        [Tooltip("The particle system that plays when the weapon is fired and is considered to be a muzzle flash.")]
        [SerializeField] private ParticleSystem _muzzleFlash = null;

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
        /// The cached object pooler instance.
        /// </summary>

        public ObjectPooler CachedObjectPooler { get; private set; }

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
        /// The particle system that plays when the weapon is fired and is considered to be a muzzle flash.
        /// </summary>

        public ParticleSystem MuzzleFlash { get { return _muzzleFlash; } }

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

            // Disable shooting and reloading logic for the length of the reload animation.

            DisableShootingAndReloading();

            // Notify the animator to start the "Firing" animation.

            CachedAnimator.SetTrigger("Fire");
        }

        /// <summary>
        /// Generates shooting effects. Declared for use in animation events only.
        /// </summary>

        private void PlayShootingEffects()
        {
            // Sound effects.

            var clipToPlay = AmmoClipObject.RuntimeValue > 0 ? WeaponShotClip : WeaponTriggerClip;
            CachedAudioSource.PlayOneShot(clipToPlay);

            // Particle effects.

            if (AmmoClipObject.RuntimeValue > 0)
            {
                MuzzleFlash.Play();
            }

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

                IRaycastHittable raycastHittable;
                if (objectToHit.TryGetComponent<IRaycastHittable>(out raycastHittable))
                {
                    raycastHittable.RegisterHit(hitInfo, FirepointTransform.position);
                }
                else
                {
                    CachedObjectPooler.SpawnFromPool("BulletImpact", hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                IDamageable damageable;
                if (objectToHit.TryGetComponent<IDamageable>(out damageable))
                {
                    damageable.TakeDamage(1);
                }
            }
        }

        /// <summary>
        /// Decreases the amount of ammunition supplied for the weapon. Called from animation events.
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

                // Disable shooting and reloading logic for the length of the reload animation.

                DisableShootingAndReloading();

                // Play reloading sound effect.

                CachedAudioSource.PlayOneShot(WeaponReloadClip);

                // Notify animator to play animation.

                CachedAnimator.SetTrigger("Reload");
            }
        }

        /// <summary>
        /// Reloads ammunition from the ammo reserves to the ammo clip. Called from animation events. 
        /// </summary>

        private void ReloadAmmoToClip()
        {
            // Reloading logic.

            var amountOfAmmoToReload = AmmoClipObject.MaximumValue - AmmoClipObject.RuntimeValue;
            var amountOfAmmoAvailable = amountOfAmmoToReload < AmmoObject.RuntimeValue ? amountOfAmmoToReload : AmmoObject.RuntimeValue;

            AmmoObject.RuntimeValue -= amountOfAmmoAvailable;
            AmmoClipObject.RuntimeValue += amountOfAmmoAvailable;
        }

        /// <summary>
        /// Enables shooting and reloading. Called from animation events.
        /// </summary>

        public void EnableShootingAndReloading()
        {
            _canShoot = true;
            _canReload = true;
        }

        /// <summary>
        /// Disable shooting and reloading.
        /// </summary>

        private void DisableShootingAndReloading()
        {
            _canShoot = false;
            _canReload = false;
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
            CachedObjectPooler = ObjectPooler.Instance;
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