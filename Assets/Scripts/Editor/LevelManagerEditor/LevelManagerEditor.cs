using Runtime.LevelSystem;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            LevelManager levelManager = target as LevelManager;
            DrawDefaultInspector();

            if (GUILayout.Button("Next level"))
            {
                if (!Application.isPlaying) return;
                if (levelManager != null) levelManager.ForceNextLevel();
            }
        }
    }
}
