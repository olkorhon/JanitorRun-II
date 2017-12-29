using AssemblyCSharp;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SurveySubmitter : MonoBehaviour
{
    string dbName = "JANITORRUN";
    string collectionName = "Surveys";

    private LikertController[] likert_controllers;

    public InputField age_field;
    public InputField email_field;
    public Toggle interview_toggle;
    public InputField comments_field;

    Constant constants;
    ServiceAPI serviceApi;
    StorageService storageService;

    void Start()
    {
        DontDestroyOnLoad(this);

        constants = new Constant();

        serviceApi = new ServiceAPI(constants.apiKey, constants.secretKey);
        storageService = serviceApi.BuildStorageService();
    }

    // Closes the game and sets the answered bit so player does not get the dialog again
    public void violentyClose()
    {
        Debug.Log("Survey violently closed");
        PlayerPrefs.SetInt("dialog_visible", 0);
        PlayerPrefs.SetInt("answered", 1);
        Destroy(this);
    }

    // Closes the dialog, just to be shown again after the next game
    public void close()
    {
        Debug.Log("Survey closed");
        PlayerPrefs.SetInt("dialog_visible", 0);
        Destroy(this.gameObject);
    }

    // Compiles and submits a log of this survey to the database
    public void compileAndSubmit()
    {
        Debug.Log("Compiling & Submitting");

        // Player has answered, no need to show the dialog ever again
        PlayerPrefs.SetInt("answered", 1);

        likert_controllers = GetComponentsInChildren<LikertController>();
        Dictionary<string, string> data = new Dictionary<string, string>();

        try
        {
            // Add identification data
            data.Add("Id", SystemInfo.deviceUniqueIdentifier);
            data.Add("Date", DateTime.Now.ToString());

            // Add special fields data
            data.Add("Age", age_field.text);
            data.Add("Email", email_field.text);
            data.Add("Interview", interview_toggle.isOn.ToString());
            data.Add("Comments", comments_field.text);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        foreach (LikertController laik in likert_controllers)
            data.Add(laik.getQuestion().Replace("\r", ""), laik.getAnswer().ToString());

        // Convert to simple json format
        StringBuilder json = new StringBuilder();

        foreach (string key in data.Keys)
            json.Append(string.Format("\"{0}\":\"{1}\",", key, data[key]));

        json.Insert(0, '{');
        json[json.Length - 1] = '}';

        string final_json_package = json.ToString().Replace("\r", "");
        Debug.Log(final_json_package);

        storageService.InsertJSONDocument(dbName, collectionName, final_json_package, new UnityCallBack());

        close();
    }

    public class UnityCallBack : App42CallBack
    {
        public void OnSuccess(object response)
        {
            Debug.Log("Successfull send");
            Storage storage = (Storage)response;
            IList<Storage.JSONDocument> jsonDocList = storage.GetJsonDocList();
            for (int i = 0; i < jsonDocList.Count; i++)
            {
                App42Log.Console("objectId is " + jsonDocList[i].GetDocId());
                App42Log.Console("Created At " + jsonDocList[i].GetCreatedAt());
            } 
        }

        public void OnException(Exception e)
        {
            Debug.Log("Exception : " + e);
            App42Log.Console("Exception : " + e);
        }
    }
}
