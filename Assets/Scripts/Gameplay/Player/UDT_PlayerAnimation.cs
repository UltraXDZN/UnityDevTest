using UDT.Gameplay.Player;
using UnityEngine;

namespace UDT.Gameplay
{
    public class UDT_PlayerAnimation
    {
        private Animator m_playerAnimator;
        private UDT_Player m_player;

        public UDT_PlayerAnimation(UDT_Player player, Animator animator)
        {
            m_playerAnimator = animator;
            m_player = player;
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
                //m_player.DealDamage();
            }
            if (!m_player.IsAlive){
                m_playerAnimator.SetTrigger("IsDead");
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