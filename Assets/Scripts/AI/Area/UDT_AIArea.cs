using UnityEngine;

namespace UDT.Gameplay.AI {
    using static Settings.UDT_Constants;

    public class UDT_AIArea : MonoBehaviour
    {
        [Header("Area Settings")]
        [SerializeField] private Vector3 m_size = Vector3.one;

        [Header("Visual Settings")]
        [SerializeField] private Color m_areaColor = Color.white;
        [SerializeField] [Range(0, 1)] private float m_alpha = 0.5f;

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

        private void GenerateRandomPosition()
        {
            GeneratedPosition = transform.position + new Vector3(
                Random.Range(-m_size.x / LIMIT_DIVIDER_2D, m_size.x / LIMIT_DIVIDER_2D),
                GENERATED_Y_AREA_POSITION,
                Random.Range(-m_size.z / LIMIT_DIVIDER_2D, m_size.z / LIMIT_DIVIDER_2D)
            );
        }

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
    }
}