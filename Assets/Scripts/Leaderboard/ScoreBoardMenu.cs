
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Net;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.game;
using AssemblyCSharp;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that contains code for requesting 20 best scores to leaderboard from App42 cloud.
/// </summary>

/*
    Class that contains code for requesting 20 best scores to leaderboard from App42 cloud.
    Initializes the App42 service and makes the call.

    @Author: Mikael Martinviita
*/



public class ScoreBoardMenu : MonoBehaviour {

    ServiceAPI sp = null;
    ScoreBoardService scoreBoardService = null; // Initializing ScoreBoard Service.
    Constant cons = new Constant(); //Class that hold constant values
    ScoreBoardResponse callBack = new ScoreBoardResponse(); //App42 callback
    ScoreBoardListControl listControl; //Class that populates the list

    public int max = 20; //maximun number of player to show

    //Bool to check if scores added
    private bool scoresAdded = false;

    // custom validation by the client of the server certificate
    #if UNITY_EDITOR
    public static bool Validator(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    { return true; }
    #endif

    // Use this for initialization
    void Start()
    {
        //Server certificate
        #if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = Validator;
        #endif

        Dictionary<String, String> otherMetaHeaders = new Dictionary<String, String>();
        otherMetaHeaders.Add("orderByAscending", "score");// Use orderByDescending for Descending or orderByAscending for Ascending
        
        sp = new ServiceAPI(cons.apiKey, cons.secretKey); //Initialize App42 serviceAPI
        scoreBoardService = sp.BuildScoreBoardService(); // Initializing ScoreBoard Service.
        scoreBoardService.SetOtherMetaHeaders(otherMetaHeaders);
        scoreBoardService.GetTopNRankings(cons.gameName, max, callBack); //Get <max> top rankings from cloud
        listControl = this.GetComponentInChildren<ScoreBoardListControl>();
        
    }

    // Runs at every frame
    void Update()
    {
        // Do only once
        if (callBack.callDone && !scoresAdded)
        {
            listControl.AddScores(callBack.getResult(), max); // Add rankings to leaderboard
            scoresAdded = true; // disable line above
        }   
    }     
}
