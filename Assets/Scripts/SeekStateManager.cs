using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekStateManager : MonoBehaviour
{
    public float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public GameObject hideCharacter;
    private void Update()
    {
        if(FieldOfViewCheck())
        {
            if(hideCharacter.GetComponent<HideStateManager>() != null) {
                hideCharacter.GetComponent<HideStateManager>().Imprison();
            }
        }
    }

    public bool FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                   hideCharacter = target.gameObject;
                   return true;
                }
            }
        }
        return false;
    }
}