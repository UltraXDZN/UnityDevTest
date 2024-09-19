
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UDT.Gameplay.Player
{
    using AI;
    using UDT.Gameplay.UI;
    using static Settings.UDT_Constants;

    [RequireComponent(typeof(Rigidbody))]
    public class UDT_Player : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField] private Rigidbody m_playerRigidbody;
        [SerializeField] private Transform m_cameraPivotTransform;

        [Header("Armature Transforms")]
        [SerializeField] private Transform m_playerHandTransform;
        [SerializeField] private Transform m_playerBackTransform;
        [SerializeField] private Transform m_playerLeftShoulderTransform;
        [SerializeField] private Transform m_playerRightShoulderTransform;

        [Header("Weapon Transforms")]
        [SerializeField] private Transform m_playerSwordTransform;
        [SerializeField] private Animator m_playerAnimator;
        [SerializeField] private LayerMask m_worldLayer;

        [SerializeField] private BoxCollider m_playerColliderCrouch;
        [SerializeField] private CapsuleCollider m_playerColliderStand;

        [SerializeField] private bool m_battleStateStatus = false;

        [SerializeField] private string m_tagField;

        [SerializeField] private BoxCollider[] m_affectedColliders;

        [SerializeField] private int m_playerHealth = PLAYER_MAX_HEALTH;
        [SerializeField] private Slider m_healthBar;
        [SerializeField] private UDT_PauseMenu m_pauseMenu;

        private UDT_AIGuard m_selectedGuard = null;

        private UDT_PlayerMovement m_playerMovement;
        private UDT_PlayerAnimation m_playerAnimation;
        private UDT_CameraController m_cameraController;
        private UDT_InverseKinematics m_inverseKinematics;
        private UDT_ObstacleDetection m_obstacleDetection;
        private UDT_SwordPositioning m_swordPositioning;
        private UDT_PlayerCollisionHandler m_collisionHandler;


        public bool IsAlive { get => m_playerHealth > PLAYER_DEATH_HEALTH; }

        public bool BattleStateStatus { get => m_battleStateStatus; set => m_battleStateStatus = value; }
        public UDT_AIGuard SelectedGuard { set => m_selectedGuard = value; }


        private void Awake()
        {
            m_playerRigidbody = GetComponent<Rigidbody>();
            InitializeComponents();
        }

        private void Start()
        {
            m_affectedColliders = GameObject.FindGameObjectsWithTag(m_tagField).SelectMany(go => go.GetComponents<BoxCollider>()).ToArray();
        }

        /// <summary>
        /// Initializes all the components of the player
        /// </summary>
        private void InitializeComponents()
        {
            m_playerAnimation = new UDT_PlayerAnimation(this, m_playerAnimator);
            m_playerMovement = new UDT_PlayerMovement(m_playerRigidbody, m_playerAnimator, m_playerAnimation);
            m_cameraController = new UDT_CameraController(m_cameraPivotTransform, transform);
            m_inverseKinematics = new UDT_InverseKinematics(m_playerAnimator, m_worldLayer, m_playerRightShoulderTransform, m_playerLeftShoulderTransform);
            m_obstacleDetection = new UDT_ObstacleDetection(this, m_worldLayer, m_inverseKinematics, m_playerMovement);
            m_swordPositioning = new UDT_SwordPositioning(m_playerSwordTransform);
            m_collisionHandler = new UDT_PlayerCollisionHandler(m_playerAnimator, m_playerColliderCrouch, m_playerColliderStand);
        }

        private void Update()
        {
            m_playerMovement.HandleMovement();
            m_cameraController.FollowPlayer();
            m_playerAnimation.UpdateAnimations();
            UpdateGate();
        }

        private void FixedUpdate()
        {
            m_playerMovement.ApplyMovement();
            m_obstacleDetection.CheckForObstacles();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            m_inverseKinematics.ApplyIK(layerIndex);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wall Gate" || other.tag == "Branch")
            {
                m_collisionHandler.HandleTriggerEnter(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Wall Gate" || other.tag == "Branch")
            {
                m_collisionHandler.HandleTriggerExit(other);
            }
        }

        /// <summary>
        /// Updates the gate collider based on the player's battle status
        /// </summary>
        private void UpdateGate()
        {

            ApplyBattleStatus();
            foreach (BoxCollider _wallGate in m_affectedColliders)
            {
                _wallGate.enabled = BattleStateStatus;
            }
        }

        /// <summary>
        /// Updates the player's battle state status
        /// </summary>
        private void ApplyBattleStatus()
        {
            BattleStateStatus = GetBattleStatus();
        }

        /// <returns>Player's battle status</returns>
        private bool GetBattleStatus()
        {
            return m_playerAnimator.GetBool("IsInBattle");
        }

        /// <summary>
        /// Moves the sword to the player's hand
        /// </summary>
        public void MoveSwordToHand()
        {
            m_swordPositioning.MoveSwordTo(m_playerHandTransform);
        }

        /// <summary>
        /// Moves the sword to the player's back
        /// </summary>
        public void MoveSwordToBack()
        {
            m_swordPositioning.MoveSwordTo(m_playerBackTransform);
        }

        /// <summary>
        /// Decreases the player's health by the given damage amount.
        /// </summary>
        public void DecreaseHealth(int damage)
        {
            m_playerHealth -= damage;
            updateSlider();
            if (m_playerHealth <= PLAYER_DEATH_HEALTH)
            {
                Die();
            }
        }

        /// <summary>
        /// Finds and returns the closest AI Guard to the player within the detection range.
        /// </summary>
        /// <returns>The closest AI Guard GameObject, or null if none are found within range.</returns>
        public GameObject GetClosestAIGuard()
        {
            GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard AI");
            GameObject closestGuard = null;

            float shortestDistance = Mathf.Infinity;
            Vector3 playerPosition = transform.position;

            foreach (GameObject guard in guards)
            {
                float distance = Vector3.Distance(guard.transform.position, playerPosition);
                float detectionRange = AI_DETECTION_RANGE;

                if (distance < shortestDistance && distance <= detectionRange)
                {
                    shortestDistance = distance;
                    closestGuard = guard;
                }
            }

            return closestGuard;
        }

        /// <summary>
        /// Deals damage to the selected AI Guard.
        /// </summary>
        public void DealDamage()
        {
            int damage = DEFAULT_DAMAGE;

            try {
                m_selectedGuard = GetClosestAIGuard().GetComponent<UDT_AIGuard>();
                m_selectedGuard.TakeDamage(damage);
            }
            catch {
                Debug.LogError("No guard found!");
            }
        }

        /// <summary>
        /// Opens the pause menu when the player dies.
        /// </summary>
        public void Die()
        {
            m_pauseMenu.TogglePauseMenu();
        }

        private void updateSlider()
        {
            m_healthBar.maxValue = PLAYER_MAX_HEALTH;
            m_healthBar.value = m_playerHealth;
        }

    }
}