using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMapper : MonoBehaviour
{
    public RectTransform marker;
    public float border = 10.0f;

    // Determined from transform rect
    float widthFactor = 100.0f;
    float heightFactor = 100.0f;

    // Initialization
    private void Start()
    {
        // Sanity checks
        if (transform.GetType() != typeof(RectTransform))
        {
            Debug.LogWarning("transform not a rectTransform, Start aborted");
            return;
        }

        // Factor values
        RectTransform rt = (RectTransform)transform;
        widthFactor = rt.rect.width - border * 2;
        heightFactor = rt.rect.height - border * 2;
    }

    // Update is called once per frame, refresh input from Input module
    void Update ()
    {
        marker.anchoredPosition = new Vector2(
            border + widthFactor  * (Input.GetAxis("Horizontal") + 1.0f) / 2.0f,
            border + heightFactor * (Input.GetAxis("Vertical")   + 1.0f) / 2.0f);
	}
}
