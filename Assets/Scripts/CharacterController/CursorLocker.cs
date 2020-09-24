using UnityEngine;

namespace Assets.Scripts.CharacterController
{
    public class CursorLocker : MonoBehaviour
    {
        #region FIELDS

        private bool _isCursorLocked = true;
        private bool _shouldLockCursor = true;

        #endregion

        #region PROPERTIES

        public bool IsCursorLocked
        {
            get { return _isCursorLocked; }
            set { _isCursorLocked = value; }
        }

        public bool ShouldLockCursor
        {
            get { return _shouldLockCursor; }
            set { _shouldLockCursor = value; }
        }

        #endregion

        #region METHODS

        public void SetCursorLock(bool value)
        {
            ShouldLockCursor = value;
            if (!ShouldLockCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void UpdateCursorLock()
        {
            if (ShouldLockCursor)
            {
                InternalLockUpdate();
            }
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                IsCursorLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                IsCursorLocked = true;
            }

            if (IsCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Update()
        {
            UpdateCursorLock();
        }

        #endregion
    }
}
