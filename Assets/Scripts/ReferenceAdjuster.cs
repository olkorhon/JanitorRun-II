
using UnityEngine;

/// <summary>
/// Class that provides a tool for scaling and moving the reference spots on the scene
/// </summary>

/*
    Class that provides a tool for scaling and moving the reference spots on the scene.
    Needs 2 reference points to manipulate.

    @Author: Olli Korhonen
*/

public class ReferenceAdjuster : MonoBehaviour
{
    public Transform bottom_left;
    public Transform top_right;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float hor = Input.GetAxis("AdjustHorizontal");
        float ver = Input.GetAxis("AdjustVertical");
        Vector2 amount = new Vector2(hor, ver);

        float scale = Input.GetAxis("AdjustScale");

        if (!Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (amount.magnitude > 0.5)
            {
                amount.Normalize();
                moveReference(amount / 10f);
            }

            if (scale > 0.5)
            {
                scaleUp(0.01f);
            }
            else if (scale < -0.5)
            {
                scaleDown(0.01f);
            }
        }
        else
        {
            if (amount.magnitude > 0.5)
            {
                amount.Normalize();
                moveReference(amount / 10f);
            }

            if (scale > 0.5)
            {
                scaleUp(0.1f);
            }
            else if (scale < -0.5)
            {
                scaleDown(0.1f);
            }
        }
	}

    private void scaleUp(float percentage)
    {
        Vector3 middle = (bottom_left.position + top_right.position) / 2.0f;
        Vector3 size = top_right.position - bottom_left.position;

        size *= 1.0f + percentage;

        bottom_left.position = middle - size / 2;
        top_right.position = middle + size / 2;
    }

    private void scaleDown(float percentage)
    {
        Vector3 middle = (bottom_left.position + top_right.position) / 2.0f;
        Vector3 size = top_right.position - bottom_left.position;

        size *= 1.0f - percentage;

        bottom_left.position = middle - size / 2;
        top_right.position = middle + size / 2;
    }

    private void moveReference(Vector2 amount)
    {
        // Append the amount to the relevant axes, in this case x and z
        this.bottom_left.transform.position = new Vector3(
            this.bottom_left.position.x + amount.x,
            this.bottom_left.position.y,
            this.bottom_left.position.z + amount.y);
        this.top_right.transform.position = new Vector3(
           this.top_right.position.x + amount.x,
           this.top_right.position.y,
           this.top_right.position.z + amount.y);
    }
}
