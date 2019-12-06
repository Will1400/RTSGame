﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoIcon : MonoBehaviour
{
    [SerializeField]
    private string IconName;
    [SerializeField]
    private Vector3 offset;

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position + offset, IconName, true);
    }
}
