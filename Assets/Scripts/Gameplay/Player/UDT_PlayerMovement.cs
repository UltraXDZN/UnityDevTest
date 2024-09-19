using UnityEngine;

namespace UDT.Gameplay.Player
{
    using static Settings.UDT_Constants;

    public class UDT_PlayerMovement
    {
        private Rigidbody m_playerRigidbody;
        private Animator m_playerAnimator;
        private float m_maxSpeed = MAX_SPEED;
        private float m_currentSpeed = DEFAULT_SPEED;
        private float m_turnSpeed = DEFAULT_TURN_SPEED;
        private float m_horizontal;
        private float m_vertical;
        private Vector3 m_direction;

        private UDT_PlayerAnimation m_playerAnimation;

        public float CurrentSpeed { get => m_currentSpeed; set => m_currentSpeed = value; }

        public UDT_PlayerMovement(Rigidbody rigidbody, Animator animator, UDT_PlayerAnimation playerAnimation)
        {
            m_playerRigidbody = rigidbody;
            m_playerAnimator = animator;
            m_playerAnimation = playerAnimation;
        }
        /// <summary>
        /// Handles the player movement
        /// </summary>
        public void HandleMovement()
        {
            AssignAxisValues();
            SetDirection();
            DirectionToLookAt(m_direction);
            AccelerateSpeed(m_maxSpeed);
            m_playerAnimation.UpdateMovementAnimation(m_direction);
        }

        /// <summary>
        /// Applies the movement to the player's transform
        /// </summary>
        public void ApplyMovement()
        {
            TranslatePlayer(m_direction);
        }

        /// <summary>
        /// Reads the axis input values and assigns them to the player's movement values
        /// </summary>
        private void AssignAxisValues()
        {
            m_horizontal = Input.GetAxis(HORIZONTAL_AXIS);
            m_vertical = Input.GetAxis(VERTICAL_AXIS);
        }

        /// <summary>
        /// Sets the direction of the player's movement
        /// </summary>
        private void SetDirection()
        {
            m_direction = GetMovementVector(m_horizontal, m_vertical);
        }

        /// <summary>
        /// Accelerates the player's speed based on the current speed and the max speed
        /// </summary>
        /// <param name="maxSpeed"></param>
        private void AccelerateSpeed(float maxSpeed)
        {
            CurrentSpeed = m_direction.magnitude > DEFAULT_SPEED ? 
                Mathf.Lerp(CurrentSpeed, maxSpeed, ACCELERATION * Time.deltaTime) : 
                CurrentSpeed;
        }

        /// <summary>
        /// Translates the player's position based on the direction of the player and the current speed
        /// </summary>
        /// <param name="direction"></param>
        private void TranslatePlayer(Vector3 direction)
        {
            m_playerRigidbody.MovePosition(m_playerRigidbody.position + m_playerRigidbody.transform.forward * direction.magnitude * CurrentSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the player to look at the desired direction
        /// </summary>
        /// <param name="direction"></param>
        private void DirectionToLookAt(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                TurnToDirection(direction);
            }
        }

        /// <summary>
        /// Rotates the player to look at the normalized direction (pressing W key makes player go up in the Z axis)
        /// </summary>
        /// <param name="direction"></param>
        private void TurnToDirection(Vector3 direction)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(Vector3.zero.x, Y_ROTATION_SKEW_ANGLE, Vector3.zero.z));
            Vector3 skewedDirection = rotationMatrix.MultiplyPoint3x4(direction);

            Vector3 relative = m_playerRigidbody.position + skewedDirection - m_playerRigidbody.position;
            Quaternion relativeRotation = Quaternion.LookRotation(relative, Vector3.up);

            m_playerRigidbody.rotation = Quaternion.RotateTowards(m_playerRigidbody.rotation, relativeRotation, m_turnSpeed * Time.deltaTime);
        }


        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        /// <returns>Returns the movement vector based on the horizontal and vertical axis (input values)</returns>
        private Vector3 GetMovementVector(float horizontal, float vertical)
        {
            return new Vector3(horizontal, Vector3.zero.y, vertical);
        }

        /// <summary>
        /// Calculates the max speed based on the player's crouching state
        /// </summary>
        public void CalculateMaxSpeed()
        {
            m_maxSpeed = m_playerAnimator.GetBool("IsCrouching") ? MAX_CROUCHING_SPEED : MAX_SPEED;
        }

    }
}