using System;
using UnityEngine;

namespace UDT.Gameplay.AI {
    using static Settings.UDT_Constants;
    using Random = UnityEngine.Random;

    public class UDT_AIArea : MonoBehaviour
    {
        [Header("Area Settings")]
        [SerializeField] private Vector3 m_size = Vector3.one;

        [Header("Visual Settings")]
        [SerializeField] private Color m_areaColor = Color.white;
        [SerializeField] [Range(0, 1)] private float m_alpha = AREA_ALPHA_DEFAULT;

        private Vector3 m_generatedPosition;

        public Vector3 GeneratedPosition 
        { 
            get => m_generatedPosition; 
            private set => m_generatedPosition = value; 
        }
        public Color AreaColor { get => m_areaColor; set => m_areaColor = value; }

        private void Start()
        {
            InvokeRepeating(nameof(GenerateRandomPosition), POINT_RESPAWN_TIMER, POSITION_GENERATION_INTERVAL);
        }

        /// <summary>
        /// Generates a random position within the area.
        /// </summary>
        /// <returns>Generated position</returns>
        public Vector3 GenerateRandomPosition()
        {
            Vector3 localPosition = new Vector3(
                Random.Range(-m_size.x / LIMIT_DIVIDER_2D, m_size.x / LIMIT_DIVIDER_2D),
                GENERATED_Y_AREA_POSITION,
                Random.Range(-m_size.z / LIMIT_DIVIDER_2D, m_size.z / LIMIT_DIVIDER_2D)
            );

            GeneratedPosition = transform.TransformPoint(localPosition);
            return GeneratedPosition;
        }

#region Gizmos
        [ExecuteInEditMode]
        private void OnDrawGizmosSelected()
        {
            DrawAreaGizmo();
        }

        private void DrawAreaGizmo()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = new Color(AreaColor.r, AreaColor.g, AreaColor.b, m_alpha);
            Gizmos.DrawCube(Vector3.zero, m_size);
        }
#endregion
    }
}