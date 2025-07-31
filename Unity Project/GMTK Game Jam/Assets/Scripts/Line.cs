using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    List<Vector3> points;

    [SerializeField]
    private float minDistance = 0.1f;

    public void UpdateLine(Vector3 position)
    {
        // Init if null
        if (points == null)
        {
            points = new List<Vector3>();
            SetPoint(position);
            return;
        }

        if (Vector3.Distance(points.Last(),position) > minDistance)
        {
            SetPoint(position);
        }
    }

    void SetPoint(Vector3 point)
    {
        points.Add(point);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
    }
}
