using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private bool debugDrawing = false;
    [SerializeField]
    private float debugDrawingDistance = 10.0f;

    Line activeLine;
    List<Line> lines;

    void Update()
    {
        if (!debugDrawing)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            GameObject newLine = Instantiate(linePrefab);
            activeLine = newLine.GetComponent<Line>();

            if (lines == null)
                lines = new List<Line>();
            lines.Add(activeLine);
        }

        if (Input.GetMouseButtonUp(0))
        {
            activeLine = null;
        }

        if (activeLine != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, debugDrawingDistance));
            activeLine.UpdateLine(mousePos);
        }
    }
}
