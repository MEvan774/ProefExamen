using Runtime.LevelSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GpsManager))]
public class GpsManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        GpsManager gpsManager = target as GpsManager;
        DrawDefaultInspector();

        if (GUILayout.Button("Add Checkpoint"))
        {
            if (!Application.isPlaying) return;
            if (gpsManager != null) gpsManager.AddCheckPoints();
        }
        if (GUILayout.Button("Remove Checkpoint"))
        {
            if (!Application.isPlaying) return;
            if (gpsManager != null) gpsManager.RemoveCheckPoints();
        }
        if (GUILayout.Button("Save Save"))
        {
            if (!Application.isPlaying) return;
            if (gpsManager != null) gpsManager.SaveCheckPoints();
        }
        if (GUILayout.Button("Load Save"))
        {
            if (!Application.isPlaying) return;
            if (gpsManager != null) gpsManager.LoadCheckPoints();
        }
        if (GUILayout.Button("Delete Save"))
        {
            if (!Application.isPlaying) return;
            if (gpsManager != null) gpsManager.DeleteCheckpoints();
        }
    }
}
