using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationHelper : MonoBehaviour
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pointCount"></param>
    public static List<Vector3> GetFormation(Vector3 position, int pointCount)
    {
        List<Vector3> points = new List<Vector3>();

        int row = -1;
        for (int i = 0; i < pointCount; i++)
        {
            if (i % 5 == 0)
                row++;

            Vector3 offset = new Vector3(1.5f, 0) * (i % 5) + new Vector3(0, 0, row * 2);

            points.Add(position + offset);
        }

        return points;
    }
}
