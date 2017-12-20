using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class RacingLine : MonoBehaviour
{
    public RacingLineConfig config;
    public LineRenderer lineRenderer;
    public Material material;

    private int pointCount = 0;
    private int numCurves = 0;
    private List<Vector3> pathPoints = new List<Vector3>();

    private void Start()
    {
        if (!lineRenderer)
            lineRenderer = (LineRenderer)GetComponent<LineRenderer>();

        lineRenderer.material = material;
    }

    private void Update()
    {
        /*
        UpdatePath();

        for (int i = 1; i < pointCount; i++)
        {
            Vector3 start = pathPoints[i - 1];
            Vector3 end = pathPoints[i];
            DrawLine(start, end, .5f, Color.white);
        }
        */

        numCurves = config.controlPoints.Count / 3;

        if (numCurves > 0 && config.controlPoints.Count % 3 == 0)
            numCurves--;
        
        pointCount = 10;

        CalculatePath();

        DrawCurve();
    }

    void CalculatePath()
    {
        pathPoints.Clear();

        for (int c = 0; c < numCurves; c++)
        {
            int nodeIndex = c * 3;

            Vector3 p0 = config.controlPoints[nodeIndex];
            Vector3 p1 = config.controlPoints[nodeIndex + 1];
            Vector3 p2 = config.controlPoints[nodeIndex + 2];
            Vector3 p3 = config.controlPoints[nodeIndex + 3];

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
            //Vector3 start = pathPoints[i - 1];
            Vector3 point = pathPoints[i];
            lineRenderer.positionCount = pathPoints.Count;
            lineRenderer.SetPosition(i, point);
            //DrawLine(start, end, .5f, Color.white);
        }

        /*
        for (int c = 0; c < numCurves; c++)
        {
            int nodeIndex = c * 3;

            Vector3 p0 = config.controlPoints[nodeIndex];
            Vector3 p1 = config.controlPoints[nodeIndex + 1];
            Vector3 p2 = config.controlPoints[nodeIndex + 2];
            Vector3 p3 = config.controlPoints[nodeIndex + 3];

            for (int p = 1; p <= pointCount; p++)
            {
                float t = p / (float)pointCount;

                Vector3 point = CalculateBezier(p0, p1, p2, p3, t);
                lineRenderer.positionCount = (c * pointCount) + p;
                lineRenderer.SetPosition((c * pointCount) + (p - 1), point);
            }
        }
        */
    }

    /*
    void CreateCurve()
    {
        segments = config.controlPoints.Count / 3;

        for (int s = 0; s < config.controlPoints.Count - 3; s += 3)
        {
            Vector3 p0 = config.controlPoints[s];
            Vector3 p1 = config.controlPoints[s + 1];
            Vector3 p2 = config.controlPoints[s + 2];
            Vector3 p3 = config.controlPoints[s + 3];

            if (s == 0)
            {
                pathPoints.Add(CalculateBezier(p0, p1, p2, p3, 0.0f));
            }

            for (int p = 0; p < (pointCount / segments); p++)
            {
                float t = (1.0f / (pointCount / segments)) * p;
                Vector3 point = CalculateBezier(p0, p1, p2, p3, t);
                pathPoints.Add(point);
            }
        }
    }
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

    void DrawLine(Vector3 start, Vector3 end, float lineSize, Color color)
    {
        //lineRenderer.material = material;
        //lineRenderer.material.color = color;
        //lineRenderer.startWidth = lineSize;
        //lineRenderer.endWidth = lineSize;
        //lineRenderer.positionCount = 2;
        //lineRenderer.SetPosition(0, start);
        //lineRenderer.SetPosition(1, end);
    }

    void OnDrawGizmos()
    {
        for (int cp = 0; cp < config.controlPoints.Count; cp++)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(config.controlPoints[cp], Vector3.up, .08f);
        }

        CalculatePath();
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 point = pathPoints[i];

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(point, .1f);
        }

        /*
        UpdatePath();
        for (int i = 1; i < (path.pointCount); i++)
        {
            Vector3 startv = path.pathPoints[i - 1];
            Vector3 endv = path.pathPoints[i];
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startv, endv);
        }
        */
    }
}
