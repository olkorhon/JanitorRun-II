using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SurveyPageLoader : MonoBehaviour
{
    QuestionLoader question_loader;
    LikertController[] controllers;

    public int start_offset;
    public int questions;

    public GameObject likert_prefab;
    public float horizontal_offset = 120.0f;
    public float vertical_offset = 200.0f;
    public float vertical_separation = 40.0f;

	// Use this for initialization
	void Start ()
    {
        Debug.Log("SurveyPageLoader: Starting script");

        this.question_loader = GetComponentInParent<QuestionLoader>();
        if (question_loader == null)
            Debug.LogWarning("SurveyPageLoader: No QuestionLoader component found in parent, cannot prepare likert controllers.");

        setupControllers();
	}

    private void setupControllers()
    {
        controllers = new LikertController[questions];

        // Instanciate LikertQuestions
        GameObject instance_holder;
        for (int i = 0; i < questions; i++)
        {
            instance_holder = Instantiate(likert_prefab);
            instance_holder.transform.SetParent(this.transform);
            instance_holder.transform.localScale = new Vector3(1, 1, 1);

            RectTransform rect_transform = instance_holder.GetComponent<RectTransform>();
            rect_transform.anchoredPosition = new Vector3(horizontal_offset, vertical_offset - vertical_separation * i, 0);
            
            controllers[i] = instance_holder.GetComponent<LikertController>();
        }

        // Load questions into these questions
        if (controllers.Length > 0)
        {
            ArraySegment<string> questions = question_loader.getQuestions(start_offset, controllers.Length);

            if (questions.Count != controllers.Length)
                Debug.LogError("SurveyPageLoader: Cannot populate controllers with questions, fetched question count does not match controller count. <" + questions.Count + ":" + controllers.Length + ">");
            else
            {
                // Load questions in the order they are introduced
                for (int i = 0; i < controllers.Length; i++)
                {
                    controllers[i].loadQuestion(questions.Array[questions.Offset + i]);
                }
            }
        }
        else
        {
            Debug.LogWarning("SurveyPageLoader: No controllers found, no questions will be displayed.");
        }
    }
}
