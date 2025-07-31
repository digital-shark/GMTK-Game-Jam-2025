using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    float maxExpectedDist = 100;

    [HideInInspector]
    public float testScore;

    [Range(0.01f, 1f)]
    public float maxAcceptableDistance = 0.2f; // Tune this

    public List<Vector2> solutionPoints;

    public float CalculateScore()
    {
        Line[] lines = FindObjectsByType<Line>(FindObjectsSortMode.None);
        if (lines.Length <= 0)
        {
            Debug.Log("No Lines found to calculate score!");
        }

        List<Vector3> allLines = new List<Vector3>();
        foreach (var line in lines)
        {
            if (line.points.Count > 0)
            {
                allLines.AddRange(line.points);
            }
        }
        //testScore = ComparePaths(allLines, solutionPoints);
        testScore = Compare(ConvertV3toV2(allLines), solutionPoints);

        return testScore;
    }

    float ComparePaths(List<Vector3> userPoints, List<Vector2> solutionPoints)
    {
        float sum = 0;
        for (int i = 0; i < userPoints.Count; i++)
        {
            sum += Vector2.Distance(userPoints[i], solutionPoints[i]);
        }
        float avgDist = sum / userPoints.Count;
        return Mathf.Clamp01(1.0f - avgDist / maxExpectedDist) * 100; // Convert to percent
    }

    List<Vector2> ConvertV3toV2(List<Vector3> v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3.ToArray(), getV3fromV2).ToList<Vector2>();
    }

    public static Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public float Compare(List<Vector2> userPath, List<Vector2> refPath, int numPoints = 64)
    {
        var normUser = NormalizePath(userPath, numPoints);
        var normRef = NormalizePath(refPath, numPoints);

        float totalDist = 0f;

        for (int i = 0; i < numPoints; i++)
        {
            totalDist += Vector2.Distance(normUser[i], normRef[i]);
        }

        float avgDist = totalDist / numPoints;

        // Convert to similarity percent (100 = perfect match, 0 = far off)
        float similarity = Mathf.Clamp01(1f - (avgDist / maxAcceptableDistance));
        return similarity * 100f;
    }

    List<Vector2> NormalizePath(List<Vector2> path, int numPoints)
    {
        var resampled = Resample(path, numPoints);
        var centered = Center(resampled);
        var scaled = ScaleToUnit(centered);
        return scaled;
    }

    List<Vector2> Resample(List<Vector2> path, int numPoints)
    {
        float totalLength = 0f;
        for (int i = 1; i < path.Count; i++)
            totalLength += Vector2.Distance(path[i - 1], path[i]);

        float step = totalLength / (numPoints - 1);
        List<Vector2> result = new List<Vector2> { path[0] };
        float distSoFar = 0f;

        for (int i = 1; i < path.Count && result.Count < numPoints; i++)
        {
            Vector2 curr = path[i];
            Vector2 prev = path[i - 1];
            float segmentLength = Vector2.Distance(prev, curr);

            while (distSoFar + segmentLength >= step && result.Count < numPoints)
            {
                float t = (step - distSoFar) / segmentLength;
                Vector2 newPoint = Vector2.Lerp(prev, curr, t);
                result.Add(newPoint);
                segmentLength -= (step - distSoFar);
                prev = newPoint;
                distSoFar = 0f;
            }

            distSoFar += segmentLength;
        }

        while (result.Count < numPoints)
            result.Add(path[path.Count - 1]);

        return result;
    }

    List<Vector2> Center(List<Vector2> path)
    {
        Vector2 centroid = Vector2.zero;
        foreach (var p in path) centroid += p;
        centroid /= path.Count;

        for (int i = 0; i < path.Count; i++)
            path[i] -= centroid;

        return path;
    }

    List<Vector2> ScaleToUnit(List<Vector2> path)
    {
        float maxX = float.MinValue, maxY = float.MinValue;
        foreach (var p in path)
        {
            maxX = Mathf.Max(maxX, Mathf.Abs(p.x));
            maxY = Mathf.Max(maxY, Mathf.Abs(p.y));
        }

        float scale = Mathf.Max(maxX, maxY);
        if (scale == 0f) return path;

        for (int i = 0; i < path.Count; i++)
            path[i] /= scale;

        return path;
    }
}
