using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour {
    [Range(0, 10)] public float viewRadius;
    [Range(0,360)] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<GameObject> visibleTargets = new();

    public bool foundEnemies;

    void Start() {
        StartCoroutine(nameof(FindTargetsWithDelay), .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets() {
        visibleTargets.Clear();
        foundEnemies = false;
        
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInRadius.Length; i++) 
        {
            GameObject target = targetsInRadius[i].transform.gameObject;
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask)) {
                    // See the target
                    foundEnemies = true;
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) 
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees 
                                * Mathf.Deg2Rad));
    }
}
