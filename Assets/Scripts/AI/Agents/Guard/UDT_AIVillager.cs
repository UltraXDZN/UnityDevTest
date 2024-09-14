using UnityEngine;

namespace UDT.Gameplay.AI
{
    using System;
    using System.Collections;
    using Player;
    using Unity.VisualScripting;
    using UnityEngine.AI;

    public class UDT_AIVillager : UDT_AINPC
    {
        [SerializeField] private bool m_isRespawning = false;

        protected override void Start()
        {
            base.Start();
        }

        protected void Update()
        {
            if (GetComponent<NavMeshAgent>().enabled){
                DestinationReached();
                Debug.Log("Villager AI Update");
                CheckPlayerDetection();
                CheckRespawn();
            }
        }

        private void CheckPlayerDetection()
        {
            float distanceSqr = (transform.position - m_player.position).sqrMagnitude;
            bool isWithinDetectionRange = distanceSqr <= Math.Pow(m_playerDetectionRange, 2);

            m_agent.isStopped = isWithinDetectionRange && m_playerScript.BattleStateStatus;
            m_agent.autoBraking = isWithinDetectionRange && m_playerScript.BattleStateStatus;

            if (isWithinDetectionRange && m_playerScript.BattleStateStatus)
            {
                m_isRespawning = true;
                StartCoroutine(RunOffCameraAndDisable());
            }
            else
            {
                transform.LookAt(m_player.position, Vector3.up);
            }
        }

        private IEnumerator RunOffCameraAndDisable()
        {
            RunAwayAgentSettings();

            Vector3 offCameraPosition = transform.position + (transform.forward * 50f);

            if (m_agent != null && m_agent.isOnNavMesh && m_agent.enabled)
            {
                m_agent.SetDestination(offCameraPosition);
                while (m_agent.isActiveAndEnabled && m_agent.remainingDistance > 0.1f && m_playerScript.BattleStateStatus)
                {
                    yield return null;
                }
            }
            else
            {
                // If the NavMeshAgent is not active or not placed on the NavMesh, move the object directly to the off-camera position
                transform.position = offCameraPosition;
            }

            if (m_playerScript.BattleStateStatus)
            {
                m_isRespawning = true;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<Collider>().enabled = false;
                yield return new WaitUntil(() => !m_playerScript.BattleStateStatus);
                Respawn();
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<NavMeshAgent>().enabled = true;
                GetComponent<Collider>().enabled = true;
            }
        }
        private void CheckRespawn()
        {
            if (!m_playerScript.BattleStateStatus && m_isRespawning)
            {
                Respawn();
            }
        }
        
        private void Respawn()
        {
            if (m_agent != null && m_agent.isOnNavMesh && m_agent.enabled)
            {
                m_agent.acceleration = 0.1f;
                m_agent.speed = 2f;
                m_agent.Warp(m_startingPosition);
                m_agent.isStopped = false;
                m_agent.autoBraking = false;
                m_isRespawning = false;
            }
            else
            {
                // If the NavMeshAgent is not active or not placed on the NavMesh, simply reset the object's position and rotation
                transform.position = m_startingPosition;
                transform.rotation = Quaternion.identity;
                m_isRespawning = false;
            }
        }



        private void RunAwayAgentSettings()
        {
            m_agent.speed = 10f;
            m_agent.acceleration = 1000f;
            m_agent.autoBraking = false;
            m_agent.isStopped = false;
        }
    }
}