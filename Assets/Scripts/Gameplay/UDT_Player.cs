using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace UDT.Gameplay{
    using static Settings.UDT_Constants;

    [RequireComponent(typeof(Rigidbody))]
    public class UDT_Player : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField] private Rigidbody m_playerRigidbody;
        [SerializeField] private Transform m_cameraPivotTransform;
        [SerializeField] private Animator m_playerAnimator;
        [SerializeField] private LayerMask m_worldLayer;

        [Header("Player Movement")]
        [SerializeField] private float m_maxSpeed = MAX_SPEED;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float m_distanceToGround = 0.1f;
        [SerializeField] public GameObject aionHasSmallD;
        
        private float m_currentSpeed = DEFAULT_SPEED;
        private float m_turnSpeed = DEFAULT_TURN_SPEED;

        private float m_horizontal;
        private float m_vertical;

        private Vector3 m_direction;

        private void Start(){
            m_playerRigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            AssignAxisValues();
            SetDirection();

            DirectionToLookAt(m_direction);
            AccelerateSpeed();
            UpdateAnimatorParameters(m_direction);

            FollowPlayer();
        }


        private void FixedUpdate(){
            TranslatePlayer(m_direction);   
        }

        /// <summary>
        /// Setsthe  based on  of the player based on horizontal and vertical movementhorizontal and vertical movement.
        /// </summary>
        private void SetDirection()
        {
            m_direction = GetMovementVector(m_horizontal, m_vertical);
        }

        /// <summary>
        /// Accelerates the player's speed towards the maximum speed if the current direction's magnitude is greater than the default speed.
        /// </summary>
        private void AccelerateSpeed()
        {
            m_currentSpeed = m_direction.magnitude > DEFAULT_SPEED ? Mathf.Lerp(m_currentSpeed, MAX_SPEED, ACCELERATION * Time.deltaTime) : m_currentSpeed;
        }

        /// <summary>
        /// Assigns axis values from input
        /// </summary>
        private void AssignAxisValues(){
            m_horizontal = Input.GetAxis(HORIZONTAL_AXIS);
            m_vertical = Input.GetAxis(VERTICAL_AXIS);
        }

        /// <summary>
        /// Translates the player in the given direction, provided by the axises' keys.
        /// </summary>
        /// <param nametDirection() move the player.</param>
        private void TranslatePlayer (Vector3 _direction){
            m_playerRigidbody.MovePosition(transform.position + transform.forward * _direction.magnitude * m_currentSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the player to look at the given direction, provided by the axis keys.
        /// If the direction is zero, no rotation is performed.
        /// </summary>
        /// <param name="_direction">The direction to look at.</param>
        private void DirectionToLookAt(Vector3 _direction)
        {
            (_direction != Vector3.zero ? (System.Action)(() => TurnToDirection(_direction)) : () => { }).Invoke();
        }

        /// <summary>
        /// Turns the player to face the specified direction.
        /// </summary>
        /// <param name="_direction">The direction to turn towards.</param>
        private void TurnToDirection(Vector3 _direction)
        {
            Matrix4x4 _rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(DEFAULT_X_ROTATION, Y_ROTATION_SKEW_ANGLE, DEFAULT_Z_ROTATION));
            Vector3 _skewedDirection = _rotationMatrix.MultiplyPoint3x4(_direction);

            Vector3 _relative = transform.position + _skewedDirection - transform.position;
            Quaternion _relativeRotation = Quaternion.LookRotation(_relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, _relativeRotation, m_turnSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Makes the camera pivot follow the transform of the player.
        /// </summary>
        private void FollowPlayer()
        {
            m_cameraPivotTransform.position = transform.position;
        }
        
        /// <summary>
        /// Gets the movement vector based on horizontal and vertical input.
        /// </summary>
        /// <param name="_horizontal">The horizontal input value.</param>
        /// <param name="_vertical">The vertical input value.</param>
        /// <returns>A Vector3 representing the movement direction.</returns>
        private Vector3 GetMovementVector(float _horizontal, float _vertical) {
            return new Vector3(_horizontal, DEFAULT_Y_POSITION, _vertical);
        }

        /// <summary>
        /// Updates the animator parameter for the blend tree based on movement direction.
        /// </summary>
        /// <param name="_direction">The movement direction vector.</param>
        private void UpdateAnimatorParameters(Vector3 _direction){
            float _speedPercent = _direction.magnitude;
            m_playerAnimator.SetFloat("Speed", _speedPercent); // Assumes the blend tree uses a parameter named "Speed"
        }

        /// <summary>
        /// Called by the Unity animation system to handle IK (Inverse Kinematics) operations on the character.
        /// It sets the weights and transformations for the feet IK.
        /// </summary>
        /// <param name="_layerIndex">The index of the layer on which the IK is applied.</param>
        private void OnAnimatorIK(int _layerIndex)
        {
            SetFeetIKWeights();
            SetFeetIKTransforms();
        }

        /// <summary>
        /// Sets the IK weights for the character's feet based on the animator's parameters.
        /// </summary>
        private void SetFeetIKWeights()
        {
            SetIKWeight(AvatarIKGoal.LeftFoot, m_playerAnimator.GetFloat(IK_WEIGHT_LEFT));
            SetIKWeight(AvatarIKGoal.RightFoot, m_playerAnimator.GetFloat(IK_WEIGHT_RIGHT));
        }

        /// <summary>
        /// Sets the IK position and rotation weights for a specified foot.
        /// </summary>
        /// <param name="_goal">The foot (left or right) for which the IK weights are being set.</param>
        /// <param name="_weight">The weight (influence) of the IK position and rotation.</param>
        private void SetIKWeight(AvatarIKGoal _goal, float _weight)
        {
            m_playerAnimator.SetIKPositionWeight(_goal, _weight);
            m_playerAnimator.SetIKRotationWeight(_goal, _weight);
        }

        /// <summary>
        /// Sets the IK transformations (position and rotation) for the character's feet
        /// to align them with the ground surface.
        /// </summary>
        private void SetFeetIKTransforms()
        {
            SetIKTransform(AvatarIKGoal.LeftFoot);
            SetIKTransform(AvatarIKGoal.RightFoot);
        }

        /// <summary>
        /// Calculates and sets the IK position and rotation for a specified foot
        /// to ensure it properly aligns with the ground surface.
        /// </summary>
        /// <param name="_goal">The foot (left or right) for which the IK transformation is being set.</param>
        private void SetIKTransform(AvatarIKGoal _goal)
        {
            Vector3 _footPosition = m_playerAnimator.GetIKPosition(_goal);
            Ray _ray = new Ray(_footPosition + Vector3.up, Vector3.down);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, m_distanceToGround + IK_DISTANCE, m_worldLayer))
            {
                Vector3 _targetPosition = _hit.point + Vector3.up * m_distanceToGround;
                Quaternion _targetRotation = Quaternion.LookRotation(transform.forward, _hit.normal);

                m_playerAnimator.SetIKPosition(_goal, _targetPosition);
                m_playerAnimator.SetIKRotation(_goal, _targetRotation);
            }
        }

    }
}