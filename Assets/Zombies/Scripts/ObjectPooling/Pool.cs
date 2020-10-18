using System;
using UnityEngine;

namespace Assets.Zombies.Scripts.ObjectPooling
{
    [Serializable]
    public class Pool
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("The tag that is assigned to this pool of objects.")]
        [SerializeField] private string _tag;

        [Tooltip("The prefab that defines the kind of instances this pool holds.")]
        [SerializeField] private GameObject _prefab;

        [Tooltip("The amount of objects stored in this pool.")]
        [SerializeField] private int _size;

        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// The tag that is assigned to this pool of objects.
        /// </summary>

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// The prefab that this pool stores.
        /// </summary>

        public GameObject Prefab
        {
            get { return _prefab; }
            set { _prefab = value; }
        }

        /// <summary>
        /// The size of the pool.
        /// </summary>

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        #endregion
    }
}