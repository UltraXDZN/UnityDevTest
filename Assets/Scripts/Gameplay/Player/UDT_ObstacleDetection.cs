using System;
using UnityEngine;

namespace UDT.Gameplay.Player
{
    using static Settings.UDT_Constants;

    public class UDT_ObstacleDetection
    {
        private UDT_Player m_playerController;
        private LayerMask m_worldLayer;
        
        private Vector3 m_wallNormal;
        private UDT_InverseKinematics m_inverseKinematics;
        private UDT_PlayerMovement m_playerMovement;



        public UDT_ObstacleDetection(UDT_Player playerController, LayerMask worldLayer, UDT_InverseKinematics inverseKinematics, UDT_PlayerMovement playerMovement)
        {
            m_playerController = playerController;
            m_worldLayer = worldLayer;
            m_inverseKinematics = inverseKinematics;
            m_playerMovement = playerMovement;
        }

        /// <summary>
        /// Check for obstacles in front of the player
        /// </summary>
        public void CheckForObstacles()
        {
            Ray ray = new Ray(new Vector3(m_playerController.transform.position.x, m_playerController.transform.position.y + 0.5f, m_playerController.transform.position.z), m_playerController.transform.forward);

            RaycastHit _hit;

            if (Physics.Raycast(ray, out _hit, OBSTACLE_RAY_DISTANCE_DEFAULT, m_worldLayer))
            {
                if (_hit.collider.CompareTag("Wall"))
                {
                    SlowDownPlayerUponHit(_hit);
                }
            }
            UpdateWallNormal(_hit.normal);
            
        }

        /// <summary>
        /// Slow down the player upon hitting an obstacle (Wall)
        /// </summary>
        /// <param name="_hit"></param>
        private void SlowDownPlayerUponHit(RaycastHit _hit)
        {
            m_playerMovement.CurrentSpeed -= m_playerMovement.CurrentSpeed * Time.deltaTime;
            if (_hit.distance < OBSTACLE_RAY_DISTANCE_MIN)
            {
                m_playerMovement.CurrentSpeed = DEFAULT_SPEED;
            }
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 start = new Vector3(m_playerController.transform.position.x, m_playerController.transform.position.y + 0.5f, m_playerController.transform.position.z);
            Gizmos.DrawRay(start, m_playerController.transform.forward * OBSTACLE_RAY_DISTANCE_DEFAULT);
        }

        /// <summary>
        /// Update the normal of the wall that the player is facing
        /// </summary>
        /// <param name="normal"></param>
        private void UpdateWallNormal(Vector3 normal)
        {
            m_inverseKinematics.SetWallNormal(normal);
        }
    }
}