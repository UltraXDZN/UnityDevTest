using System.Collections;
using UnityEngine;

namespace UDT.Gameplay.AI
{
    using static Settings.UDT_Constants;
    public class UDT_AIGuard : UDT_AINPC
    {
        [Header("Guard Settings")]
        [SerializeField] private float m_attackRadius = GUARD_ATTACK_RADIUS;
        [SerializeField] private float m_chasingRadius = GUARD_DETECTION_RADIUS;
        [SerializeField] private float m_attackCooldown = GUARD_ATTACK_COOLDOWN; 
        [SerializeField] private float m_guardHealth = GUARD_HEALTH;

        private bool isAttacking = false;


        protected override void Start()
        {
            base.Start();
        }

        protected override void HandleState()
        {
            base.HandleState();

            switch (currentState)
            {
                case NPCState.Patrolling:
                    currentState = CheckPlayerRangeStatus(NPCState.Chasing);
                    break;

                case NPCState.Chasing:
                    ChasePlayer();
                    if (IsPlayerInAttackRange())
                    {
                        currentState = NPCState.Attacking;
                    }
                    break;

                case NPCState.Attacking:
                    if (!IsPlayerInAttackRange())
                    {
                        currentState = NPCState.Chasing;
                    }
                    if (!isAttacking)
                    {
                        StartCoroutine(AttackPlayer());
                    }
                    break;
            }
        }

        /// <returns>Returns if the player is within the guard's detection range</returns>
        protected override bool IsPlayerInAttackRange()
        {
            float distanceSqr = (transform.position - m_player.position).sqrMagnitude;
            return distanceSqr <= Mathf.Pow(m_attackRadius, GUARD_RANGE_POWER);
        }

        /// <summary>
        /// Coroutine to attack the player
        /// </summary>
        protected override IEnumerator AttackPlayer()
        {
            isAttacking = true;

            while (m_playerScript.IsAlive && currentState == NPCState.Attacking)
            {
                m_playerScript.DecreaseHealth(GUARD_DAMAGE);
                m_playerScript.SelectedGuard = this;

                yield return new WaitForSeconds(m_attackCooldown);
            }

            isAttacking = false;

            if (!m_playerScript.IsAlive)
            {
                ReturnToPatrol();
            }
            else
            {
                currentState = NPCState.Chasing;
            }
        }

        protected override void ChasePlayer()
        {
            base.ChasePlayer();
        }

        /// <summary>
        /// Public endpoint to deal damage to "this" guard.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            DecreaseHealth(damage);
        }

        /// <summary>
        /// Decreases the guard's health by the given damage amount.
        /// </summary>
        /// <param name="damage"></param>
        private void DecreaseHealth(int damage)
        {
            m_guardHealth -= damage;
            if (m_guardHealth <= GUARD_DEATH_HEALTH)
            {
                Destroy(gameObject);
                m_playerScript.SelectedGuard = null;
            }
        }
#region Gizmos
        private void OnDrawGizmosSelected()
        {
            // Draw pursuit and attack radii
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, m_chasingRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_attackRadius);
        }
#endregion
    }
}