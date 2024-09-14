using UnityEngine;
using UnityEditor;

namespace UDT.Gameplay.AI.Editor
{
    public static class UDT_AreaMenu
    {
        [MenuItem("GameObject/AI Area/Create NPC Area", false, 0)]
        public static void CreateNPCSpawningArea()
        {
            CreateSpawningArea("NPC", Color.yellow);
        }

        [MenuItem("GameObject/AI Area/Create Enemy Area", false, 0)]
        public static void CreateEnemySpawningArea()
        {
            CreateSpawningArea("Enemy", Color.red);
        }

        private static void CreateSpawningArea(string type, Color areaColor)
        {
            GameObject go = new GameObject($"New {type} AI Area");
            UDT_AIArea area = go.AddComponent<UDT_AIArea>();
            area.AreaColor = areaColor;

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, $"Create {type} AI Area");
        }
    }
}