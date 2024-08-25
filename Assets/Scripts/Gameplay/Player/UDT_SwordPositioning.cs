using UnityEngine;

namespace UDT.Gameplay.Player
{
    public class UDT_SwordPositioning
    {
        private Transform m_playerSwordTransform;
        private Transform m_playerHandTransform;
        private Transform m_playerBackTransform;

        public UDT_SwordPositioning(Transform swordTransform, Transform handTransform, Transform backTransform)
        {
            m_playerSwordTransform = swordTransform;
            m_playerHandTransform = handTransform;
            m_playerBackTransform = backTransform;
        }
        /// <summary>
        /// Moves the sword to the player's hand
        /// </summary>
        public void MoveSwordToHand()
        {
            MoveSwordTo(m_playerHandTransform);
        }

        /// <summary>
        /// Moves the sword to the player's back
        /// </summary>
        public void MoveSwordToBack()
        {
            MoveSwordTo(m_playerBackTransform);
        }

        /// <summary>
        /// Moves the sword from current transform to the target transform
        /// </summary>
        /// <param name="target"></param>
        private void MoveSwordTo(Transform target)
        {
            m_playerSwordTransform.SetParent(target);
            m_playerSwordTransform.localPosition = Vector3.zero;
            m_playerSwordTransform.localRotation = Quaternion.identity;
        }
    }
}