using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InAppReview))] // Replace with your target type
public class InAppReviewEditor : Editor
{

    public override void OnInspectorGUI()
    {
        // Show a message box at the top of the Inspector
        EditorGUILayout.HelpBox("By Default,All the Settings are Already Perfect, Only Change any setting if you need anything else", MessageType.Warning);
        // Draw default inspector below the message
        DrawDefaultInspector();

    }
}
