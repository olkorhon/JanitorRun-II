using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LikertController : MonoBehaviour
{
    //private static int NOT_AT_ALL = 0;
    //private static int SLIGHTLY = 1;
    //private static int MODERATELY = 2;
    //private static int FAIRLY = 3;
    //private static int EXTREMELY = 4;

    LikertKnob[] likert_knobs;
    Image[] selection_knobs;
    Text question_text;

    string question;

    int current_choice;

	// Use this for initialization
	void Start ()
    {
        current_choice = -1;

        likert_knobs = GetComponentsInChildren<LikertKnob>();
        selection_knobs = new Image[likert_knobs.Length];
        for (int i = 0; i < likert_knobs.Length; i++)
        {
            Image[] images = likert_knobs[i].gameObject.GetComponentsInChildren<Image>();
            selection_knobs[i] = images[1];
            selection_knobs[i].gameObject.SetActive(false);
        }

        question_text = GetComponent<Text>();
        if (question_text == null)
            Debug.LogWarning("LikertController: Could not find text component in the host object, prefab is very likely broken.");
        else if (question != null)
            question_text.text = question;

	}

    public void loadQuestion(string question)
    {
        // If possible load directly to text field, store to question variable if no text is available
        if (question_text != null)
            this.question_text.text = question;
        else
            this.question = question;
    }

    public void changeSelection(int target)
    {
        
        for (int i = 0; i < selection_knobs.Length; i++)
        {
            if (target == i)
            {
                this.current_choice = target;
                selection_knobs[i].gameObject.SetActive(true);
            }
            else
                selection_knobs[i].gameObject.SetActive(false);
        }
    }

    public string getQuestion()
    {
        return question_text.text;
    }
    public int getAnswer()
    {
        return current_choice;
    }
}
