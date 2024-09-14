using UDT.Gameplay.Player;
using UnityEngine;
using UnityEngine.AI;

namespace UDT.Gameplay.AI
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class UDT_AINPC : MonoBehaviour
    {
        protected NavMeshAgent m_agent;
        enum NPCState { Idle, Patrolling, Chasing, Attacking, RunningAway };

        [SerializeField] protected Vector3 m_startingPosition;
        [SerializeField] protected UDT_AIArea m_areaOfMovement;
        [SerializeField] protected Transform m_player;
        [SerializeField] protected float m_playerDetectionRange = 10f; // Adjust this value to set the detection range

        protected UDT_Player m_playerScript;

        protected virtual void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_startingPosition = transform.position;
            m_playerScript = m_player.GetComponent<UDT_Player>();
        }

        protected virtual void DestinationReached()
        {
            if (!m_agent.pathPending && m_agent.remainingDistance <= m_agent.stoppingDistance && (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f))
            {
                m_agent.SetDestination(m_areaOfMovement.GeneratedPosition);
            }
        }

    }
}