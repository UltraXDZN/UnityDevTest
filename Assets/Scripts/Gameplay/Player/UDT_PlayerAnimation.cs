using UnityEngine;

namespace UDT.Gameplay
{
    public class UDT_PlayerAnimation
    {
        private Animator m_playerAnimator;

        public UDT_PlayerAnimation(Animator animator)
        {
            m_playerAnimator = animator;
        }

        /// <summary>
        /// Updates the player's animations
        /// </summary>
        public void UpdateAnimations()
        {
            m_playerAnimator.SetBool("HasSlashed", false);

            if (Input.GetKeyDown(KeyCode.E))
            {
                m_playerAnimator.SetBool("IsInBattle", !m_playerAnimator.GetBool("IsInBattle"));
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_playerAnimator.SetBool("HasSlashed", true);
            }
        }

        /// <summary>
        /// Updates the player's movement animations
        /// </summary>
        /// <param name="direction"></param>
        public void UpdateMovementAnimation(Vector3 direction)
        {
            float speedPercent = direction.magnitude;
            m_playerAnimator.SetFloat("Speed", speedPercent);
        }
    }
}