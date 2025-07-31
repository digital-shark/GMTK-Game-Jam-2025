using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScoreManager))]
public class ScoreManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScoreManager manager = (ScoreManager)target;

        if (GUILayout.Button("Calculate Score"))
        {
            manager.CalculateScore();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Last Score:", manager.testScore.ToString("F1") + " %");
    }
}
