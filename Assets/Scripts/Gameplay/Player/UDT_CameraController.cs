using UnityEngine;

namespace UDT.Gameplay.Player
{
    public class UDT_CameraController
    {
        private Transform m_cameraPivotTransform;
        private Transform m_playerTransform;

        public UDT_CameraController(Transform cameraPivot, Transform player)
        {
            m_cameraPivotTransform = cameraPivot;
            m_playerTransform = player;
        }
        
        /// <summary>
        /// Follows the player's position
        /// </summary>
        public void FollowPlayer()
        {
            m_cameraPivotTransform.position = m_playerTransform.position;
        }
    }
}