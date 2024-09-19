using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UDT.Gameplay.AI
{
    using static Settings.UDT_Constants;
    public class UDT_Fish : MonoBehaviour
    {
        [Header("Fish Settings")]
        [SerializeField] private float m_moveSpeed = FISH_SPEED;
        [SerializeField] private float m_scaredSpeedMultiplier = SCARED_SPEED_MULTIPLIER;
        [SerializeField] private float m_changeDirectionInterval = CHANGE_DIRECTION_INTERVAL;
        [SerializeField] private UDT_AIArea m_areaOfMovement;
        [SerializeField] private float m_detectionRadius = DETECTION_RADIUS;

        [SerializeField] private GameObject m_playerObject;

        private NavMeshAgent navMeshAgent;
        private bool isScared;
        private Vector3 targetPosition;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = m_moveSpeed;
            StartCoroutine(ChangeDirection());
        }

        private void Update()
        {
            if (IsPlayerInFearRange())
            {
                SetScared(m_playerObject.transform.position);
            }
            else
            {
                ResetScared();
            }
            navMeshAgent.SetDestination(targetPosition);

        }

        /// <summary>
        /// Coroutine that changes the direction of the fish at a set interval.
        /// </summary>
        private IEnumerator ChangeDirection()
        {
            while (true)
            {
                if (!isScared)
                {
                    targetPosition = m_areaOfMovement.GenerateRandomPosition();
                }
                yield return new WaitForSeconds(m_changeDirectionInterval);
            }
        }

        /// <summary>
        /// Checks if the player is within the fear range of the fish.
        /// </summary>
        /// <returns>True if the player is within the fear range, false otherwise</returns>
        private bool IsPlayerInFearRange()
        {
            float distanceSqr = (transform.position - m_playerObject.transform.position).sqrMagnitude;
            return distanceSqr <= Mathf.Pow(m_detectionRadius, FEAR_RANGE_POWER);
        }

        /// <summary>
        /// Sets the fish to be scared and moves it away from the player.
        /// </summary>
        /// <param name="playerPosition">The position of the player</param>
        public void SetScared(Vector3 playerPosition)
        {
            isScared = true;
            targetPosition = (transform.position - playerPosition).normalized * m_scaredSpeedMultiplier + transform.position;
        }

        /// <summary>
        /// Resets the fish to not be scared and moves it to a random position.
        /// </summary>
        public void ResetScared()
        {
            isScared = false;
            targetPosition = m_areaOfMovement.GenerateRandomPosition();
        }

#region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_detectionRadius); 
        }
#endregion
    }
}