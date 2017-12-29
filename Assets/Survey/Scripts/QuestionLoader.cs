using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class QuestionLoader : MonoBehaviour
{
    string[] questions;
    public TextAsset questions_asset;

    // Use this for initialization
    void Awake()
    {
        Debug.Log("QuestionLoader: Starting script");

        try
        {
            questions = questions_asset.text.Split('\n');
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            questions = new string[0];
        }

        Debug.Log("QuestionLoader: Survey questions found: " + questions.Length);
    }

    public ArraySegment<string> getQuestions(int start, int count)
    {
        try
        {
            return new ArraySegment<string>(this.questions, start, count);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return new ArraySegment<string>();
        }
    }
}
