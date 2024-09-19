using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UDT.Gameplay.AI
{
    using Player;
    using static UDT.Settings.UDT_Constants;

    [RequireComponent(typeof(NavMeshAgent))]
    public class UDT_AINPC : MonoBehaviour
    {
        protected NavMeshAgent m_agent;

        protected enum NPCState { Idle, Patrolling, Chasing, Attacking, RunningAway };
        protected NPCState currentState = NPCState.Patrolling;

        [SerializeField] protected Vector3 m_startingPosition;
        [SerializeField] protected UDT_AIArea m_areaOfMovement;
        [SerializeField] protected Transform m_player;
        [SerializeField] protected float m_playerDetectionRange = AI_DETECTION_RANGE;
        
        [SerializeField] protected float m_maxSpeed = AI_DEFAULT_MAX_SPEED;
        [SerializeField] protected float m_minSpeed = AI_DEFAULT_MIN_SPEED;
        [SerializeField] protected float m_slowingDistance = AI_DEFAULT_SLOWING_DISTANCE;
        [SerializeField] protected float m_acceleration = AI_DEFAULT_ACCELERATION;
        [SerializeField] protected float m_deceleration = AI_DEFAULT_DECELERATION;

        protected UDT_Player m_playerScript;

        protected virtual void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_startingPosition = transform.position;
            m_playerScript = m_player.GetComponent<UDT_Player>();

            m_agent.speed = m_maxSpeed;
            m_agent.acceleration = m_acceleration;
            m_agent.stoppingDistance = AI_DEFAULT_STOPPING_DISTANCE;

            SetNextPatrolDestination();
        }

        protected virtual void Update()
        {
            HandleState();
            AdjustSpeedBasedOnDistance();
        }

        /// <summary>
        /// Handles the NPC's state based on the current state. To be overridden by derived NPC types.
        /// </summary>
        protected virtual void HandleState()
        {
            switch (currentState)
            {
                case NPCState.Patrolling:
                    if (!m_agent.pathPending && m_agent.remainingDistance <= m_agent.stoppingDistance)
                    {
                        SetNextPatrolDestination();
                    }
                    break;

                case NPCState.Chasing:
                case NPCState.Attacking:
                case NPCState.Idle:
                case NPCState.RunningAway:
                    break;
            }
        }

        /// <summary>
        /// Checks if the player is within range and sets the state to the referenced state if it is.
        /// </summary>
        /// <param name="endState"></param>
        /// <returns>State an AI is in</returns>
        protected NPCState CheckPlayerRangeStatus(NPCState endState){
            if (IsPlayerInRange() && m_playerScript.BattleStateStatus)
            {
                return endState;
            }
            return NPCState.Patrolling;
        }

        protected bool IsPlayerInRange()
        {
            float distanceSqr = (transform.position - m_player.position).sqrMagnitude;
            return distanceSqr <= Mathf.Pow(m_playerDetectionRange, GUARD_RANGE_POWER);
        }

        protected virtual bool IsPlayerInAttackRange()
        {
            // To be overridden by derived Guard class
            return false;
        }

        /// <summary>
        /// Chases the player by setting the destination to the player's position.
        /// </summary>
        protected virtual void ChasePlayer()
        {
            if (m_agent != null && m_agent.isOnNavMesh && m_agent.enabled)
            {
                m_agent.SetDestination(m_player.position);
            }
        }

        /// <summary>
        /// Sets the next patrol destination for the NPC.
        /// </summary>
        protected virtual void SetNextPatrolDestination()
        {
            Vector3 newDestination = m_areaOfMovement.GenerateRandomPosition();
            if (newDestination != Vector3.zero)
            {
                m_agent.SetDestination(newDestination);
            }
            else
            {
                Debug.LogWarning("Generated patrol destination is Vector3.zero. Please check UDT_AIArea settings.");
            }
        }

        /// <summary>
        /// Coroutine to attack the player. To be overridden by derived NPC types.
        /// </summary>
        protected virtual IEnumerator AttackPlayer()
        {
            yield break;
        }

        /// <summary>
        /// Returns the NPC to patrolling state.
        /// </summary>
        protected virtual void ReturnToPatrol()
        {
            currentState = NPCState.Patrolling;
            SetNextPatrolDestination();
        }

        /// <summary>
        /// Adjusts the NavMeshAgent's speed based on the remaining distance to the destination.
        /// The agent slows down as it approaches the target.
        /// </summary>
        protected virtual void AdjustSpeedBasedOnDistance()
        {
            if (m_agent.pathPending || m_agent.destination == null)
                return;

            float remainingDistance = m_agent.remainingDistance;

            if (remainingDistance <= m_slowingDistance && remainingDistance > m_agent.stoppingDistance)
            {
                float speedRatio = remainingDistance / m_slowingDistance;
                m_agent.speed = Mathf.Lerp(m_minSpeed, m_maxSpeed, speedRatio);
            }
            else if (remainingDistance <= m_agent.stoppingDistance)
            {
                m_agent.velocity = Vector3.Lerp(m_agent.velocity, Vector3.zero, Time.deltaTime * m_deceleration);
            }
            else
            {
                m_agent.speed = m_maxSpeed;
            }

            m_agent.speed = Mathf.Max(m_agent.speed, AI_DEFAULT_MIN_SPEED);
        }
    }
}
