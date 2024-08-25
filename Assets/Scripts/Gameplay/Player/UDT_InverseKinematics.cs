using UnityEngine;

namespace UDT.Gameplay.Player
{
    using static Settings.UDT_Constants;

    public class UDT_InverseKinematics
    {
        private Animator m_playerAnimator;
        private LayerMask m_worldLayer;
        private float m_distanceToGround = 0.1f;
        private Vector3 wallNormal;
        public Transform rightShoulderSpawn;
        public Transform leftShoulderSpawn;
        public float distanceWall = 0.1f;
        public float handsOffset = 0.5f;

        public UDT_InverseKinematics(Animator animator, LayerMask worldLayer, Transform rightShoulder, Transform leftShoulder)
        {
            m_playerAnimator = animator;
            m_worldLayer = worldLayer;
            rightShoulderSpawn = rightShoulder;
            leftShoulderSpawn = leftShoulder;
        }
        
        /// <summary>
        /// Applies the Inverse Kinematics to the player
        /// </summary>
        /// <param name="layerIndex"></param>
        public void ApplyIK(int layerIndex)
        {
            SetLimbsIKWeights();
            SetLimbsIKTransforms();
        }

        /// <summary>
        /// Sets the IK weight for all the limbs of the player
        /// </summary>
        private void SetLimbsIKWeights()
        {
            SetIKWeight(AvatarIKGoal.LeftFoot, m_playerAnimator.GetFloat(IK_WEIGHT_LEFT));
            SetIKWeight(AvatarIKGoal.RightFoot, m_playerAnimator.GetFloat(IK_WEIGHT_RIGHT));
            SetIKWeight(AvatarIKGoal.LeftHand, m_playerAnimator.GetFloat(IK_WEIGHT_LEFT_HAND));
            SetIKWeight(AvatarIKGoal.RightHand, m_playerAnimator.GetFloat(IK_WEIGHT_RIGHT_HAND));
        }

        /// <summary>
        /// Sets the IK weight for the desired limb of the player
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="weight"></param>
        private void SetIKWeight(AvatarIKGoal goal, float weight)
        {
            m_playerAnimator.SetIKPositionWeight(goal, weight);
            m_playerAnimator.SetIKRotationWeight(goal, weight);
        }

        /// <summary>
        /// Sets the IK position and rotation for all the limbs of the player
        /// </summary>
        private void SetLimbsIKTransforms()
        {
            SetIKTransform(AvatarIKGoal.LeftFoot);
            SetIKTransform(AvatarIKGoal.RightFoot);
            SetIKTransform(AvatarIKGoal.LeftHand);
            SetIKTransform(AvatarIKGoal.RightHand);
        }

        /// <summary>
        /// Sets the IK position and rotation for the desired limb of the player
        /// </summary>
        /// <param name="goal"></param>
        private void SetIKTransform(AvatarIKGoal goal)
        {
            Vector3 limbPosition = m_playerAnimator.GetIKPosition(goal);
            Ray ray = new Ray(limbPosition + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, m_distanceToGround + IK_DISTANCE, m_worldLayer))
            {
                Vector3 targetPosition = hit.point + Vector3.up * m_distanceToGround;
                Quaternion targetRotation = Quaternion.LookRotation(m_playerAnimator.transform.forward, hit.normal);

                m_playerAnimator.SetIKPosition(goal, targetPosition);
                m_playerAnimator.SetIKRotation(goal, targetRotation);
            }

            if (goal == AvatarIKGoal.LeftHand || goal == AvatarIKGoal.RightHand)
            {
                HandleHandIK(goal, hit);
            }
        }

        /// <summary>
        /// Handles the IK for the hands of the player
        /// </summary>
        /// <param name="goal">Position of the desired hand in the armature</param>
        /// <param name="hit"></param>
        private void HandleHandIK(AvatarIKGoal goal, RaycastHit hit)
        {
            var d = Vector3.Dot(wallNormal, m_playerAnimator.transform.right);
            if (d < -0.55f)
            {
                PlaceRightHand();
            }
            else if (d > 0.55f)
            {
                PlaceLeftHand();
            }
            else
            {
                PlaceRightHand(handsOffset * (-0.5f * hit.distance));
                PlaceLeftHand(handsOffset * (-0.5f * hit.distance));
            }
        }

        /// <summary>
        /// Places the right hand of the player on the wall
        /// </summary>
        /// <param name="offset">Distance from the center of the body</param>
        private void PlaceRightHand(float offset = 0.0f)
        {
            RaycastHit hit;
            var ray = new Ray(rightShoulderSpawn.position, -wallNormal);
            if (Physics.Raycast(ray, out hit, 1.0f, m_worldLayer))
            {
                var v = Vector3.zero;
                if (offset != 0.0f)
                {
                    v = Quaternion.Euler(0, 90, 0) * wallNormal;
                }

                Vector3 targetPosition = hit.point + hit.normal * distanceWall + v * offset;
                m_playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, targetPosition);
            }
        }

        /// <summary>
        /// Places the left hand of the player on the wall
        /// </summary>
        /// <param name="offset">Distance from the center of the body</param>
        private void PlaceLeftHand(float offset = 0.0f)
        {
            RaycastHit hit;
            var ray = new Ray(leftShoulderSpawn.position, -wallNormal);
            if (Physics.Raycast(ray, out hit, 1.0f, m_worldLayer))
            {
                var v = Vector3.zero;
                if (offset != 0.0f)
                {
                    v = Quaternion.Euler(0, -90, 0) * wallNormal;
                }

                Vector3 targetPosition = hit.point + hit.normal * distanceWall + v * offset;
                m_playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, targetPosition);
            }
        }

        /// <summary>
        /// Sets the normal of the wall that the player is colliding with
        /// </summary>
        /// <param name="normal"></param>
        public void SetWallNormal(Vector3 normal)
        {
            wallNormal = normal;
        }
    }
}