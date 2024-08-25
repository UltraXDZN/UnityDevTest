using System;
using UnityEngine;

namespace UDT.Gameplay.Player
{
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

        private UDT_PlayerMovement m_playerMovement;
        private UDT_PlayerAnimation m_playerAnimation;
        private UDT_CameraController m_cameraController;
        private UDT_InverseKinematics m_inverseKinematics;
        private UDT_ObstacleDetection m_obstacleDetection;
        private UDT_SwordPositioning m_swordPositioning;
        private UDT_PlayerCollisionHandler m_collisionHandler;


        private void Awake()
        {
            m_playerRigidbody = GetComponent<Rigidbody>();
            InitializeComponents();
        }

        /// <summary>
        /// Initializes all the components of the player
        /// </summary>
        private void InitializeComponents()
        {
            m_playerAnimation = new UDT_PlayerAnimation(m_playerAnimator);
            m_playerMovement = new UDT_PlayerMovement(m_playerRigidbody, m_playerAnimator, m_playerAnimation);
            m_cameraController = new UDT_CameraController(m_cameraPivotTransform, transform);
            m_inverseKinematics = new UDT_InverseKinematics(m_playerAnimator, m_worldLayer, m_playerRightShoulderTransform, m_playerLeftShoulderTransform);
            m_obstacleDetection = new UDT_ObstacleDetection(this, m_worldLayer, m_inverseKinematics, m_playerMovement);
            m_swordPositioning = new UDT_SwordPositioning(m_playerSwordTransform, m_playerHandTransform, m_playerBackTransform);
            m_collisionHandler = new UDT_PlayerCollisionHandler(m_playerAnimator, m_playerColliderCrouch, m_playerColliderStand);
        }

        private void Update()
        {
            m_playerMovement.HandleMovement();
            m_playerAnimation.UpdateAnimations();
            m_cameraController.FollowPlayer();
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
            m_collisionHandler.HandleTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            m_collisionHandler.HandleTriggerExit(other);
        }
    }
}