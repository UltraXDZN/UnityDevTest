using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

namespace UDT.Gameplay{
    using static Settings.UDT_Constants;

    [RequireComponent(typeof(Rigidbody))]
    public class UDT_Player : MonoBehaviour
    {
        [SerializeField] private float m_speed = DEFAULT_SPEED;
        [SerializeField] private Rigidbody m_playerRigidbody;
        [SerializeField] private Transform m_cameraPivotTransform;

        private float m_turnSpeed = DEFAULT_TURN_SPEED;

        private float m_horizontal;
        private float m_vertical;

        private Vector3 m_direction;

        private void Start(){
            m_playerRigidbody = GetComponent<Rigidbody>();
        }

        private void Update(){

            AssignAxisValues();
            m_direction = GetMovementVector(m_horizontal, m_vertical);

            DirectionToLookAt(m_direction);
            FollowPlayer();
        }

        private void FixedUpdate(){
            TranslatePlayer(m_direction);   
        }

        /// <summary>
        /// Assigns axis values from input.
        /// </summary>
        private void AssignAxisValues(){
            m_horizontal = Input.GetAxis(HORIZONTAL_AXIS);
            m_vertical = Input.GetAxis(VERTICAL_AXIS);
        }

        /// <summary>
        /// Translates the player in the given direction, provided by the axises' keys.
        /// </summary>
        /// <param name="_direction">The direction to move the player.</param>
        private void TranslatePlayer (Vector3 _direction){
            m_playerRigidbody.MovePosition(transform.position + transform.forward * _direction.magnitude * m_speed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates the player to look at the given direction, provided by the axises' keys.
        /// </summary>
        /// <param name="_direction">The direction to look at.</param>

        private void DirectionToLookAt(Vector3 _direction){
            if (_direction == Vector3.zero) return;

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

    }
}