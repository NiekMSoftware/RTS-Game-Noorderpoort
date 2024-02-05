using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(DetectEnemies))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI() {
        DetectEnemies fow = (DetectEnemies)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up,
                        Vector3.forward, 360, fow.viewRadius, 10f);

        Handles.color = Color.blue;
        Handles.DrawWireArc(fow.transform.position, Vector3.up,
            Vector3.forward, 360, fow.senseDistance, 10f);

        Vector3 viewAngle = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        
        Handles.color = Color.white;
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngle * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (GameObject visibleTarget in fow.visibleTargets)
        {
            if (visibleTarget == null) continue;

            Handles.DrawLine(fow.transform.position, visibleTarget.transform.position);
        }
    }
}
#endif
