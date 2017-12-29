using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(LineRenderer))]
public class RacingLine : MonoBehaviour
{
    public RacingLineConfig config; // ScriptableObject asset containing the control points.
    public LineRenderer lineRenderer; // Line renderer component reference.
    public Material material; // Material for the line.
    public int pointCount = 10; // Number of curve points per segment to be calculated.

    // Display helpers:
    public bool displayGuides = true; // Display handles in editor.
    public bool displayLabels = true; // Display control point index labels with handles.

    private int numCurves = 0; // Number of segments.
    private List<Vector3> pathPoints = new List<Vector3>(); // Calculated path points.

    private void Start()
    {
        // Get line renderer reference.
        if (!lineRenderer)
            lineRenderer = (LineRenderer)GetComponent<LineRenderer>();

        // Set the line material.
        lineRenderer.material = material;
    }

    private void Update()
    {
        // Calculate the number of segments for path calculation.
        InitPath();

        // Calculate the points.
        CalculatePath();

        // Draw the line segments.
        DrawCurve();
    }

    void InitPath()
    {
        if (config.controlPoints.Count == 0) return;

        // Get segment count.
        numCurves = (int)((config.controlPoints.Count - 1) / 3);
    }

    void CalculatePath()
    {
        if (pointCount <= 0) return;

        // Clear old path.
        pathPoints.Clear();

        // Calculate each segment:
        for (int c = 0; c < numCurves; c++)
        {
            int nodeIndex = c * 3;

            Vector3 p0 = config.controlPoints[nodeIndex];
            Vector3 p1 = config.controlPoints[nodeIndex + 1];
            Vector3 p2 = config.controlPoints[nodeIndex + 2];
            Vector3 p3 = config.controlPoints[nodeIndex + 3];

            // Calculate each point in the segment:
            for (int p = 0; p <= pointCount; p++)
            {
                float t = p / (float)pointCount;

                Vector3 point = CalculateBezier(p0, p1, p2, p3, t);
                pathPoints.Add(point);
            }
        }
    }

    void DrawCurve()
    {
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 point = pathPoints[i];
            lineRenderer.positionCount = pathPoints.Count;
            lineRenderer.SetPosition(i, point);
        }
    }

    /**
     * Calculates a point in cubic bezier curve at t.
     */
    Vector3 CalculateBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float tt = t * t;
        float ttt = tt * t;
        float u = 1.0f - t;
        float uu = u * u;
        float uuu = uu * u;

        Vector3 p = uuu * p0;
        p += 3.0f * uu * t * p1;
        p += 3.0f * u * tt * p2;
        p += ttt * p3;

        return p;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!displayGuides) return;

        for (int cp = 0; cp < config.controlPoints.Count; cp++)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(config.controlPoints[cp], Vector3.up, .08f);
            if (displayLabels)
                Handles.Label(config.controlPoints[cp], cp.ToString());
        }

        InitPath();

        CalculatePath();

        for (int i = 1; i < pathPoints.Count; i++)
        {
            Vector3 first = pathPoints[i - 1];
            Vector3 last = pathPoints[i];

            Handles.color = Color.yellow;
            Handles.DrawLine(first, last);
        }
    }
#endif
}
