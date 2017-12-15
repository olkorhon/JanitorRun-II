
using UnityEngine;
using System.Net;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.game;
using AssemblyCSharp;
using System.Collections.Generic;
using System;

/// <summary>
/// Class that contains code for saving scores to leaderboard in App42 cloud.
/// </summary>

/*
    Class that contains code for saving scores to leaderboard in App42 cloud.
    Saves player's name and time.
    SaveTimeScore called by PlayerControl when player is finished.

    @Author: Mikael Martinviita
*/


public class ScoreBoard : MonoBehaviour {

    public int num_players;

    ServiceAPI sp = null;
    ScoreBoardService scoreBoardService = null; // Initializing ScoreBoard Service.
    Constant cons = new Constant(); // Class that contains constant values for App42
    ScoreBoardSaveResponse callBack = new ScoreBoardSaveResponse(); // App42 callback

    // custom validation by the client of the server certificate
    #if UNITY_EDITOR
    public static bool Validator(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    { return true; }
    #endif

    void Start()
    {
        //Server certificate
        #if UNITY_EDITOR
        ServicePointManager.ServerCertificateValidationCallback = Validator;
        #endif

        //ServiceAPI initialization
        sp = new ServiceAPI(cons.apiKey, cons.secretKey);

        num_players = 0;
        
    }

    // Saves player's name and time to App42 cloud
    public void SaveTimeScore(string p_name, double time)
    {
        Debug.Log("time saved");
        if (time > 0)
        {
            String dbName = "JANITORRUN";
            String collectionName = "Timescores with playerID's";
            Dictionary<string, object> jsonDoc = new Dictionary<string, object>();
            jsonDoc.Add("name", p_name);
            jsonDoc.Add("time", time);
            jsonDoc.Add("ID", SystemInfo.deviceUniqueIdentifier);
            jsonDoc.Add("number of players", num_players);
            App42API.SetDbName(dbName);
            scoreBoardService = sp.BuildScoreBoardService(); // Initializing ScoreBoard Service.
            scoreBoardService.AddJSONObject(collectionName, jsonDoc);
            scoreBoardService.SaveUserScore(cons.gameName, p_name, time, callBack);
        }

    }
}
