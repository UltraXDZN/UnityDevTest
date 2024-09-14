using UnityEngine;

namespace UDT.Gameplay.Player
{
    public class UDT_SwordPositioning
    {
        private Transform m_playerSwordTransform;

        public UDT_SwordPositioning(Transform swordTransform)
        {
            m_playerSwordTransform = swordTransform;
        }

        /// <summary>
        /// Moves the sword from current transform to the target transform
        /// </summary>
        /// <param name="target"></param>
        public void MoveSwordTo(Transform target)
        {
            m_playerSwordTransform.SetParent(target);
            m_playerSwordTransform.localPosition = Vector3.zero;
            m_playerSwordTransform.localRotation = Quaternion.identity;
        }
    }
}