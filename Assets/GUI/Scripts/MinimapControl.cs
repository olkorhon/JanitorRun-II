using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that contains code for handling minimap.
/// </summary>

/*
    Class that manages a RawImage minimap. 
    Script needs two reference points in the scene that correspond to
    the lower left and upper right corners of the minimap image.
   
    TODO:
    Zooming controls

    @Author: Olli Korhonen
*/

public class MinimapControl : MonoBehaviour
{
    private Transform player;
    public Transform bottom_left_reference;
    public Transform top_right_reference;
    public Transform pointer;

    private float angle_offset = 90;
    private RawImage image;
    //private Vector2 size_cur;
    //private Vector2 size_max;

    private float normalizing_factor_horizontal;
    private float normalizing_factor_vertical;

    // Zoom controls
    //public float zoom_min;
    //public float zoom_max;
    //public float zoom_cur;

	// Use this for initialization
	public void startMinimap(Steering target)
    {  
        Debug.Log("Minimap start");
        // Find a player to track if one is not provided
        Steering p_control = target;
        if (p_control != null)
        {
            this.player = p_control.transform;
        }
        else
        {
            Debug.LogWarning("Minimap: Could not find local players ");
        }

        if (this.pointer == null)
        {
            Debug.LogWarning("Minimap: Could not find a reference to a pointer");
        }

        // Count correct scale and position from anchors in the scene
        this.normalizing_factor_horizontal = Mathf.Abs(bottom_left_reference.position.x - top_right_reference.position.x);
        this.normalizing_factor_vertical = Mathf.Abs(bottom_left_reference.position.z - top_right_reference.position.z);

        // Fetch own components
        this.image = GetComponent<RawImage>();

        // Zooming limit
        //if (this.image.texture.width < this.image.texture.height)
        //    this.size_max = new Vector2(1, (this.image.texture.height - this.body.rect.height) / this.image.texture.height);
        //else
        //    this.size_max = new Vector2(1, (this.image.texture.height - this.body.rect.height) / this.image.texture.height);

        // Set default zoom to 0
        changeZoom(0.0f);
    }

    // Update is called once per frame
    void Update ()
    {
        // Set the minimap to point at the correct position in the map
        if (this.player != null)
        {
            // Convert player position to a zero based space
            Vector3 converted_pos = this.player.position - bottom_left_reference.position;

            // Normalize position to be between 0 and 1 (Same as the one used in uv maps)
            Vector3 normalized_pos = new Vector2(
                converted_pos.x / normalizing_factor_horizontal,
                converted_pos.z / normalizing_factor_vertical);

            // Define a box around the normalized position and set it as the uvRect of the image
            this.image.uvRect = new Rect(normalized_pos.x - 0.25f, normalized_pos.y - 0.25f, 0.50f, 0.50f);

            // Rotate the pointer to the same angle as the character (Remember that we are doing a plane conversion from horizontal to vertical)
            if (this.pointer != null)
            {
                this.pointer.transform.rotation = Quaternion.Euler(0, 0, -this.player.rotation.eulerAngles.y + angle_offset);
            }
        }
	}

    public void changeZoom(float new_zoom)
    {
        //this.zoom_cur = this.zoom_min + (this.zoom_max - this.zoom_min) * new_zoom;
        //this.size_cur = this.size_max * this.zoom_cur;
    }
}
