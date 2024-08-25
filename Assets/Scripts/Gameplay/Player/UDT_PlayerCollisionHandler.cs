using System.Runtime.InteropServices;
using UnityEngine;

namespace UDT.Gameplay.Player
{
    public class UDT_PlayerCollisionHandler
    {
        private Animator m_playerAnimator;
        private BoxCollider m_playerColliderCrouch;
        private CapsuleCollider m_playerColliderStand;

        public UDT_PlayerCollisionHandler(Animator animator, BoxCollider crouchCollider, CapsuleCollider standCollider)
        {
            m_playerAnimator = animator;
            m_playerColliderCrouch = crouchCollider;
            m_playerColliderStand = standCollider;
        }

        /// <summary>
        /// Sets the Crouching state of the player
        /// </summary>
        /// <param name="other"></param>
        public void HandleTriggerEnter(Collider other)
        {
            if (other.CompareTag("Branch"))
            {
                ChangeCrouchingState(true);
            }
        }

        /// <summary>
        /// Sets the Standing state of the player
        /// </summary>
        /// <param name="other"></param>
        public void HandleTriggerExit(Collider other)
        {
            if (other.CompareTag("Branch"))
            {
                ChangeCrouchingState(false);
            }
        }

        /// <summary>
        /// Changes the player's crouching state
        /// </summary>
        /// <param name="shouldCrouch"></param>
        private void ChangeCrouchingState(bool shouldCrouch)
        {
            m_playerAnimator.SetBool("IsCrouching", shouldCrouch);
            EnableStateCollider(!shouldCrouch);
        }

        /// <summary>
        /// Enables the collider based on the player's state
        /// </summary>
        /// <param name="isStanding"></param>
        private void EnableStateCollider(bool isStanding)
        {
            m_playerColliderCrouch.enabled = !isStanding;
            m_playerColliderStand.enabled = isStanding;
        }
    }
}