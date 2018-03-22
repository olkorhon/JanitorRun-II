using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffTracker : MonoBehaviour
{
    public LineRenderer actualSpeedLine;
    public LineRenderer desiredSpeedLine;
    public LineRenderer differenceLine;

    [Space(10)]
    public int framesBuffered = 500;
    public float heightScale = 0.01f;

    // Determined from first LineRenderer transform
    float widthFactor = 100;
    float heightFactor = 100; 

    // Use this for initialization
    void Start()
    {
        // Sanity checks
        if (actualSpeedLine.transform.GetType() != typeof(RectTransform))
        {
            Debug.LogWarning("actualSpeedLine transform not a rectTransform, Start aborted");
            return;
        }

        // Factor values
        RectTransform rt = (RectTransform)actualSpeedLine.transform;
        widthFactor  = rt.rect.width;
        heightFactor = rt.rect.height;

        // Init lines
        actualSpeedLine.positionCount  = framesBuffered;
        desiredSpeedLine.positionCount = framesBuffered;
        differenceLine.positionCount   = framesBuffered;
    }

    public void advance(float actualSpeed, float targetSpeed, float diff)
    {
        scrollLine(actualSpeedLine);
        scrollLine(desiredSpeedLine);
        scrollLine(differenceLine);

        actualSpeedLine .SetPosition(framesBuffered - 1, new Vector3(widthFactor, actualSpeed * heightFactor * heightScale, 0.0f));
        desiredSpeedLine.SetPosition(framesBuffered - 1, new Vector3(widthFactor, targetSpeed * heightFactor * heightScale, 0.0f));
        differenceLine  .SetPosition(framesBuffered - 1, new Vector3(widthFactor, diff        * heightFactor * heightScale, 0.0f));
    }

    private void scrollLine(LineRenderer lr)
    {
        for (int i = 1; i < framesBuffered; i++)
        {
            Vector3 holder = lr.GetPosition(i);
            holder.x = (i - 1.0f) * widthFactor / framesBuffered;
            lr.SetPosition(i - 1, holder);
        }
    }
}