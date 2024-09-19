using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UDT.Gameplay.AI
{
    using static UDT.Settings.UDT_Constants;

    public class UDT_AIVillager : UDT_AINPC
    {
        [SerializeField] private bool m_isRespawning = false;
        private Coroutine respawnCoroutine = null;

        protected override void Start()
        {
            base.Start();
        }

        protected override void HandleState()
        {
            if (m_isRespawning)
                return;

            switch (currentState)
            {
                case NPCState.Patrolling:
                    if (IsPlayerInRange() && m_playerScript.BattleStateStatus)
                    {
                        currentState = NPCState.RunningAway;
                        StartRunningAway();
                    }
                    else
                    {
                        base.HandleState();
                    }
                    break;

                case NPCState.Chasing:
                case NPCState.Attacking:
                case NPCState.Idle:
                    base.HandleState();
                    break;

                case NPCState.RunningAway:
                    break;
            }
        }

        /// <summary>
        /// Initiates the RunningAway behavior by starting the coroutine.
        /// </summary>
        private void StartRunningAway()
        {
            if (respawnCoroutine == null)
            {
                respawnCoroutine = StartCoroutine(RunAwayAndRespawn());
            }
        }

        /// <summary>
        /// Coroutine that handles running away, disabling components, waiting, and respawning.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunAwayAndRespawn()
        {
            m_isRespawning = true;
            RunAwayAgentSettings();

            Vector3 runDirection = (transform.position - m_player.position).normalized;
            Vector3 offCameraPosition = transform.position + runDirection * VILLAGER_OUT_OF_SCREEN_LOCATION_MULTIPLIER;

            if (m_agent != null && m_agent.isOnNavMesh && m_agent.enabled)
            {
                m_agent.SetDestination(offCameraPosition);

                while (!m_agent.pathPending && m_agent.remainingDistance > m_agent.stoppingDistance)
                {
                    if (m_agent.pathStatus == NavMeshPathStatus.PathInvalid || m_agent.pathStatus == NavMeshPathStatus.PathPartial)
                        break;

                    yield return null;
                }
            }
            else
            {
                transform.position = offCameraPosition;
            }

            SetComponentsEnabled(false);

            yield return new WaitUntil(() => !m_playerScript.BattleStateStatus);

            Respawn();

            SetComponentsEnabled(true);

            currentState = NPCState.Patrolling;
            m_isRespawning = false;
            respawnCoroutine = null;
            m_maxSpeed = AI_DEFAULT_MAX_SPEED;
        }

        /// <summary>
        /// Adjusts the NavMeshAgent settings for running away.
        /// </summary>
        private void RunAwayAgentSettings()
        {
            m_maxSpeed = VILLAGER_RUN_AWAY_SPEED;
            m_agent.speed = m_maxSpeed; 
            m_agent.acceleration = VILLAGER_RUN_AWAY_ACCELERATION;
            m_agent.autoBraking = false;
            m_agent.isStopped = false;
        }

        /// <summary>
        /// Enables or disables key components.
        /// </summary>
        /// <param name="enabled"></param>
        private void SetComponentsEnabled(bool enabled)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Collider collider = GetComponent<Collider>();

            if (meshRenderer != null)
                meshRenderer.enabled = enabled;

            if (collider != null)
                collider.enabled = enabled;

            // Note: NavMeshAgent is handled separately in Respawn and during running away state
        }

        /// <summary>
        /// Respawns the NPC by resetting position and NavMeshAgent properties.
        /// </summary>
        private void Respawn()
        {
            if (m_agent != null && m_agent.isOnNavMesh && m_agent.enabled)
            {
                m_agent.ResetPath();
                m_agent.speed = m_maxSpeed;
                m_agent.acceleration = m_acceleration;
                m_agent.autoBraking = true; 
                m_agent.SetDestination(m_startingPosition);
            }
            else
            {
                transform.position = m_startingPosition;
                transform.rotation = Quaternion.identity;
            }
        }
    }
}